using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{

    private bool isDown = true;
	private bool isMoving = false;
    private GameObject player;

	public void interact(object[] paramtrs)
    {
        player = (GameObject)paramtrs[1];
		if (!isMoving) 
		{
			if(isDown)
			{
				GetComponent<Animation>().Play("ElevatorUp");
				isDown = false;
				isMoving = true;
			}else
			{
				GetComponent<Animation>().Play("ElevatorDown");
				isDown = true;
				isMoving = true;
			}
			player.transform.parent = transform;
		}
    }

    private void resetPlayerParent()
    {
        player.transform.parent = null;
    }
    
	private void stoppedMoving()
	{
		isMoving = false;
	}

    private void OnTriggerExit(Collider col)
    {
        resetPlayerParent();
    }

}
