using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    Animation anim;
    public float doorCost;

    void Start()
    {
        anim = GetComponent<Animation>();
    }

    private void OnTriggerStay(Collider col)
    {
        Debug.Log("collided with " + col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            col.transform.GetComponent<HUDManager>().displayMsg("Door costs " + doorCost,1f);
        }
    }

    public void openDoor()
    {
        anim.Play("openDoor");
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
        //Update rooms list in enemy manager
    }

    public float getCost()
    {
        return doorCost;
    }
}
