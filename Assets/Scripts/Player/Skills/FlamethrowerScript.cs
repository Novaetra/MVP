using UnityEngine;
using System.Collections;

public class FlamethrowerScript : MonoBehaviour {

    private float currentTime, totalTimer;
    private bool doneSetup = false;

	void Start ()
    {
        currentTime = 0f;
        totalTimer = 2f;
        doneSetup = true;
	}
    
    void Update()
    {
        if (doneSetup)
        {
            if (currentTime < totalTimer)
            {
                currentTime+=Time.deltaTime;
            }
            else
            {
                StartCoroutine(destroyFire());
            }
        }
    }

    private IEnumerator destroyFire()
    {
        GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

}
