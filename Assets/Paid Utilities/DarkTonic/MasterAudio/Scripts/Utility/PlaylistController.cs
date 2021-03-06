using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlaylistController : MonoBehaviour {
	public bool startPlaylistOnAwake = true;
	public bool isShuffle = false;
	public bool isAutoAdvance = true;
	public float playlistVolume = 1f;
	public bool isMuted = false;
	public string startPlaylistName = string.Empty;
	
	private AudioSource activeAudio;
	private AudioSource transitioningAudio;
	private float activeAudioEndVolume;
	private float transitioningAudioStartVolume;
	private bool isCrossFading = false;
	private float crossFadeStartTime;
	private List<int> clipsRemaining = new List<int>();
	private int currentSequentialClipIndex;
	private AudioDuckingMode duckingMode;
	private float timeToStartUnducking;
	private float timeToFinishUnducking;
	private float originalMusicVolume;
	private float initialDuckVolume;
	private float duckRange;
	private MusicSetting currentSong;
	private GameObject go;
	private FadeMode curFadeMode = FadeMode.None;
	private float slowFadeTargetVolume;
	private float slowFadeVolStep;
	private MasterAudio.Playlist currentPlaylist = null;
	private float lastTimeMissingPlaylistLogged = -5f;
	private System.Action fadeCompleteCallback;
	
    public delegate void SongChangedEventHandler(string newSongName);
    public event SongChangedEventHandler SongChanged;
	
	private static List<PlaylistController> _instances = null;
	
	public enum PlaylistStates {
		NotInScene,
		Stopped,
		Playing,
		Paused,
		Crossfading
	}
	
	public enum FadeMode {
		None,
		GradualFade
	}
	
	public enum AudioDuckingMode {
		NotDucking,
		SetToDuck,
		Ducked
	}
	
	#region Monobehavior events
	void Awake() {
		this.useGUILayout = false;
		duckingMode = AudioDuckingMode.NotDucking;
		currentSong = null;
		
		var audios = this.GetComponents<AudioSource>();
		if (audios.Length < 2)
		{
			Debug.LogError("This prefab should have exactly two Audio Source components. Please revert it.");
			return;
		}
		
		AudioSource audio1 = audios[0];
		AudioSource audio2 = audios[1];
		
		audio1.clip = null;
		audio2.clip = null;
		
		activeAudio = audio1;
		transitioningAudio = audio2;
		go = this.gameObject;
		curFadeMode = FadeMode.None;
		fadeCompleteCallback = null;
	}
	
	// Use this for initialization 
	void Start () {
		// fill up randomizer
		InitializePlaylist();
		
		if (currentPlaylist != null && startPlaylistOnAwake) {
			PlayNextOrRandom();
		}
		
		StartCoroutine(this.CoStart());
	}
	
	IEnumerator CoStart() {
		while (true) {
			yield return StartCoroutine(this.CoUpdate());
		}
	}
	
	IEnumerator CoUpdate() {
		yield return new WaitForSeconds(MasterAudio.INNER_LOOP_CHECK_INTERVAL);
		
		// gradual fade code
		if (curFadeMode != FadeMode.GradualFade) {
			yield break;
		}
		
		if (activeAudio == null) {
			yield break; // paused or error in setup
		}
		
		var newVolume = PlaylistVolume + slowFadeVolStep;
		
		if (slowFadeVolStep > 0f) {
			newVolume = Math.Min(newVolume, slowFadeTargetVolume);	
		} else {
			newVolume = Math.Max(newVolume, slowFadeTargetVolume);	
		}
		
		playlistVolume = newVolume;
		UpdateMasterVolume();
		
		if (newVolume == slowFadeTargetVolume) {
			if (fadeCompleteCallback != null) {
				fadeCompleteCallback();
				fadeCompleteCallback = null;
			}
			curFadeMode = FadeMode.None;
		}
	}
	
	void FixedUpdate() {
		if (isCrossFading) {
			// cross-fade code
			if (activeAudio.volume >= activeAudioEndVolume) {
				CeaseAudioSource(transitioningAudio);
				isCrossFading = false;
				SetDuckProperties(); // they now should read from a new audio source
			}
			
			var ratioPassed = (Time.time - crossFadeStartTime) / MasterAudio.Instance.CrossFadeTime;
			
			activeAudio.volume = ratioPassed * activeAudioEndVolume;
			transitioningAudio.volume = transitioningAudioStartVolume * (1 - ratioPassed);
			
			// end cross-fading code
		} 
		
		if (IsAutoAdvance && !activeAudio.loop && activeAudio.clip != null) {
			var currentClipTime = activeAudio.clip.length - activeAudio.time - (MasterAudio.Instance.CrossFadeTime * activeAudio.pitch);
			var clipFadeStartTime = Time.deltaTime * EventCalcSounds.FRAMES_EARLY_TO_TRIGGER * activeAudio.pitch;
			if (currentClipTime < clipFadeStartTime) {
				PlayNextOrRandom();
			}
		}
		
		if (isCrossFading) {
			return;
		}
		
		this.AudioDucking();
	}
	#endregion
	
	#region public methods
	
	/// <summary>
	/// This method returns a reference to the PlaylistController whose name you specify. This is necessary when you have more than one.
	/// </summary>
	/// <param name="playlistControllerName"></param>
	/// <returns></returns>
	public static PlaylistController InstanceByName(string playlistControllerName) {
		var match = Instances.Find(delegate(PlaylistController obj) {
			return obj != null && obj.name == playlistControllerName;	
		});
		
		if (match != null) {
			return match;
		}
		
		Debug.LogError("Could not find Playlist Controller '" + playlistControllerName + "'.");
		return null;
	}
	
	/// <summary>
	/// This method is used by Master Audio to update conditions based on the Ducked Volume Multiplier changing.
	/// </summary>
	public void UpdateDuckedVolumeMultiplier() {
		if (Application.isPlaying) {
			SetDuckProperties();
		}
	}
	
	/// <summary>
	/// This method will pause the Playlist.
	/// </summary>
	public void PausePlaylist() {
		if (activeAudio == null || transitioningAudio == null) {
			return;
		}
		
		activeAudio.Pause();
		transitioningAudio.Pause();
	}
	
	/// <summary>
	/// This method will unpause the Playlist.
	/// </summary>
	public bool ResumePlaylist() {
		if (activeAudio == null || transitioningAudio == null) {
			return false;
		}
		
		if (activeAudio.clip == null) {
			return false;
		}
		
		activeAudio.Play();
		transitioningAudio.Play();
		return true;
	}
	
	/// <summary>
	/// This method will Stop the Playlist. Cross-fading will still happen (cross-fading to no new song).
	/// </summary>
	public void StopPlaylist() {
		if (!Application.isPlaying) {
			return;
		}
		
		currentSong = null;
		CeaseAudioSource(this.activeAudio);
		CeaseAudioSource(this.transitioningAudio);
	}
	
	/// <summary>
	/// This method allows you to fade the Playlist to a specified volume over X seconds.
	/// </summary>
	/// <param name="targetVolume">The volume to fade to.</param>
	/// <param name="fadeTime">The amount of time to fully fade to the target volume.</param>
	public void FadeToVolume(float targetVolume, float fadeTime, System.Action callback = null) {
		if (fadeTime <= MasterAudio.INNER_LOOP_CHECK_INTERVAL) {
			playlistVolume = targetVolume;
			UpdateMasterVolume();
			return;			
		}
		
		curFadeMode = FadeMode.GradualFade;
		slowFadeTargetVolume = targetVolume;
		slowFadeVolStep = (slowFadeTargetVolume - PlaylistVolume) / (fadeTime / MasterAudio.INNER_LOOP_CHECK_INTERVAL);
		fadeCompleteCallback = callback;
	}
	
	/// <summary>
	/// This method will play a random song in the current Playlist.
	/// </summary>
	public void PlayRandomSong() {
		if (clipsRemaining.Count == 0) {
			Debug.LogWarning("There are no playlist clips to play.");
			return;
		}
		
		var randIndex = UnityEngine.Random.Range(0, clipsRemaining.Count);
		var clipIndex = clipsRemaining[randIndex];
		
		PlayPlaylistSong(currentPlaylist.MusicSettings[clipIndex]);
		
		clipsRemaining.RemoveAt(randIndex);
		if (clipsRemaining.Count == 0) {
			FillClips();
		}
	}
	
	/// <summary>
	/// This method will play the next song in the current Playlist.
	/// </summary>
	public void PlayNextSong() {
		if (currentPlaylist == null) {
			return;
		}
		
		PlayPlaylistSong(currentPlaylist.MusicSettings[currentSequentialClipIndex]);
		currentSequentialClipIndex++;
		
		if (currentSequentialClipIndex >= currentPlaylist.MusicSettings.Count) {
			currentSequentialClipIndex = 0;
		}
	}
	
	/// <summary>
	/// This method will play the song in the current Playlist whose name you specify.
	/// </summary>
	/// <param name="clipName">The name of the song to play.</param>
	public void TriggerPlaylistClip(string clipName) {
		if (currentPlaylist == null) {
			MasterAudio.LogNoPlaylist(this.name, "TriggerPlaylistClip");
			return;
		}
		
		MusicSetting setting = currentPlaylist.MusicSettings.Find(delegate(MusicSetting obj) {
			return obj.clip.name == clipName;
		});
		
		if (setting == null) {
			Debug.LogWarning("Could not find clip '" + clipName + "' in current Playlist in '" + this.name + "'.");
			return;
		}
		
		PlayPlaylistSong(setting);
	}
	
	public void DuckMusicForTime(float duckLength, float pitch, float duckedTimePercentage) {
		if (isCrossFading) {
			return; // no ducking during cross-fading, it screws up calculations.
		}
		
		var rangedDuck = duckLength / pitch;
		
		duckingMode = AudioDuckingMode.SetToDuck;
		timeToStartUnducking = Time.time + (rangedDuck * duckedTimePercentage);
		timeToFinishUnducking = Math.Max(Time.time + rangedDuck, timeToStartUnducking);
	}
	
	/// <summary>
	/// This method is used to update state based on the Playlist Master Volume.
	/// </summary>
	public void UpdateMasterVolume() {
		if (!Application.isPlaying) {
			return;
		}
		
		if (activeAudio != null && currentSong != null && !IsCrossFading) {
			activeAudio.volume = currentSong.volume * PlaylistVolume;
		}
		
		if (currentSong != null) {
			activeAudioEndVolume = currentSong.volume * PlaylistVolume;
		}
		
		SetDuckProperties();
	}
	
	
	/// <summary>
	/// This method is used to change the current Playlist to a new one, and optionally start it playing.
	/// </summary>
	public void ChangePlaylist(string playlistName, bool playFirstClip = true) {
		startPlaylistName = playlistName;
		
		StopPlaylist();
		
		InitializePlaylist();
		
		if (!Application.isPlaying) {
			return;
		}
		
		if (playFirstClip) {
			PlayNextOrRandom();
		}
	}
	
	#endregion
	
	#region Helper methods
	private void InitializePlaylist()
	{
		FillClips();
		currentSequentialClipIndex = 0;
	}
	
	private void PlayNextOrRandom()
	{
		if (!isShuffle) {
			PlayNextSong();
		} else {
			PlayRandomSong();
		}
	}
	
	private void FillClips() {
		clipsRemaining.Clear();
		
		// add clips from named playlist.
		if (startPlaylistName == MasterAudio.NO_PLAYLIST_NAME) {
			return;
		}
		this.currentPlaylist = MasterAudio.GrabPlaylist(startPlaylistName);
		
		
		if (this.currentPlaylist == null) {
			return;
		}
		
		MusicSetting aSong = null;
		
		for (var i = 0; i < currentPlaylist.MusicSettings.Count; i++) {
			aSong = currentPlaylist.MusicSettings[i];
			if (aSong.clip == null) {
				continue;
			}
			
			clipsRemaining.Add(i);
		}
	}
	
	private void PlayPlaylistSong(MusicSetting setting) {
		AudioSource audioClip;
		AudioSource transClip;
		
		if (activeAudio == null) {
			Debug.LogError("PlaylistController prefab is not in your scene. Cannot play a song.");
			return;
		}
		
		if (activeAudio.clip == null) {
			audioClip = activeAudio;
			transClip = transitioningAudio;
		} else if (transitioningAudio.clip == null) {
			audioClip = transitioningAudio;
			transClip = activeAudio;
		} else {
			// both are busy!
			//Debug.LogWarning("Both audio sources are busy cross-fading. You may want to shorten your cross-fade time in Playlist Inspector.");
			audioClip = transitioningAudio;
			transClip = activeAudio;
		}
		
		if (setting.clip != null) {
			audioClip.clip = setting.clip;
			audioClip.pitch = setting.pitch;
		}
		
		audioClip.loop = SongShouldLoop(setting);
		audioClip.clip = setting.clip;
		audioClip.pitch = setting.pitch;
		
		// set last know time for current song.
		if (currentSong != null) {
			currentSong.lastKnownTimePoint = activeAudio.timeSamples;
		}
		
		if (MasterAudio.Instance.CrossFadeTime == 0 || transClip.clip == null) {
			CeaseAudioSource(transClip);
			audioClip.volume = setting.volume * PlaylistVolume;
		} else {
			audioClip.volume = 0f;
			isCrossFading = true;
			duckingMode = AudioDuckingMode.NotDucking;
			crossFadeStartTime = Time.time;
		}
		
		SetDuckProperties();
		
		if (currentPlaylist != null) {
			switch (currentPlaylist.songTransitionType) {
				case MasterAudio.SongFadeInPosition.SynchronizeClips:
					transitioningAudio.timeSamples = activeAudio.timeSamples;
					break;
				case MasterAudio.SongFadeInPosition.NewClipFromLastKnownPosition:
					var thisSongInPlaylist = currentPlaylist.MusicSettings.Find(delegate(MusicSetting obj) {
						return obj == setting;
					});
				
					if (thisSongInPlaylist != null) {
						transitioningAudio.timeSamples = thisSongInPlaylist.lastKnownTimePoint;
					}
					break;
			}
		}
		
		if (SongChanged != null) {
			var clipName = String.Empty;
			if (audioClip != null) {
				clipName = audioClip.clip.name;
			}
			SongChanged(clipName);
		}
		
		audioClip.Play();
		
		activeAudio = audioClip;
		transitioningAudio = transClip;
		
		activeAudioEndVolume = setting.volume * PlaylistVolume;
		transitioningAudioStartVolume = transitioningAudio.volume * PlaylistVolume;
		currentSong = setting;
	}
	
	private void CeaseAudioSource(AudioSource source) {
		if (source == null) {
			return;
		}
		
		source.Stop();
		source.clip = null;
	}
	
	private void SetDuckProperties() {
		originalMusicVolume = activeAudio.volume;
		if (currentSong != null) {
			originalMusicVolume = currentSong.volume * PlaylistVolume;
		}
		
		initialDuckVolume = MasterAudio.Instance.DuckedVolumeMultiplier * originalMusicVolume;
		duckRange = originalMusicVolume - MasterAudio.Instance.DuckedVolumeMultiplier;
		
		duckingMode = AudioDuckingMode.NotDucking; // cancel any ducking
	}
	
	private void AudioDucking() {
		switch (duckingMode) {
		case AudioDuckingMode.NotDucking:
			break;
		case AudioDuckingMode.SetToDuck:
			activeAudio.volume = initialDuckVolume;
			duckingMode = AudioDuckingMode.Ducked;
			break;
		case AudioDuckingMode.Ducked:
			if (Time.time >= timeToFinishUnducking) {
				activeAudio.volume = originalMusicVolume;
				duckingMode = AudioDuckingMode.NotDucking;
			} else if (Time.time >= timeToStartUnducking) { 
				activeAudio.volume = initialDuckVolume + (Time.time - timeToStartUnducking) / (timeToFinishUnducking - timeToStartUnducking) * duckRange;
			}
			break;
		} 
	}
	
	private bool SongShouldLoop(MusicSetting setting) {
		if (CurrentPlaylist != null && CurrentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {		
			return true;
		}
		
		return setting.isLoop;
	}
	#endregion
	
	#region Properties
	/// <summary>
	/// This property returns the current state of the Playlist. Choices are: NotInScene, Stopped, Playing, Paused, Crossfading
	/// </summary>
	public PlaylistStates PlaylistState {
		get {
			if (this.activeAudio == null || this.transitioningAudio == null) {
				return PlaylistStates.NotInScene;
			}
			
			if (!ActiveAudioSource.isPlaying) {
				if (ActiveAudioSource.time != 0f) {
					return PlaylistStates.Paused;
				} else {
					return PlaylistStates.Stopped;
				}
			}

			if (isCrossFading) {
				return PlaylistStates.Crossfading;
			}
			
			return PlaylistStates.Playing;
		}
	}
	
	private AudioSource ActiveAudioSource {
		get {
			if (activeAudio.clip == null) {
				return transitioningAudio;
			} else {
				return activeAudio;
			}
		}
	}
	
	/// <summary>
	/// This property returns all for the PlaylistControllers in the Scene.
	/// </summary>
	public static List<PlaylistController> Instances {
		get {
			if (_instances == null) {
				_instances = new List<PlaylistController>();
				
				var controllers = GameObject.FindObjectsOfType(typeof(PlaylistController));
				for (var i = 0; i < controllers.Length; i++) {
					_instances.Add(controllers[i] as PlaylistController);
				}
			}
			
			return _instances;
		}
		set {
			// only for non-caching.
			_instances = value;
		}
	}
	
	/// <summary>
	/// This property returns the GameObject for the PlaylistController's GameObject.
	/// </summary>
	public GameObject PlaylistControllerGameObject {
		get {
			return go;
		}
	}
	
	/// <summary>
	///  This property returns the current Audio Source for the current Playlist song that is playing.
	/// </summary>
	public AudioSource CurrentPlaylistSource {
		get {
			if (activeAudio == null) {
				return null;
			}
			
			return activeAudio;
		}
	}
	
	/// <summary>
	///  This property returns the current Audio Clip for the current Playlist song that is playing.
	/// </summary>
	public AudioClip CurrentPlaylistClip {
		get {
			if (activeAudio == null) {
				return null;
			}
			
			return activeAudio.clip;
		}
	}
	
	/// <summary>
	/// This property returns the currently fading out Audio Clip for the Playlist (null if not during cross-fading).
	/// </summary>
	public AudioClip FadingPlaylistClip {
		get {
			if (!isCrossFading) {
				return null;
			}
			
			if (transitioningAudio == null) {
				return null;
			}
			
			return transitioningAudio.clip;
		}
	}
	
	/// <summary>
	/// This property returns the currently fading out Audio Source for the Playlist (null if not during cross-fading).
	/// </summary>
	public AudioSource FadingSource {
		get {
			if (!isCrossFading) {
				return null;
			}
			
			if (transitioningAudio == null) {
				return null;
			}
			
			return transitioningAudio;
		}
	}
	
	/// <summary>
	/// This property returns whether or not the Playlist is currently cross-fading.
	/// </summary>
	public bool IsCrossFading {
		get {
			return isCrossFading;
		} 
	}
	
	/// <summary>
	/// This property gets and sets the volume of the Playlist Controller with Master Playlist Volume taken into account.
	/// </summary>
	public float PlaylistVolume {
		get {
			return MasterAudio.PlaylistMasterVolume * playlistVolume;
		}
		set {
			playlistVolume = value;
		}
	}
	
	/// <summary>
	/// This property returns the current Playlist
	/// </summary>
	public MasterAudio.Playlist CurrentPlaylist {
		get {
			if (currentPlaylist == null && Time.time - lastTimeMissingPlaylistLogged > 2f) {
				Debug.LogWarning("Current Playlist is NULL. Subsequent calls will fail.");
				lastTimeMissingPlaylistLogged = Time.time;
			}
			return currentPlaylist;
		}
	}
	
	/// <summary>
	/// This property returns the name of the current Playlist
	/// </summary>
	public string PlaylistName {
		get {
			return CurrentPlaylist.playlistName;
		}
	}
	
	/// <summary>
	/// This property returns whether the current Playlist is muted or not
	/// </summary>
	public bool IsMuted {
		get {
			return isMuted;
		}
		set {
			isMuted = value;
			
			if (Application.isPlaying) {
				if (activeAudio != null) {
					activeAudio.mute = value;
				}
				
				if (transitioningAudio != null) {
					transitioningAudio.mute = value;
				}
			} else {
				var audios = this.GetComponents<AudioSource>();
				for (var i = 0; i < audios.Length; i++) {
					audios[i].mute = value;
				}
			}
		}
	}
	
	private bool IsAutoAdvance {
		get {
			if (CurrentPlaylist != null && CurrentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {
				return false;	
			}
			
			return isAutoAdvance;
		}
	}
	#endregion
}