using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnTimer : MonoBehaviour {

    public float time;

    void Start()
    {
        StartCoroutine(waitToKill());
    }

    private IEnumerator waitToKill()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
