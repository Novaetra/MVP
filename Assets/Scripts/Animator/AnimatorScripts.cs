using UnityEngine;
using System.Collections;

public class AnimatorScripts : MonoBehaviour
{
    private Animator anim;
    private bool isRaising;
    private bool isLowering;
    
    [SerializeField]
    private float lowerRaiseAmnt;
    private float startY;
    private float endY;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        isRaising = false;
        isLowering = false;
        lowerRaiseAmnt = 2f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        updateColliderHeight();
        if(isRaising == true && startY-transform.position.y < lowerRaiseAmnt)
        {
            transform.position += new Vector3(0f, lowerRaiseAmnt) * Time.deltaTime;
        }

        if(isLowering == true && startY - transform.position.y < lowerRaiseAmnt)
        {
            transform.position += new Vector3(0f, -lowerRaiseAmnt) * Time.deltaTime;
        }
        
        if(startY - transform.position.y >= lowerRaiseAmnt)
        {
            isLowering = false;
            isRaising = false;
        }
	}
    void updateColliderHeight()
    {
        if (GetComponent<CharacterController>() != null)
        {
            GetComponent<CharacterController>().height = anim.GetFloat("colliderHeight");
        }
    }

    public void lowerDead()
    { 
        startY = transform.position.y;
        endY = startY - lowerRaiseAmnt;
        isLowering = true;
    }

    public void raiseDead()
    {
        startY = transform.position.y;
        endY = startY - lowerRaiseAmnt;
        isRaising = true;
    }
}
