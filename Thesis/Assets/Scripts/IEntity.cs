using UnityEngine;

public interface IEntity {

	string Title { get; }
	string Description { get; }
	bool FacingLeft { get; set; }
	Vector2 LookDirection { get; set; }
	void Heal();
	void OnHurt();
	void OnDeath();
	void Attack();
	void Block();
	void Roll();
	void BackStep();
	void RotateBody();
}
