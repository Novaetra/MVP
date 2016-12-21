using UnityEngine;
using System.Collections;

public class FloorRoom : MonoBehaviour
{
    private EnemyManager em;
    private bool setUpDone = false;

    private void Start()
    {
        em = GameObject.Find("Managers").GetComponent<EnemyManager>();
        StartCoroutine(waitToFinishSetup());
    }

    private IEnumerator waitToFinishSetup()
    {
        yield return new WaitForSeconds(1.0f);
        setUpDone = true;
    }

    //This is to keep track of where the player is
    private void OnTriggerEnter(Collider col)
    {
        if(setUpDone==true)
        {
            //If a player is inside the collider
            if (col.tag == "Player")
            {
                //Set the room the player is currently in to this one
                em.setCurrentRoom(transform);
                //Updates the spawns available for enemies so they spawn in this room too
                em.updateSpawnsAvailable();
            }
        }
    }
}
