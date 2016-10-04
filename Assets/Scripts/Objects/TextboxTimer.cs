using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextboxTimer : MonoBehaviour {

	private float totalTimer = 0f;
	private float currentTimer  = 0f;


	void Update () 
	{
		//If the currnent time is less than timer amount, increase it by one over time
		if (currentTimer < totalTimer) {
			currentTimer += Time.deltaTime;
		} 
		//Once the timer is done, hide the msg box
		else
		{
			hideMsg ();
		}
	}
	//Setter for total timer
	public void setTimer(float dur)
	{
		totalTimer = dur;
	}

	//Hides message box and destroys this timer component
	private void hideMsg()
	{	
		Text txt = GetComponent<Text> ();
		txt.enabled = false;
		txt.text = null;
		Destroy (GetComponent<TextboxTimer> ());
	}
}
