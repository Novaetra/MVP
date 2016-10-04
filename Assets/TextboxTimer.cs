using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextboxTimer : MonoBehaviour {

	private float totalTimer = 0f;
	private float currentTimer  = 0f;

	// Update is called once per frame
	void Update () 
	{
		if (currentTimer < totalTimer) {
			currentTimer += Time.deltaTime;
		} else
		{
			hideTimer ();
		}
	}

	public void setTimer(float dur)
	{
		totalTimer = dur;
	}

	private void hideTimer()
	{	
		Text txt = GetComponent<Text> ();
		txt.enabled = false;
		txt.text = null;
		Destroy (GetComponent<TextboxTimer> ());
	}
}
