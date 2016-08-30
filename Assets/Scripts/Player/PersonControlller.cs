using UnityEngine;
using System.Collections;

public class PersonControlller : MonoBehaviour 
{
	public float walkSpeed = 5f;
	public float runSpeed = 8f;
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
    private float meleeDamage = 100f;
    private float interactDistance = 2f;
    private StatsManager sm;

    private Raycaster[] raycasters;

    private GameObject personReviving;

    private bool isSettingUp = true;

	private void setUp () 
	{
		cs = GetComponent<CharacterController> ();
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
                debugControls();
            }
        }


        if (Input.GetKeyUp(KeyCode.Escape))
        {
            toggleCursorLock(!cursorLocked);
        }
    }
    
    void debugControls()
    {

        if (Input.GetKeyUp(KeyCode.M))
        {
            sm.recieveExp(5);
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            sm.recieveDamage(10);
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

    private void throwRays()
    {
        RaycastHit hit;
        foreach (Raycaster caster in raycasters)
        {
            Transform raycaster = caster.transform;
            Debug.DrawRay(raycaster.transform.position, -raycaster.transform.forward * interactDistance, Color.blue);
            if (Physics.Raycast(raycaster.transform.position, -raycaster.transform.forward, out hit, interactDistance))
            {
                checkRevive(hit);
                checkDoor(hit);
            }
        }
    }
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


    private void checkDoor(RaycastHit hit)
    {
        if (hit.transform.tag == "Door")
        {
			if (sm.getCurrentExp() - hit.transform.GetComponentInParent<Door>().getCost() >= 0 && hit.transform.GetComponentInParent<Door>().getOpen() == false)
            {
                hit.transform.GetComponentInParent<PhotonView>().RPC("openDoor",PhotonTargets.AllBuffered,null);
                sm.subtractExp(hit.transform.GetComponentInParent<Door>().getCost());
            }
        }
    }

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
                        hit.transform.SendMessage("recieveDamage", meleeDamage);
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

}
