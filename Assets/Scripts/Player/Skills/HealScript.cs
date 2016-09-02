using UnityEngine;
using System.Collections;

public class HealScript : MonoBehaviour
{
    private float totalHealTimer = 2f;
    private float currentHealTimer;
    private float healAmt;
    private bool setUp = false;
    private StatsManager sm;
	private SkillManager skillManager;

    void Start()
    {
        sm = GetComponentInParent<StatsManager>();
        currentHealTimer = 0f;
		skillManager = GetComponentInParent<SkillManager> ();

		for(int i = 0; i<skillManager.getKnownSkills().Count;i++)
		{
			if(skillManager.getKnownSkills()[i].getName().Equals("Heal"))
			{
				healAmt = skillManager.getKnownSkills()[i].getEffectAmount();
			}
		}


        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
        setUp = true;
    }

    void Update()
    {
        try
        {
            if (setUp == true)
            {
                if (currentHealTimer < totalHealTimer)
                {
                    currentHealTimer += Time.deltaTime;
                    if (sm.getCurrentHealth() < sm.getTotalHealth())
                    {
                        sm.setCurrentHealth(sm.getCurrentHealth() + (healAmt * Time.deltaTime));
                    }
                }
                else
                {
                    GetComponent<PhotonView>().RPC("removeHeal", PhotonTargets.AllBuffered, null);
                }
            }
        }
        catch(System.NullReferenceException o)
        {

        }
    }

    [PunRPC]
    void removeHeal()
    {
        if(GetComponent<PhotonView>().isMine==true)
        {
            StartCoroutine(destroy());
        }
    }

    IEnumerator destroy()
    {
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }
}
