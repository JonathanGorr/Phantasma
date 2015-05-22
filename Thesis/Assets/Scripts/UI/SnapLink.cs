using UnityEngine;
using System.Collections;

public class SnapLink : MonoBehaviour {

	private HingeJoint2D _joint;
	public DistanceJoint2D _distanceJoint;
	public bool broken;

	void Awake()
	{
		//_health = GetComponent<Health>();
		_joint = GetComponent<HingeJoint2D> ();

		if(broken)
		{
			_distanceJoint.enabled = false;
			_joint.enabled = false;
		}
	}

	public void Break()
	{
		_distanceJoint.enabled = false;
		_joint.enabled = false;
	}
}
