using UnityEngine;
using System.Collections;

public class CollisionField : MonoBehaviour
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
	public bool RenderDebug = false;

	ArrayList m_CollidingObjs = new ArrayList();
	Vector3 m_Front;
	Vector3 m_Back;

	public delegate void ObjectActionHandler( FieldObject obj );
	public ObjectActionHandler ObjectMoved;
	public ObjectActionHandler ObjectEntered;
	public ObjectActionHandler ObjectLeft;

	void Start()
	{
	}
	
	void Update()
	{
		if( m_CollidingObjs.Count == 0 )
			return;

		Bounds bounds = this.collider.bounds;
		ColliderVertices = GetColliderVertexPositions( this.gameObject );

		if( RenderDebug )
		{
			Debug.DrawLine( ColliderVertices[(int)VertexType.MinCP], ColliderVertices[(int)VertexType.MaxCP] );
			Debug.DrawLine( ColliderVertices[(int)VertexType.MinCP] + Vector3.up * bounds.size.y * 2, ColliderVertices[(int)VertexType.MaxCP] + Vector3.up * bounds.size.y * 2 );
			Debug.DrawLine( ColliderVertices[(int)VertexType.MinCP], ColliderVertices[(int)VertexType.MinCP] + Vector3.up * bounds.size.y * 2 );
			Debug.DrawLine( ColliderVertices[(int)VertexType.MaxCP], ColliderVertices[(int)VertexType.MaxCP] + Vector3.up * bounds.size.y * 2 );
			Debug.DrawLine( ColliderVertices[(int)VertexType.MinCP] + (ColliderVertices[(int)VertexType.MaxCP] - ColliderVertices[(int)VertexType.MinCP])*0.5f,
			               ColliderVertices[(int)VertexType.MinCP] + (ColliderVertices[(int)VertexType.MaxCP] - ColliderVertices[(int)VertexType.MinCP])*0.5f + Vector3.up * bounds.size.y * 2, Color.yellow );
		}

		Vector3 minToMaxCP = ColliderVertices[(int)VertexType.MaxCP] - ColliderVertices[(int)VertexType.MinCP];
		float minToMaxCPMag = minToMaxCP.magnitude;

		if( minToMaxCPMag == 0.0f )
			return;

		for( int i = 0, size = m_CollidingObjs.Count; i < size; ++i )
		{
			FieldObject obj = (FieldObject)m_CollidingObjs[i];
			Transform objTrans = obj.collider.transform;

			Vector3 projectedPoint = ProjectPointToLine( ColliderVertices[(int)VertexType.MinCP], ColliderVertices[(int)VertexType.MaxCP], objTrans.position ); 

			if( RenderDebug )
			{
				Debug.DrawLine( objTrans.position, projectedPoint, Color.yellow );
				Debug.DrawLine( objTrans.position, projectedPoint + Vector3.up * bounds.size.y * 2.0f, Color.yellow );
				Debug.DrawLine( projectedPoint, projectedPoint + Vector3.up * bounds.size.y * 2.0f, Color.red );
			}

			Vector3 toProjPoint = projectedPoint - ColliderVertices[(int)VertexType.MinCP];
			float toProjPointMag = toProjPoint.magnitude;

			float perc = toProjPointMag / minToMaxCPMag;
			perc = Mathf.Clamp( perc, 0.0f, 1.0f );

			if( obj.percent != perc )
			{
				obj.percent = perc;
				OnObjectAction( ObjectMoved, obj );
			}
		}
	}

	void OnObjectAction( ObjectActionHandler handler, FieldObject obj )
	{
		if( null != handler )
			handler( obj );
	}

	void OnTriggerEnter( Collider other )
	{
		foreach( FieldObject obj in m_CollidingObjs )
			if( obj.collider == other )
				return;

		FieldObject fObj = new FieldObject();
		fObj.collider = other;
		fObj.percent = (-1.0f);

		m_CollidingObjs.Add( fObj );
		OnObjectAction( ObjectEntered, fObj );
	}

	void OnTriggerExit( Collider other )
	{
		FieldObject fObj = null;

		foreach( FieldObject obj in m_CollidingObjs )
			if( obj.collider == other )
			{
				fObj = obj;
				break;
			}

		if( null != fObj )
		{
			fObj.percent = Mathf.Round( fObj.percent );
			OnObjectAction( ObjectMoved, fObj );
			m_CollidingObjs.Remove( fObj );
			OnObjectAction( ObjectLeft, fObj );
		}
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

	static Vector3 ProjectPointToLine( Vector3 lineBegin, Vector3 lineEnd, Vector3 point )
	{
		return Vector3.Project((point-lineBegin),(lineEnd-lineBegin))+lineBegin;
	}
}
