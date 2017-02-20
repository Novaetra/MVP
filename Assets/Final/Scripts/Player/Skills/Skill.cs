using UnityEngine;
using System.Collections;

public class Skill
{

    private string name;
    private string description;
	private float effectAmount;
    private float cost;
    private float cooldown;
    private float currentCooldown;
    private GameObject slotAssignedTo;
    private Animator anim;
    private Skills currentEnumSkill;
    private SkillType skillType;
    private StatsManager stats;
    private int requirement;

    public Skill(string n, string d, float effectAmnt,  float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm)
    {
        name = n;
        description = d;
        effectAmount = effectAmnt;
        cost = c;
        cooldown = cd;
        currentCooldown = cooldown;
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();//GameObject.Find("Player").GetComponent<Animator>();
        currentEnumSkill = enumSkill;
        stats = sm;
        requirement = req;
        skillType = st;
    }

    public void use()
    {
        if (skillType == SkillType.Magic)
        {
            stats.useMana(cost);
        }
        else if (skillType == SkillType.Physical)
        {
            stats.useStamina(cost, false);
        }
        startCooldown();

        anim.SetInteger("Skill", (int)currentEnumSkill);
        // PhotonGameManager.currentplayer.GetComponent<PhotonView>().RPC("rpcUse", PhotonTargets.AllBuffered, (int)currentEnumSkill);
    }


	public void upgradeSkill()
	{
		effectAmount += effectAmount;
	}


    private void startCooldown()
    {
        currentCooldown = 0f;
    }

    public int getRequirement()
    {
        return requirement;
    }

    public float getEffectAmount()
    {
        return effectAmount;
    }

	public string getName()
	{
		return name;
	}

	public string getDescription()
	{
		return description;
	}

	public float getCooldown()
	{
		return cooldown;
	}

    public float getCost()
    {
        return cost;
    }

    public SkillType getType()
    {
        return skillType;
    }


    public void setCurrentCooldown(float cd)
	{
		currentCooldown = cd;
	}

	public float getCurrentCooldown()
	{
		return currentCooldown;
	}

	public void assignSlot(GameObject slot)
	{
		slotAssignedTo = slot;
	}

	public GameObject getSlot()
	{
		return slotAssignedTo;
	}

}
