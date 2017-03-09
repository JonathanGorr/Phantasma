using UnityEngine;
using System.Collections;

public class RustyLinkHealth : Health {

	public HingeJoint2D _joint;
	public DistanceJoint2D _distanceJoint;
	public bool broken;

	public override void UpdateHealthBar()
	{
		
	}

	public override void OnKill()
	{
		_distanceJoint.enabled = false;
		_joint.enabled = false;
	}
}
