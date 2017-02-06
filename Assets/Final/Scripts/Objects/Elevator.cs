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
		//If the elevator isn't moving
		if (!isMoving) 
		{
			//If the elevator is downstairs, make it go up
			if(isDown)
			{
				GetComponent<Animation>().Play("ElevatorUp");
				isDown = false;
				isMoving = true;
			}
			//if the elevator is upstairs, make it go down
			else
			{
				GetComponent<Animation>().Play("ElevatorDown");
				isDown = true;
				isMoving = true;
			}
			player.transform.parent = transform;
		}
    }

	//Reset the player's parent so it isn't attached to elevator anymore
    private void resetPlayerParent()
    {
        player.transform.parent = null;
    }
    
	//Sets moving to false so it can be interacted with again
	private void stoppedMoving()
	{
		isMoving = false;
	}

	//Once the player leaves the elevator, remove him as a child
    private void OnTriggerExit(Collider col)
    {
        if(player!=null && col.tag == "Player")
        {
            resetPlayerParent();
        }
    }

}
