
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum Skills
{
	Empty = -1, BasicAttack = 0, Fireball = 2, Heal = 3, Flamethrower=4
};

public enum SkillType
{
    Empty,Physical, Magic
};


public class SkillManager : MonoBehaviour
{

    private List<Skill> allSkills;
    public List<Skill> knownSkills;
    private GameObject skillBar;
    private StatsManager sm;
    private Animator anim;
    private HUDManager hudman;

    private KeyCode[] keyCodes =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6
    };

    void Start()
    {
        skillBar = GameObject.Find("SkillBar").gameObject;
        hudman = GetComponent<HUDManager>();
        allSkills = new List<Skill>();
        knownSkills = new List<Skill>();
        sm = GetComponent<StatsManager>();
        anim = GetComponent<Animator>();
        setUpList();
    }

    void Update()
    {
        checkKeys();
        updateCooldowns();
    }

    private void checkKeys()
    {
        //Note: basic attacks are not included. Those are handled in the person controller
        for (int x = 0; x < keyCodes.Length; x++)
        {
            GameObject slot = GameObject.Find("Slot (" + (x + 1) + ")");

            if (Input.GetKeyUp(keyCodes[x]))
            {
                if (slot.transform.FindChild("SlotContent").GetComponent<SkillTreePiece>().getSkill() != null)
                {
                    Skill skill = slot.transform.FindChild("SlotContent").GetComponent<SkillTreePiece>().getSkill();
                    if (skill != null && skill.getCurrentCooldown() >= skill.getCooldown())
                    {
                        if(sm.getCurrentMana() - skill.getCost() >= 0 && anim.GetInteger("Skill") == -1)
                        {
                            skill.use();
                        }
                        else
                        {
                            string msg = "";

                            if(skill.getType() == SkillType.Magic)
                            {
                                msg = "mana";
                            }
                            else
                            {
                                msg = "stamina";
                            }

                            hudman.displayMsg("Not enough " + msg ,1f);
                        }
                    }
                }
            }
        }
    }

    private void updateCooldowns()
    {
        //Update cooldowns by incremementing the current cooldown and updating the fill amount of image
        if(knownSkills != null)
        {
            foreach (Skill s in knownSkills)
            {
                if (s.getCurrentCooldown() < s.getCooldown())
                {
                    s.setCurrentCooldown(s.getCurrentCooldown() + 1 * Time.deltaTime);
                    s.getSlot().GetComponent<Image>().fillAmount = (s.getCurrentCooldown() / s.getCooldown());
                }
            }
        }
    }

    private void setUpList()
    {
        //(string name, string description, float effect amount, float cost, float cd, Skills enumSkill, SkillType type int requirement, int numUpgrades, StatsManager sm)
        allSkills.Add(new Skill("Fireball", "Hurls a flaming ball of fire forward", 100f, 25f, 4f, Skills.Fireball, SkillType.Magic, 2, 3,sm));
        allSkills.Add(new Skill("Heal", "Heals self", 50f, 25f, 5f, Skills.Heal, SkillType.Magic, 2, 3,sm));
        allSkills.Add(new Skill("Flamethrower", "Throws flames ._.", 10f, 35f, 10f, Skills.Flamethrower, SkillType.Magic, 4, 3,sm));
        allSkills.Add(new Skill("", "Empty", 0f, 0f, 0f, Skills.Empty, SkillType.Empty, 0, 0,sm));
        //Links all the skill tree pieces to the actual skill 
        gameObject.BroadcastMessage("setSkill");
        StartCoroutine(wait(.5f));
        gameObject.BroadcastMessage("postSetUp");
    }

    private IEnumerator wait(float secs)
    {
        yield return new WaitForSeconds(secs);
    }

    public List<Skill> getAllSkills()
    {
        return allSkills;
    }

    public void addToKnown(Skill sk)
    {
        knownSkills.Add(sk);
    }

    public void removeFromKnown(Skill sk)
    {
        knownSkills.Remove(sk);
    }

    public List<Skill> getKnownSkills()
    {
        return knownSkills;
    }

    /*
    [PunRPC]
    void rpcUse(int currentEnumSkill)
    {
        anim.SetInteger("Skill", (int)currentEnumSkill);
    }
    */
}
