using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    Animation anim;
    public float doorCost;
    private bool isOpen;
    [SerializeField]
    private Transform[] adjacentRooms;

	//Sets starting values
    void Start()
    {
        anim = GetComponent<Animation>();
        isOpen = false;
    }

	//Opens the door upon user interaction
    public void interact(object[] parameters)
    {
        RaycastHit hit = (RaycastHit)parameters[0];
        GameObject player = (GameObject)parameters[1];
        StatsManager sm = player.GetComponent<StatsManager>();
		//If the player has enough to buy the door, open the door (on all clients)
        if (sm.getCurrentExp() - hit.transform.GetComponentInParent<Door>().getCost() >= 0 && hit.transform.GetComponentInParent<Door>().getOpen() == false)
        {
            openDoor();
			//Subtract the exp from the player
            sm.subtractExp(hit.transform.GetComponentInParent<Door>().getCost());
        }
    }
    
	//This displays the door cost if the player approaches it
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.GetComponent<HUDManager>().displayMsg("Door costs " + doorCost + " exp",2f);
        }
    }
    
	//Plays the open door animation and removes the colliders
    public void openDoor()
    {
        anim.Play("openDoor");
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
        isOpen = true;
        //Update rooms list in enemy manager
    }
	#region getters
    public float getCost()
    {
        return doorCost;
    }

    public bool getOpen()
    {
        return isOpen;
    }

    public Transform[] getAdjacentRooms()
    {
        return adjacentRooms;
    }
	#endregion
}
