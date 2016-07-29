using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
	private Transform skillBar;
	private Transform bg;
	private SkillManager manager;
	private DraggableSlot dragSlot;
	private GameObject currentPlayer;

	public void setUp()
	{
		currentPlayer = PhotonView.Find (PhotonGameManager.playerID).gameObject;
		//Gets the skill bar for iterating through it later
		skillBar = currentPlayer.transform.GetComponentInChildren<Canvas>().transform.Find("SkillBar").transform;
		//Get the skill manager
		manager = currentPlayer.GetComponent<SkillManager>();
		//Gets the background object so that we can change it to gray or white
		bg = transform.parent;
		bg = bg.FindChild ("SlotBG");
		bg.GetComponent<Image> ().color = Color.gray;
		//Make the slot undraggable if empty
		dragSlot = GetComponent<DraggableSlot> ();
		dragSlot.isDraggable = false;
	}
		
	public void OnDrop(PointerEventData data)
	{
		//Check to see if the skill is placed somewhere else before assigning it to the slot
		scanSkillBar ();
	}
	private void scanSkillBar()
	{
		if (DraggableItem.skillBeingDragged != null) 
		{
			//For each slot in the bar
			foreach (Transform child in skillBar) 
			{
				Transform content = child.GetChild (1);
				//If the skill being dragged from the tree is already assigned somewhere else, then clear
				//The other instance of it and assign the new one 
				if (content.GetComponent<SkillTreePiece> ().getSkill () != null) 
				{
					if (content.GetComponent<SkillTreePiece> ().getSkill () == DraggableItem.skillBeingDragged) 
					{
						bg.GetComponent<Image> ().sprite = null;
						bg.GetComponent<Image> ().color = Color.white;
						content.GetComponent<DraggableSlot> ().isDraggable = false;
						content.GetComponent<SkillTreePiece> ().getSkill ().assignSlot (null);
						content.GetComponent<SkillTreePiece> ().setSkill (null);
						content.GetComponent<Image> ().sprite = child.GetComponent<Image> ().sprite;
						manager.removeFromKnown (content.GetComponent<SkillTreePiece> ().getSkill ());
						assignSkillToSlot ();
						return;
					}
				}
			}
		}
		assignSkillToSlot ();
	}
	private void assignSkillToSlot()
	{
		//This just checks to see if the thing being dragged is from the skill tree or another slot
		if (DraggableItem.skillBeingDragged != null) 
		{
			//Assigns the image and skill to the slot
			GetComponent<SkillTreePiece> ().setSkill (DraggableItem.skillBeingDragged);
			GetComponent<Image> ().sprite = DraggableItem.imgBeingDragged;
			bg.GetComponent<Image>().sprite = DraggableItem.imgBeingDragged;
		} 
		//If its being dragged from another slot
		else 
		{
			//Swap the two skills
			Skill dragged = DraggableSlot.skillBeingDragged;
			Sprite img = DraggableSlot.imgBeingDragged;
				DraggableSlot.originalSlot.GetComponent<SkillTreePiece> ().setSkill (GetComponent<SkillTreePiece> ().getSkill ());
				DraggableSlot.originalSlot.GetComponent<SkillTreePiece> ().getSkill ().assignSlot (gameObject);
				DraggableSlot.originalSlot.GetComponent<Image> ().sprite = GetComponent<Image> ().sprite;
				DraggableSlot.originalParent.GetChild (0).GetComponent<Image> ().sprite = GetComponent<Image> ().sprite;
				GetComponent<SkillTreePiece> ().setSkill (dragged);
				GetComponent<SkillTreePiece> ().getSkill ().assignSlot (DraggableSlot.originalSlot.gameObject);
				GetComponent<Image> ().sprite = img;
				bg.GetComponent<Image> ().sprite = img;
				DraggableSlot.foundTarget = true;
		}

		//Adds the skill to the list of known skills
		manager.addToKnown (GetComponent<SkillTreePiece> ().getSkill ());
		//Assign the skill to the slot
		GetComponent<SkillTreePiece> ().getSkill ().assignSlot (gameObject);
		//Makes the slot draggable
		dragSlot.isDraggable = true;
		//Changes background color to gray
		bg.GetComponent<Image> ().color = Color.gray;
	}
}
