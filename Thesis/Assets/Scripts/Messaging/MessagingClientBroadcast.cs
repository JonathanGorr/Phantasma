using UnityEngine;

public class MessagingClientBroadcast : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D target)
    {
		if(target.tag == "Player")
        MessagingManager.Instance.Broadcast();
    }
}
