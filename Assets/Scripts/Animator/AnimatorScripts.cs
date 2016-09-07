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

	//Sets the variables
	void Start ()
    {
        anim = GetComponent<Animator>();
        isRaising = false;
        isLowering = false;
        lowerRaiseAmnt = 2f;
	}

	void Update ()
    {
        updateColliderHeight();
		//If character was just revived, the isRaising bool would equal true, which would then cause the character to go up a little
        if(isRaising == true && startY-transform.position.y < lowerRaiseAmnt)
        {
            transform.position += new Vector3(0f, lowerRaiseAmnt) * Time.deltaTime;
        }
		//If character just died, the isRaising bool would equal false, which would then cause the character to go down a little to prevent hovering above the ground
        if(isLowering == true && startY - transform.position.y < lowerRaiseAmnt)
        {
            transform.position += new Vector3(0f, -lowerRaiseAmnt) * Time.deltaTime;
        }
        //This will make sure you don't raise/lower too much
        if(startY - transform.position.y >= lowerRaiseAmnt)
        {
            isLowering = false;
            isRaising = false;
        }
	}
	//This updates the collider height to match the character's laying down position
    void updateColliderHeight()
    {
        if (GetComponent<CharacterController>() != null)
        {
            GetComponent<CharacterController>().height = anim.GetFloat("colliderHeight");
        }
    }
	//Sets up variables so the character is lowered
    public void lowerDead()
    { 
        startY = transform.position.y;
        endY = startY - lowerRaiseAmnt;
        isLowering = true;
    }
	//Sets up variables so the character is raised
    public void raiseDead()
    {
        startY = transform.position.y;
        endY = startY - lowerRaiseAmnt;
        isRaising = true;
    }
}
