using UnityEngine;
using System.Collections;

public class Skill
{

    private string name;
    private string description;
    private float damage;
    private float cost;
    private float cooldown;
    private float currentCooldown;
    private GameObject slotAssignedTo;
    private Animator anim;
    private Skills currentEnumSkill;
    private SkillType skillType;
    private StatsManager stats;
    private int requirement;

    public Skill(string n, string d, float dmg,  float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm)
    {
        name = n;
        description = d;
        damage = dmg;
        cost = c;
        cooldown = cd;
        currentCooldown = cooldown;
        anim = PhotonGameManager.currentplayer.GetComponent<Animator>();
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
        PhotonGameManager.currentplayer.GetComponent<PhotonView>().RPC("rpcUse", PhotonTargets.All, (int)currentEnumSkill);
    }


    private void startCooldown()
    {
        currentCooldown = 0f;
    }

    public int getRequirement()
    {
        return requirement;
    }

    public float getDmg()
    {
        return damage;
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
