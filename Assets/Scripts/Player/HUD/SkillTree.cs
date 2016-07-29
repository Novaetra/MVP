﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour 
{
    
	public List<SkillTreePiece> treePieces;
	private SkillTreePiece piece;
    private List<Toggle> toggles;
    
	public void setUp()
	{
        toggles = new List<Toggle>();
        treePieces = new List<SkillTreePiece>();
		foreach (Transform child in transform.GetChild(0).transform) 
		{
			if (child.gameObject.GetComponentInChildren<SkillTreePiece> () != null) 
			{
				piece = child.gameObject.GetComponentInChildren<SkillTreePiece> ();
                toggles.Add(child.GetComponent<Toggle>());
                child.gameObject.GetComponent<Toggle>().interactable = false;
				treePieces.Add (piece);
			}
		}
	}
    
    public void activateUnlockable(StatsManager sm)
    {
        try
        {
            int x = 0;
            foreach (SkillTreePiece piece in treePieces)
            {
                if (piece.getUnlocked() == false && piece.getSkill().getRequirement() <= sm.getCurrentLvl() && sm.getUpgradePnts() > 0)
                {
                    toggles[x].interactable = true;
                }
                else
                {
                    toggles[x].interactable = false;
                }
                x++;
            }
        }
        catch(System.NullReferenceException e)
        {
            Debug.LogError("Couldnt link ability. Check its name");
        }
    }

}
