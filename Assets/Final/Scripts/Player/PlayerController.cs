using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{

	//Temp
	public int expIncreaseAmt;

	public float walkSpeed = 4f;
	public float runSpeed = 6f;
	public float currentSpeed;
    public Transform upperBody;
	public float lastUpperRot;
	public bool cursorLocked = true;
    private bool isReviving;
    private bool isGrounded = true;
	private CharacterController cs;
	private Animator anim;
	private float lastFullRot;
	private float XSensitivity = 2f;
	private float YSensitivity = 2f;
	private float MinimumY = 80f;
	private float MaximumY = 70f;
	private float meleeDistance = 2f;
    private float interactDistance = 2f;
    private StatsManager sm;
	private HUDManager hudman;

    private Raycaster[] raycasters;

    private GameObject personReviving;

    private bool isSettingUp = true;

	public void set_Up () 
	{
		cs = GetComponent<CharacterController> ();
		hudman = GetComponent<HUDManager> ();
		currentSpeed = walkSpeed;
		anim = GetComponent<Animator> ();
		lastUpperRot = 0f;
		lastFullRot = 0f;
		sm = GetComponent<StatsManager> ();
        isReviving = false;
        personReviving = null;
        raycasters = GetComponentsInChildren<Raycaster>();
        isSettingUp = false;
    }

	private void Update()
	{
        if(sm.getAlive() == true)
        {
            if (!isSettingUp)
            {
                checkClick();
                checkSprint();
                movement();
                updateCursorLock();
                checkInteract();
				checkTab ();
				debugControls();
				checkLvlUp ();
            }
        }
    }

	private void checkTab()
	{
		if (Input.GetButtonUp ("Tab")) 
		{
			toggleCursorLock (!cursorLocked);
			if (cursorLocked == false) 
			{
				hudman.showPanel ();
			}
			else 
			{
				
				hudman.hidePanel ();
				hudman.hideTooltip ();
			}
		}
	}
    
    void debugControls()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
			sm.recieveExp(expIncreaseAmt);
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            sm.recieveDamage(10);
        }

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			toggleCursorLock(!cursorLocked);
		}

		if (Input.GetKeyUp (KeyCode.P)) 
		{
			UnityEditor.EditorApplication.isPaused = true;
		}
		if (Input.GetKeyUp (KeyCode.O)) {
			UnityEditor.EditorApplication.isPaused = false;
		}
    }
		
	void LateUpdate ()
    {
        if (sm.getAlive() == true)
        {
            cameraRot();
        }
    }

	private void checkClick()
	{
		if (Input.GetButtonUp ("Click")&& (cursorLocked)) 
		{
            //Attack
            if (sm.getCurrentStamina() - sm.getMeleeCost() >= 0 && anim.GetInteger("Skill") != (int)Skills.BasicAttack)  
			{
				sm.useStamina (sm.getMeleeCost(),false);

				anim.SetInteger("Skill",(int)Skills.BasicAttack);
			}
		}
	}

    private void checkInteract()
    {
        //Check if E is pressed down
        if(Input.GetKey(KeyCode.E) )
        {
            throwRays();
        }
        //If its not, then set anything that relies on it to false
        else
        {
            isReviving = false;
        }
    }

	private void checkLvlUp()
	{
		if (Input.GetKeyUp (KeyCode.L) && sm.getCurrentExp() - sm.getGoalExp() >=0) 
		{
			//Level up
			sm.lvlUp(sm.getCurrentExp() - sm.getGoalExp());
		}
	}

    private void throwRays()
    {
        RaycastHit hit;
        foreach (Raycaster caster in raycasters)
        {
            Transform raycaster = caster.transform;
            Debug.DrawRay(raycaster.transform.position, -raycaster.transform.forward * interactDistance, Color.blue);
            if (Physics.Raycast(raycaster.transform.position, -raycaster.transform.forward, out hit, interactDistance))
            {
                //Has to check these two separately because each one has their own specific protocols
               // checkRevive(hit);
                checkInteract(hit);
            }
        }
    }

    private void checkInteract(RaycastHit hit)
    {
        if ((hit.transform.parent.name.Substring(0,hit.transform.parent.name.Length-1)) != "Room")
        {

            hit.transform.parent.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            hit.transform.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
        }
    }

    /*
	private void checkRevive(RaycastHit hit)
    {
        if (hit.transform.tag == "Player")
        {
            if (hit.transform.GetComponent<StatsManager>().getAlive() == false)
            {
                isReviving = true;
                personReviving = hit.transform.gameObject;
                hit.transform.GetComponent<StatsManager>().startRevive(gameObject);
            }
        }
    }
    */

    public void checkMelee()
    {
        RaycastHit hit;
        if(raycasters != null)
        {
            foreach (Raycaster caster in raycasters)
            {
                Debug.DrawRay(caster.transform.position, -caster.transform.forward * meleeDistance, Color.red, 1);
                if (Physics.Raycast(caster.transform.position, -caster.transform.forward, out hit, meleeDistance))
                {
                    if (hit.transform.tag == "Enemy")
                    {
						sm.dealMeleeDamage (hit);
                        return;
                    }
                }
            }
        }
       
    }

	private void checkSprint()
	{
		if (Input.GetButton ("Sprint"))
		{
			//Sprint
			if (sm.getCurrentStamina() > 0) 
			{
				currentSpeed = runSpeed;
				sm.useStamina ((sm.getSprintStamCost() /* - (sm.sprintStaminaCost  (sm.dexterity / 100))*/),true);
			} 
			else 
			{
				currentSpeed = walkSpeed;
			}
		}

        if(Input.GetButtonUp("Sprint"))
        {
            currentSpeed = walkSpeed;
        }
	}

	private void movement()
	{

		float speed = Input.GetAxis ("Vertical") * currentSpeed;
		float direction = Input.GetAxis ("Horizontal") * currentSpeed;
        anim.SetFloat("Speed", speed);
        anim.SetFloat("Direction", direction);
        Vector3 finalMove = new Vector3 (direction, 0f, speed);
		finalMove = transform.rotation * finalMove;
		cs.SimpleMove (finalMove);
	}

	private void cameraRot()
	{
		if (cursorLocked == true) 
		{
			float horizontal = Input.GetAxis ("Mouse X") * XSensitivity;
			float vertical = Input.GetAxis ("Mouse Y") * YSensitivity;
            
			if ((lastUpperRot - vertical < MinimumY && lastUpperRot - vertical > -MaximumY)) 
			{
				upperBody.transform.Rotate ((lastUpperRot - vertical),0f, 0f);
				lastUpperRot = lastUpperRot - vertical;
                
            }
			else
            {
                upperBody.transform.Rotate((lastUpperRot), 0f, 0f);
            }
            transform.Rotate(0f, horizontal, 0f);
        }
		else
        {
            upperBody.transform.Rotate((lastUpperRot), 0f, 0f);
        }
	}

	public void toggleCursorLock(bool val)
	{
		cursorLocked = val;
        updateCursorLock();
	}

	private void updateCursorLock()
	{
		if (cursorLocked == true) 
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;	
		} 
		else 
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

    public bool getReviving()
    {
        return isReviving;
    }
    
    public GameObject getPersonReviving()
    {
        return personReviving;
    }

    public void setPersonReviving(GameObject g)
    {
        personReviving = g;
    }

	public void resetAnimator()
	{
		anim.SetInteger ("Skill", -1);
	}
}
