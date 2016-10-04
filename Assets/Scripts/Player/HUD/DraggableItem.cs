using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private bool isDraggable = true;

	public static Skill skillBeingDragged;
	public static Sprite imgBeingDragged;

	private static float planeDistance = .11f;
	private static Transform originalParent;
	private static Vector3 startPos;

	private static GameObject currentPlayer;

	public void setUp()
	{
		currentPlayer = PhotonView.Find (PhotonGameManager.playerID).gameObject;
	}

	public void OnBeginDrag(PointerEventData data)
	{
		if (isDraggable) 
		{
			
			//Sets the object being dragged
			skillBeingDragged = GetComponent<SkillTreePiece> ().getSkill ();
			//Sets the image of the object being dragged
			imgBeingDragged = GetComponent<Image> ().sprite;
			//Sets the start position so it can go back to it later
			startPos = transform.localPosition;
			//Sets the original parent so it can go back to it later
			originalParent = gameObject.transform.parent.transform;
			//Changes the parent to Canvas so it can be dragged outside the skill tree mask

			//Temporary
			gameObject.transform.SetParent (currentPlayer.GetComponentInChildren<Canvas> ().transform);
			gameObject.transform.SetAsLastSibling ();
			//Allows the event system to pass through the object being dragged
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
		}
	}

	public void OnDrag(PointerEventData data)
	{
		if (isDraggable)
		{
			//Update position of object

			transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (data.position.x, data.position.y, planeDistance));
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (isDraggable) 
		{
			//Reset raycast block
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			//Reset skill being dragged
			skillBeingDragged = null;
			//Reset img being dragged
			imgBeingDragged = null;
			//Reset parent to its original
			gameObject.transform.SetParent (originalParent);
			//Reset object to start position
			transform.localPosition = startPos;
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		GetComponent<SkillTreePiece> ().showTooltip ();
	}

	public void OnPointerExit(PointerEventData data)
	{
		GetComponent<SkillTreePiece> ().hideTooltip ();
	}
}
