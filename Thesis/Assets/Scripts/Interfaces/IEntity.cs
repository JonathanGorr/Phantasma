using UnityEngine;

public interface IEntity {

	string Title { get; }
	string Description { get; }
	Vector2 LookDirection { get; set; }
	void OnHurt(Entity offender); //health passes the id of the offending entity to set target
	void OnDeath();
	void Attack();
	void Block();
	void Roll();
	void Jump();
	void BackStep();
	void RotateBody();
	void SetPosition(Vector2 pos);
	Facing Facing { get; set; }
}
