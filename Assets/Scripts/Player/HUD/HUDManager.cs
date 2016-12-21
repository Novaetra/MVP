using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour 
{

	public Image healthbar;
	public Image manabar;
	public Image staminabar;
	public Image expbar;
    public Image revivingBar;
    public Image revivingBarBG;

	private StatsManager sm;
	private PlayerController pc;

	private GameObject currentPlayer;
	private GameObject canvasObj;
	private GameObject panel;
	private GameObject tooltip;
	private GameObject containerForMessageBoxes;

    private Text revivingTxt;
    private Text upgradePnts;
	private Text expText;
    private Text roundsTxt;
	private Text tooltipName;
	private Text tooltipDesc;
	private Text tooltipLvl;
	private Text currentLvlTxt;

	private bool setUpDone = false;

	//Assigns all the variables to their correspinding values
	public void postSetUp()
	{
		currentPlayer = gameObject;
        pc = currentPlayer.GetComponent<PlayerController>();
		sm = currentPlayer.GetComponent<StatsManager> ();
        canvasObj = GameObject.Find("Canvas");
		panel = canvasObj.transform.FindChild ("Panel").gameObject;
        roundsTxt = canvasObj.transform.FindChild("RoundsTxt").GetComponent<Text>();
        roundsTxt.enabled = false;
		containerForMessageBoxes = canvasObj.transform.FindChild ("Message Boxes").gameObject;
        upgradePnts = panel.transform.FindChild("UpgradePoints").GetComponent<Text>();
		currentLvlTxt = panel.transform.FindChild ("CurrentLevelText").GetComponent<Text> ();
        revivingBarBG = canvasObj.transform.FindChild("ReviveBarBG").GetComponent<Image>();
        revivingTxt = revivingBarBG.transform.FindChild("Text").GetComponent<Text>();
        revivingBar = revivingBarBG.transform.FindChild("Image").GetComponent<Image>();
		tooltip = canvasObj.transform.FindChild ("Tooltip").gameObject;
		tooltipName = tooltip.transform.FindChild ("Name").GetComponent<Text> ();;
		tooltipDesc = tooltip.transform.FindChild ("Description").GetComponent<Text> ();
		tooltipLvl = tooltip.transform.FindChild ("LvlRequired").GetComponent<Text> ();
		expText = canvasObj.transform.FindChild ("Exp Counter").GetComponent<Text> ();
		tooltip.SetActive (false);
        panel.SetActive(false);
        upgradePnts.enabled = false;
		setUpDone = true;
	}
	//
	private void Update()
	{
		updateBars ();
	}

	public void updateCurrentLvlTxt()
	{
        if(currentLvlTxt != null && sm.getCurrentLvl() != null)
        {
            currentLvlTxt.text = "Level " + sm.getCurrentLvl();
        }
	}


	public GameObject getPanel()
	{
		return panel;
	}

	public void updateBars()
	{
		if (sm != null) 
		{
			healthbar.fillAmount = sm.getCurrentHealth () / sm.getTotalHealth ();
			manabar.fillAmount = sm.getCurrentMana () / sm.getTotalMana ();
			staminabar.fillAmount = sm.getCurrentStamina () / sm.getTotalStamina ();
			expbar.fillAmount = sm.getCurrentExp () / sm.getGoalExp ();
			expText.text = (int)sm.getCurrentExp () + " / " + (int)sm.getGoalExp ();
            /*
            if(pc.getReviving() == true)
            {
                StatsManager personReviving = pc.getPersonReviving().GetComponent<StatsManager>();
                if (personReviving.getCurrentReviveTimer() / personReviving.getTotalReviveTimer() < 1)
                {
                    revivingBarBG.enabled = true;
                    revivingTxt.enabled = true;
                    revivingBar.enabled = true;
                    revivingBar.fillAmount = personReviving.getCurrentReviveTimer() / personReviving.getTotalReviveTimer();
                }
                else
                {

                    revivingBarBG.enabled = false;
                    revivingTxt.enabled = false;
                    revivingBar.enabled = false;
                }
               
            }
            else
            {
                revivingBarBG.enabled = false;
                revivingTxt.enabled = false;
                revivingBar.enabled = false;
            }
            */

		}
	}

    public void updateUpgradePoints()
    {
        if(sm.getUpgradePnts() > 0)
        {
            upgradePnts.text = "Upgrade points: " + sm.getUpgradePnts();
            upgradePnts.enabled = true;
        }
        else
        {
            upgradePnts.enabled = false;
        }
    }

	public void displayMsg(string msg, float dur)
	{
		bool alreadyBeingDisplayed = false;
		//Go through each textbox
		foreach (Text textbox in containerForMessageBoxes.GetComponentsInChildren<Text>())
		{
			//Check if a textbox is already displaying your message
			if (textbox.text.Equals (msg)) {
				alreadyBeingDisplayed = true;
			}
		}

		//Go through each textbox
		foreach (Text textbox in containerForMessageBoxes.GetComponentsInChildren<Text>())
		{
			//If the text is null, it means it's not being used. 
			if (textbox.text == null || textbox.text.Equals("") && !alreadyBeingDisplayed) 
			{
				//Since it's not being used, use it here
				textbox.text = msg;
				textbox.enabled = true;
				TextboxTimer txtboxTimer = textbox.gameObject.AddComponent <TextboxTimer>() as TextboxTimer;
				txtboxTimer.setTimer (dur);
				break;
			}
		}

		//If no box is open, queue it to be displayed next

	}

    public void updateRoundsTxt(int round)
    {
        roundsTxt.enabled = true;
        roundsTxt.text = "Round " + round;
    }

	//Fills in tooltip information and adds it to screen

	public void showTooltip(string name, string desc, string lvlRequired)
	{
		tooltipName.text = name;
		tooltipDesc.text = desc;
		tooltipLvl.text = "Level: " + lvlRequired;
		tooltip.SetActive(true);

	}

	public void hideTooltip()
	{
		//Hides tooltip 
		tooltip.SetActive(false);
	}

	public void showPanel()
	{
		panel.SetActive (true);
	}

	public void hidePanel()
	{
		panel.SetActive (false);
	}
}
