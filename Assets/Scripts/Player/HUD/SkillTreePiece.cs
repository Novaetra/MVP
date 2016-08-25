using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillTreePiece : MonoBehaviour 
{
	//Attributes
	[SerializeField]
	private string skillName;
	private Skill skill;
	private List<Skill> skillList;
	private Image img;
    private StatsManager sm;
    private bool isUnlocked;
    
    //Links the skill name to the actual skill in the list of all skills
	public void setSkill()
	{
        isUnlocked = false;
		skillList = PhotonView.Find (PhotonGameManager.playerID).gameObject.GetComponent<SkillManager> ().getAllSkills ();
        sm = GetComponentInParent<StatsManager>();
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

	public void unlockSkill()
    {
        isUnlocked = true;
        GetComponentInParent<Toggle>().interactable = false;
        sm.addUpgradePoint(-1);
        sm.activateUnlockable();
        GetComponentInParent<HUDManager>().updateUpgradePoints();
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
