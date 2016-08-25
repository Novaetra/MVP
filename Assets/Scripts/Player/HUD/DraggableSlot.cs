using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{

	public static Skill skillBeingDragged;
	public static Sprite imgBeingDragged;
	public static Transform originalParent;
	public static Transform originalSlot;
	public bool isDraggable;
	public static bool foundTarget;


	private Vector3 startPos;
	private Transform background;
	private static GameObject currentPlayer;


	public void setUp()
	{
		currentPlayer = PhotonView.Find (PhotonGameManager.playerID).gameObject;
	}

	public void OnBeginDrag(PointerEventData data)
	{
		if (isDraggable) 
		{
			//Check if it has a skill
			if (GetComponent<SkillTreePiece> ().getSkill () != null) 
			{
				foundTarget = false;
				startPos = transform.localPosition;
				//Sets the object being dragged
				skillBeingDragged = transform.GetComponent<SkillTreePiece> ().getSkill ();
				//Sets the image of the object being dragged
				imgBeingDragged = transform.GetComponent<Image> ().sprite;
				//Sets the background image of the object being dragged
				background = transform.parent;
				background = background.FindChild ("SlotBG").transform;
				//Sets the original parent so it can go back to it later
				originalParent = gameObject.transform.parent.transform;
				originalSlot = originalParent.GetChild (1);
				//Changes the parent to Canvas so it can be dragged outside the skill tree mask
				gameObject.transform.SetParent (currentPlayer.GetComponentInChildren<Canvas>().transform);
				gameObject.transform.SetAsLastSibling ();
				//Allows the event system to pass through the object being dragged
				GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}
	}

	public void OnDrag(PointerEventData data)
	{
		//Update position of object
		if (isDraggable) 
		{
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x,data.position.y,0.5f));
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (isDraggable)
		{
			//Reset raycast block
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			//Reset parent to its original
			gameObject.transform.SetParent (originalParent);
			//Reset object to start position
			transform.localPosition = startPos;

			//if it doesn't find a target, then clear the slot
			if(foundTarget == false)
			{
				GetComponent<SkillTreePiece> ().setSkill (null);
				background.GetComponent<Image> ().sprite = originalParent.GetComponent<Image> ().sprite;
				GetComponent<Image> ().sprite = originalParent.GetComponent<Image> ().sprite;
			}

			//Reset all attributes to null
			skillBeingDragged = null;
			imgBeingDragged = null;
			originalSlot = null;
			originalParent = null;
		}
	}

}
