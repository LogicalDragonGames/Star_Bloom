using UnityEngine;
using System.Collections;

public class ColliderBoundsEx : MonoBehaviour
{
	public enum VertexType
	{
		MaxTR = 0,
		MaxTL,
		MaxBR,
		MaxBL,
		MinTR,
		MinTL,
		MinBR,
		MinBL,
		MaxCP,
		MinCP,
	};
	
	public Vector3[] ColliderVertices = null;
	public bool PerFrameUpdate = false;
	public bool RenderDebug = false;

	protected Vector3 m_PrevPosition;
	protected Quaternion m_PrevRotation;
	protected Vector3 m_PrevScale;

	void Awake()
	{
		UpdateVertices();
	}
	
	void Update()
	{
		if( PerFrameUpdate )
			UpdateVertices();
	}

	void UpdateVertices()
	{
		Bounds bounds = this.collider.bounds;
		ColliderVertices = GetColliderVertexPositions( this.gameObject );
	}

	public Vector3 GetColliderVertex( VertexType _type )
	{
		return ColliderVertices[ (int)_type ];
	}

	static Vector3[] GetColliderVertexPositions( GameObject obj )
	{
		Vector3[] vertices = new Vector3[10];
		Matrix4x4 thisMatrix = obj.transform.localToWorldMatrix;
		Quaternion storedRotation = obj.transform.rotation;
		Vector3 storedScale = obj.transform.localScale;
		obj.transform.rotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;
		
		Vector3 extents = obj.collider.bounds.extents;
		
		vertices[0] = thisMatrix.MultiplyPoint3x4(extents);											// MaxTR
		vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));	// MaxTL
		vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));	// MaxBR
		vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));	// MaxBL
		vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));	// MinTR
		vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));	// MinTL
		vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));	// MinBR
		vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);										// MinBL
		
		vertices[8] = vertices[(int)VertexType.MaxTL]*0.5f + vertices[(int)VertexType.MaxBR]*0.5f;	// MaxCP
		vertices[9] = vertices[(int)VertexType.MinTL]*0.5f + vertices[(int)VertexType.MinBR]*0.5f;	// MinCP
		
		obj.transform.rotation = storedRotation;
		obj.transform.localScale = storedScale;
		return vertices;
	}
}
