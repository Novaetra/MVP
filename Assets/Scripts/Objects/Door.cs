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
    
	//This displays the door cost if the player approaches it
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.GetComponent<HUDManager>().displayMsg("Door costs " + doorCost + " exp",.1f);
        }
    }
    
	//Plays the open door animation and removes the colliders
    [PunRPC]
    public void openDoor()
    {
        anim.Play("openDoor");
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
        isOpen = true;
        //Update rooms list in enemy manager
    }

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
}
