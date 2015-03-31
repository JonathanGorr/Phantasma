using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	//create constants to keep track of state
	public const int IDLE = 0;
	public const int OPENING = 1;
	public const int OPEN = 2;
	public const int CLOSING = 3;

	public float closeDelay = .5f;

	//keeps track of state
#pragma warning disable 0414
	private int state = IDLE;
#pragma warning restore 0414
	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator> ();
	}

	void OnOpenStart()
	{
		state = OPENING;
	}

	void OnOpenEnd()
	{
		state = OPEN;
	}

	void OnCloseStart()
	{
		state = CLOSING;
	}

	void OnCloseEnd()
	{
		state = IDLE;
	}

	void DissableCollider2D()
	{
		collider2D.enabled = false;
	}

	void EnableCollider2D()
	{
		collider2D.enabled = true;
	}

	public void Open()
	{
		animator.SetInteger ("AnimState", 1);
	}

	public void Close()
	{
		//takes method and calls in separate method
		StartCoroutine (CloseNow ());
	}

	private IEnumerator CloseNow()
	{
		yield return new WaitForSeconds(closeDelay);
		//this takes place after delay
		animator.SetInteger ("AnimState", 2);
	}
}