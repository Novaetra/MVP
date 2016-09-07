using UnityEngine;
using System.Collections;

public class FloorRoom : MonoBehaviour
{
    private EnemyManager em;

    private void Start()
    {
        em = GameObject.Find("Network").GetComponent<EnemyManager>();
    }

    //This is to keep track of where the player is
    private void OnTriggerEnter(Collider col)
    {
		//If a player is inside the collider
        if(col.tag == "Player")
        {
			//Set the room the player is currently in to this one
            em.setCurrentRoom(transform);
			//Updates the spawns available for enemies
            em.updateSpawnsAvailable();
        }
    }
}
