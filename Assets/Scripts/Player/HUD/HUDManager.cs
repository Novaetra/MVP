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
	private PersonControlller pc;

	private GameObject currentPlayer;
	private GameObject canvasObj;
	private GameObject panel;
	private GameObject tooltip;

    private Text revivingTxt;
    private Text upgradePnts;
	private Text shortMsg;
	private Text expText;
    private Text roundsTxt;
	private Text tooltipName;
	private Text tooltipDesc;
	private Text tooltipLvl;
	private Text currentLvlTxt;

	private float timer;
	private float currentTime;

	private bool setUpDone = false;

	//Assigns all the variables to their correspinding values
	public void postSetUp()
	{
        currentTime = 0;
        timer = 0;
		currentPlayer = gameObject;
        pc = currentPlayer.GetComponent<PersonControlller>();
		sm = currentPlayer.GetComponent<StatsManager> ();
		canvasObj = currentPlayer.GetComponentInChildren<Canvas> ().gameObject;
		panel = canvasObj.transform.FindChild ("Panel").gameObject;
        roundsTxt = canvasObj.transform.FindChild("RoundsTxt").GetComponent<Text>();
        roundsTxt.enabled = false;
		shortMsg = canvasObj.transform.FindChild ("ShortMessage").GetComponent<Text> ();
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
		updateTimer ();
	}

	private void updateTimer()
	{
		if (currentTime < timer) 
		{
			currentTime += Time.deltaTime;
		}
		else
        {
            hideMsg ();
		}
	}

	public void updateCurrentLvlTxt()
	{
		currentLvlTxt.text = "Level " + sm.getCurrentLvl ();
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
			expText.text = sm.getCurrentExp () + " / " + sm.getGoalExp ();
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
		shortMsg.text = msg;
		shortMsg.enabled = true;
		timer = dur;
		currentTime = 0f;
	}

	private void hideMsg()
	{
        try
        {
            shortMsg.enabled = false;
        }
		catch(System.NullReferenceException o)
        {
            //Debug.LogError("Error msg null");
        }
	}

    public void updateRoundsTxt(int round)
    {
        roundsTxt.enabled = true;
        roundsTxt.text = "Round " + round;
    }

	//Fills in tooltip information and adds it to screen

	public void showTooltip(string name, string desc, string lvlRequired, Transform transform)
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
