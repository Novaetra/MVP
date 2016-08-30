using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour
{
    //Attributes
    private float totalHealth;
    private float currentHealth;
    private float totalMana;
    private float currentMana;
    private float totalStamina;
    private float currentStamina;
    private float sprintStamCost;
    private float meleeCost;
    private int currentLevel;
	[SerializeField]
    private float currentExp;
	private float currentPoints;
	[SerializeField]
    private float totalExpRequiredToLvlUp;
    private int upgradePoints;
    private bool isAlive;
    private bool isReviving;
    private float reviveDistance;
    //Multipliers
    private float healthRegen;
    private float manaRegen;
    private float staminaRegen;

    //Timers
    private float stamTimer;
    private float stamCurrentTime;
    private float manaTimer;
    private float manaCurrentTime;

    private float reviveTimer;
    private float currentReviveTimer;

    private HUDManager hudman;
    private Animator anim;
    private GameObject currentReviver;
    private PhotonGameManager pgm;
    private SkillTree[] trees;

    void Start()
    {
        isAlive = true;
        isReviving = false;
        totalHealth = 100f;
        currentHealth = 100f;
        totalMana = 100f;
        currentMana = 100f;
        totalStamina = 100f;
        currentStamina = 100f;
        sprintStamCost = 20f;
        meleeCost = 5f;
        currentLevel = 1;
        currentExp = 0f;
        totalExpRequiredToLvlUp = 40f;
        currentPoints = 0f;
        upgradePoints = 0;
        reviveDistance = 3.5f;

        healthRegen = 0f;
        manaRegen = 2.5f;
        staminaRegen = 50f;

        stamTimer = 2f;
        stamCurrentTime = stamTimer;
        manaTimer = 2f;
        manaCurrentTime = manaTimer;

        reviveTimer = 2f;
        currentReviveTimer = reviveTimer;

        hudman = GetComponent<HUDManager>();
        anim = GetComponent<Animator>();
        pgm = GameObject.Find("Network").GetComponent<PhotonGameManager>();

    }

    void Update()
    {
        updateAttributes();
        if (isReviving == true)
        {
            checkRevive();
        }
    }
    //HAVE TO SET TREES WHEN THE PANEL IS ACTIVE, OTHERWISE IT RETURNS NULL
    void setUp()
    {
        trees = GetComponentsInChildren<SkillTree>();
    }

    private void updateAttributes()
    {
        if (currentHealth < totalHealth)
        {
            currentHealth += healthRegen * Time.deltaTime;
        }
        if (manaCurrentTime < manaTimer)
        {
            manaCurrentTime += 1 * Time.deltaTime;
        }
        else
        {
            if (currentMana < totalMana)
            {
                currentMana += manaRegen * Time.deltaTime;
            }
        }
        if (stamCurrentTime < stamTimer)
        {
            stamCurrentTime += 1 * Time.deltaTime;
        }
        else
        {
            if (currentStamina < totalStamina) {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }
    }

    public void recieveExp(int exp)
    {
        currentExp += exp;
        if (currentExp >= totalExpRequiredToLvlUp)
        {
            levelUp(currentExp - totalExpRequiredToLvlUp);
        }
    }

    private void levelUp(float leftOver)
    {
        currentExp = leftOver;
        totalExpRequiredToLvlUp *= 1.5f;
        currentLevel++;
        upgradePoints++;
        activateUnlockable();
        StartCoroutine(lvlUpTxt());
        if (currentExp >= totalExpRequiredToLvlUp)
        {
            levelUp(currentExp - totalExpRequiredToLvlUp);
        }
    }

    public void recieveDamage(float dmg)
    {
        currentHealth -= dmg;
        hudman.updateBars();
        checkDeath();
    }

    private void checkDeath()
    {
        if (currentHealth <= 0f)
        {
            //Death
            GetComponent<PhotonView>().RPC("death", PhotonTargets.AllBuffered, null);
        }
    }

    public void startRevive(GameObject reviver)
    {
        //So it doesn't get called more than once
           if(isReviving==false)
           {
            currentReviver = reviver;
            currentReviveTimer = 0f;
            isReviving = true;
        }
    }

    public void checkRevive()
    {
        if (isReviving == true)
        {
            float distance = Vector3.Distance(transform.position, currentReviver.transform.position);
            if (distance <= reviveDistance && currentReviver.GetComponent<PersonControlller>().getReviving() == true && isAlive == false)
            {
                if (currentReviveTimer < reviveTimer)
                {
                    currentReviveTimer += 2 * Time.deltaTime;
                }
                else if (currentReviveTimer >= reviveTimer)
                {
                    GetComponent<PhotonView>().RPC("revive", PhotonTargets.AllBuffered, null);
                }
            }
            else
            {
                isReviving = false;
                currentReviveTimer = 0;
                currentReviver.GetComponent<PersonControlller>().setPersonReviving(null);
            }
        }
    }

    [PunRPC]
    void death()
    {
        isAlive = false;
        pgm.checkGameEnded();
        anim.SetBool("isAlive",isAlive);
    }

    [PunRPC]
    void revive()
    {
        isAlive = true;
        anim.SetBool("isAlive", isAlive);
        currentHealth = totalHealth;
    }

    public void useMana(float mana)
    {
        currentMana -= mana;
        manaCurrentTime = 0f;
    }

    //Bool is if its over time or not
    public void useStamina(float stam, bool tf)
    {
        if (tf)
        {
            currentStamina -= stam * Time.deltaTime;
        }
        else
        {
            currentStamina -= stam;
        }

        stamCurrentTime = 0f;
    }

    public void activateUnlockable()
    {
        hudman.updateUpgradePoints();
        foreach (SkillTree tree in trees)
        {
            tree.activateUnlockable(this);
        }
    }

    #region getters
    public bool getReviving()
    {
        return isReviving;
    }

    public float getCurrentReviveTimer()
    {
        return currentReviveTimer;
    }

    public float getTotalReviveTimer()
    {
        return reviveTimer;
    }

    public bool getAlive()
    {
        return isAlive;
    }

    public float getMeleeCost()
	{
		return meleeCost;
	}

	public float getTotalHealth()
	{
		return totalHealth;
	}

	public float getCurrentHealth()
	{
		return currentHealth;
	}

    public void setCurrentHealth(float h)
    {
        currentHealth = h;
    }

	public float getTotalMana()
	{
		return totalMana;
	}

	public float getCurrentMana()
	{
		return currentMana;
	}

	public float getTotalStamina()
	{
		return totalStamina;
	}

    public void setCurrentStamina(float s)
    {
        currentStamina = s;
    }

	public float getCurrentStamina()
	{
		return currentStamina;
	}

	public float getCurrentExp()
	{
		return currentExp;
	}

    public int getCurrentLvl()
    {
        return currentLevel;
    }

    public int getUpgradePnts()
    {
        return upgradePoints;
    }

    public void addUpgradePoint(int num)
    {
        upgradePoints += num;
    }

    public float getGoalExp()
	{
		return totalExpRequiredToLvlUp;
	}

    public float getCurrentPoints()
    {
        return currentPoints;
    }

    public void addCurrentPoints(float pnts)
    {
        currentPoints += pnts;
    }

    public void subtractExp(float pnts)
    {
		currentExp -= pnts;
        //hudman.updatePntsTxt();
    }

	public float getSprintStamCost()
	{
		return sprintStamCost;
	}
	#endregion

	private IEnumerator lvlUpTxt()
	{
        hudman.displayMsg("You have reached level " + currentLevel, 2f);
        yield return new WaitForSeconds(2);
        hudman.displayMsg("You can now unlock a new skill!",2f);
	}
}
