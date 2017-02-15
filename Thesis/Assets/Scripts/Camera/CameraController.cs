using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Camera2 D follow.
/// Averages center of multiple listed targets.
/// </summary>

public class CameraController : MonoBehaviour {

	public Camera m_Camera;
	[Header("Target Averaging")]
	public float m_DampTime = 0.2f;
	public float m_ScreenEdgeBuffer = 4f;
	public float m_MinSize = 6.5f;
	public float height = 30f;
	/*[HideInInspector]*/ public List<Transform> m_Targets = new List<Transform>();

	private float m_ZoomSpeed;
	private Vector3 m_MoveVelocity;
	private Vector3 m_DesiredPosition;

	// Use this for initialization
	void Awake () 
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Start")
		{
			SetStartPositionAndSize();
		}
	}

	//used by targets to register themselves to this camera
	public void RegisterMe(Transform t)
	{
		if(!m_Targets.Contains(t))
		{
			m_Targets.Add(t);
		}
	}
	//used by targets to deregister themselves from this camera
	public void UnRegisterMe(Transform t)
	{
		if(m_Targets.Contains(t))
		{
			m_Targets.Remove(t);
		}
	}

	void FixedUpdate () 
	{
		if(m_Targets.Count < 1) return;
		Vector2 pos = Move();
		transform.position = new Vector3(pos.x, pos.y, Zoom());
		Zoom();
	}

	Vector2 Move()
	{
		m_DesiredPosition = FindAveragePosition();
		//ref writes back to m_MoveVelocity
		return Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
	}

	Vector3 FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;
		for(int i=0;i<m_Targets.Count;i++)
		{
			//ignore if not active/dead
			if(!m_Targets[i].gameObject.activeSelf)
				continue;//continue onto the next iteration of the loop! don't execute code below
			averagePos += m_Targets[i].position;
			numTargets++;
		}
		//if there are active targets
		if(numTargets > 0)
		{
			//divide averagepos by number of targets /=
			averagePos /= numTargets;
		}
		//don't chnage camera z
		averagePos.z = transform.position.z;
		averagePos.y += height;
		return averagePos;
	}

	float Zoom()
	{
		return Mathf.SmoothDamp(m_Camera.transform.position.z, FindRequiredDistance(), ref m_ZoomSpeed, m_DampTime);
	}

	float FindRequiredDistance()
	{
		//find the desired location of the camera from the camera's local grid
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
		//most disparate
		float size = 0;
		//go through all the targets and find the one furthest away from the desired local pos
		for(int i=0;i<m_Targets.Count;i++)
		{
			if(!m_Targets[i].gameObject.activeSelf)
			continue;
			//put the target in the camera's local grid
			Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
			//get a direction vector from camera to target
			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
			//is the size currently bigger than this distance vector
			size = Mathf.Min(size, Mathf.Abs(desiredPosToTarget.y));
			//do the same thing for x, but compensate for aspect, because x is always at least 50% longer
			size = Mathf.Min(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
		}
		//give the size some extra distance or padding on screen edge
		size -= m_ScreenEdgeBuffer;
		//if we're too zoomed in, take the min size if it's ever larger
		size = Mathf.Min(size, m_MinSize);
		return size;
	}

	void SetStartPositionAndSize()
	{
		transform.position = FindAveragePosition() + new Vector3(0, height, 0);
	}
}
