﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTreePiece : MonoBehaviour 
{
	//Attributes
	[SerializeField]
	private string skillName;
	private Skill skill;
	private List<Skill> skillList;
	private Image img;
    private StatsManager sm;
	private HUDManager hudman;
    private bool isUnlocked;
    
    //Links the skill name to the actual skill in the list of all skills
	public void setSkill()
	{
        isUnlocked = false;
		skillList = PhotonView.Find (PhotonGameManager.playerID).gameObject.GetComponent<SkillManager> ().getAllSkills ();
        sm = GetComponentInParent<StatsManager>();
		hudman = GetComponentInParent<HUDManager> ();
		img = gameObject.GetComponent<Image> ();
		foreach (Skill s in skillList) 
		{
			if (s.getName ().Equals (skillName)) 
			{
				skill = s;
				return;
			}
		}
	}

	//Unlocks/Upgrades skills
	public void unlockOrUpgradeSkill()
    {
		if (!isUnlocked) 
		{
			isUnlocked = true;
			GetComponentInParent<Toggle> ().interactable = false;
			sm.addUpgradePoint (-1);
			sm.activateUnlockable ();
			GetComponentInParent<HUDManager> ().updateUpgradePoints ();
			GetComponentInParent<SkillTree> ().unlockSkill (sm);
		} 
		else 
		{
			Debug.Log ("upgrade");
		}
    }


	//This is for one-time effects (passives)
	#region passives

	public void healthUpgradeOnUnlock()
	{
		sm.setTotalHealth (sm.getTotalHealth() + skill.getEffectAmount());
	}

	public void meleeUpgradeOnUnlock()
	{
		Debug.Log (skill.getEffectAmount ());
		sm.setMeleeDamage (sm.getMeleeDamage() + skill.getEffectAmount());
	}

	public void staminaUpgradeOnUnlock()
	{
		sm.setTotalStamina (sm.getTotalStamina() + skill.getEffectAmount());
	}

	#endregion

	public void showTooltip()
	{
		hudman.showTooltip (skill.getName(),skill.getDescription(),skill.getRequirement().ToString(), transform);
	}

	public void hideTooltip()
	{
		hudman.hideTooltip ();
	}

    public bool getUnlocked()
    {
        return isUnlocked;
    }


    public void setSkill(Skill s)
	{
		skill = s;
		if (s != null) 
		{
			skillName = s.getName ();
		} 
		else 
		{
			skillName = null;
		}
	}
	public Skill getSkill()
	{ 
		return skill;
	}
}
