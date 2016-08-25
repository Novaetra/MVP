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
        if(col.tag == "Player")
        {
            em.setCurrentRoom(transform);
            em.updateSpawnsAvailable();
        }
    }
}
