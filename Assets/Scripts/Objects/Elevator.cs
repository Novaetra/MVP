using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{

    private bool isDown = true;
    private GameObject player;

	public void interact(object[] paramtrs)
    {
        player = (GameObject)paramtrs[1];
        if(isDown)
        {
            GetComponent<Animation>().Play("ElevatorUp");
        }else
        {
            GetComponent<Animation>().Play("ElevatorDown");
        }
        player.transform.parent = transform;
    }

    private void resetPlayerParent()
    {
        player.transform.parent = null;
    }
    
    private void OnTriggerExit(Collider col)
    {
        Debug.Log("BYE");
        resetPlayerParent();
    }

}
