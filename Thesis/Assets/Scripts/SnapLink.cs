using UnityEngine;
using System.Collections;

public class SnapLink : MonoBehaviour {

	private HingeJoint2D _joint;
	public DistanceJoint2D _distanceJoint;
	//private Health _health;

	void Awake()
	{
		//_health = GetComponent<Health>();
		_joint = GetComponent<HingeJoint2D> ();
	}

/*
	void FixedUpdate()
	{
		if(_health.health <= 0)
		{
			_distanceJoint.enabled = false;
			_joint.enabled = false;
		}
	}
*/
	void OnTriggerEnter2D(Collider2D arrow)
	{
		if(arrow.gameObject.tag == "Arrow")
		{
			_distanceJoint.enabled = false;
			_joint.enabled = false;
		}
	}
}
