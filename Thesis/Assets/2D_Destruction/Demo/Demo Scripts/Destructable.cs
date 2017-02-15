using UnityEngine;
using System.Collections;

/// <summary>
/// Destructable:
/// Used to break objects by fragmentation.
/// Also triggers particle system and item/treasure spawning.
/// </summary>

[RequireComponent(typeof(Explodable))]
public class Destructable : MonoBehaviour {

	private SFX _sfx;
	public Explodable _explodable;
	public Rigidbody2D[] dependents;
	public ParticleSystem particles;
	bool exploded = false;

	[Header("Reward")]
	public Treasure treasure;

	void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
	}

	[Header("Explosion Parms")]
	public float force = 50;
	public float radius = 5;
	public float upliftModifer = 5;

	public void Explode(Vector3 position)
	{
		if(exploded) return;

		exploded = true;
		if(treasure) treasure.Dispense();
		_sfx.PlayFX("smash", position);

		//detach and play particles to death
		particles.transform.SetParent(null);
		particles.Play();

		//activate/drop all rigidbodies being held up by this object( flower pot ).
		for(int i=0;i<dependents.Length;i++)
		{
			if(dependents[i] == null) continue;
			dependents[i].gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			dependents[i].transform.SetParent(null);
			dependents[i].isKinematic = false;
			dependents[i].simulated = true;
			if(dependents[i].GetComponent<FadeOutSprite>())
				dependents[i].GetComponent<FadeOutSprite>().Fade();
		}
		_explodable.explode();
		//StartCoroutine(waitAndExplode(position));
	}

    /// <summary>
    /// exerts an explosion force on all rigidbodies within the given radius
    /// </summary>
    /// <returns></returns>
	private IEnumerator waitAndExplode(Vector3 position)
	{
		yield return new WaitForFixedUpdate();
		
		for(int i=0; i<transform.childCount - 1;i++)
		{
			if(transform.GetChild(i).GetComponent<Rigidbody2D>())
				AddExplosionForce(transform.GetChild(i).GetComponent<Rigidbody2D>(), force, position, radius, upliftModifer);
		}
	}

	/// <summary>
    /// adds explosion force to given rigidbody
    /// </summary>
    /// <param name="body">rigidbody to add force to</param>
    /// <param name="explosionForce">base force of explosion</param>
    /// <param name="explosionPosition">location of the explosion source</param>
    /// <param name="explosionRadius">radius of explosion effect</param>
    /// <param name="upliftModifier">factor of additional upward force</param>
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
	{
		var dir = (body.transform.position - explosionPosition);	
		float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        baseForce.z = 0;
		body.AddForce(baseForce);

        if (upliftModifer != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
        }
	}
}
