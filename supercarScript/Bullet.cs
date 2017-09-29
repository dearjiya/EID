using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col)
    {
        //the bullet was spawned on each client so normaly it would collide on each client and cause damage
        //so we allow this only on the local player (meaning: yes, I admit, I was shot)
        //furthermore only the local player has authority to send a command to the server
        if (col.gameObject.GetComponent<Combat>()!= null && col.gameObject.GetComponent<Combat>().isLocalPlayer)
        {
            col.gameObject.GetComponent<Combat>().CmdTakeDamage(col.gameObject.GetComponent<Combat>().netId, 20f);
        }
        //bullets cant destroy each other
        if (col.gameObject.tag != "Bullet")
        {
            Destroy(gameObject);
        }
        
    }
}
