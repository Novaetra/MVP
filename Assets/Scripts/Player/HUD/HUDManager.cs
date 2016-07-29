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
	private PersonControlller controller;
	private GameObject canvasObj;
	private GameObject panel;
    private Text revivingTxt;
    private Text upgradePnts;
	private Text shortMsg;
    private Text roundsTxt;
	private float timer;
	private float currentTime;

	private bool setUpDone = false;

	public void postSetUp()
	{
        currentTime = 0;
        timer = 0;
		currentPlayer = gameObject;
        pc = currentPlayer.GetComponent<PersonControlller>();
		sm = currentPlayer.GetComponent<StatsManager> ();
		controller = currentPlayer.GetComponent<PersonControlller> ();
		canvasObj = currentPlayer.GetComponentInChildren<Canvas> ().gameObject;
		panel = canvasObj.transform.FindChild ("Panel").gameObject;
        roundsTxt = canvasObj.transform.FindChild("RoundsTxt").GetComponent<Text>();
        roundsTxt.enabled = false;
		shortMsg = canvasObj.transform.FindChild ("ShortMessage").GetComponent<Text> ();
        upgradePnts = panel.transform.FindChild("UpgradePoints").GetComponent<Text>();
        revivingBarBG = canvasObj.transform.FindChild("ReviveBarBG").GetComponent<Image>();
        revivingTxt = revivingBarBG.transform.FindChild("Text").GetComponent<Text>();
        revivingBar = revivingBarBG.transform.FindChild("Image").GetComponent<Image>();
        panel.SetActive(false);
        upgradePnts.enabled = false;
		setUpDone = true;
	}

	private void Update()
	{
		checkTab ();
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

	private void checkTab()
	{
		if (Input.GetButtonUp ("Tab") && setUpDone == true) 
		{
			controller.toggleCursorLock (!controller.cursorLocked);
			if (controller.cursorLocked == false) 
			{
				panel.SetActive (true);
			}
			else 
			{
				panel.SetActive (false);
			}
		}
	}

	private void updateBars()
	{
		if (sm != null) 
		{
			healthbar.fillAmount = sm.getCurrentHealth () / sm.getTotalHealth ();
			manabar.fillAmount = sm.getCurrentMana () / sm.getTotalMana ();
			staminabar.fillAmount = sm.getCurrentStamina () / sm.getTotalStamina ();
			expbar.fillAmount = sm.getCurrentExp () / sm.getGoalExp ();

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
		shortMsg.enabled = false;
	}

    public void updateRoundsTxt(int round)
    {
        roundsTxt.enabled = true;
        roundsTxt.text = "Round " + round;
    }

}
