using UnityEngine;
using System.Collections;

public class FireballScript : MonoBehaviour 
{
    private SkillManager sm;
    private float dmg;
    private float range = 0.75f;
    private Raycaster[] casters;
    private bool isDestroyed;
	void Start()
	{
        sm = PhotonGameManager.currentplayer.GetComponent<SkillManager>();
        for(int i = 0; i<sm.getKnownSkills().Count;i++)
        {
            if(sm.getKnownSkills()[i].getName().Equals("Fireball"))
            {
                dmg = sm.getKnownSkills()[i].getEffectAmount();
            }
        }

        casters = GetComponentsInChildren<Raycaster>();
        isDestroyed = false;
    }


	private IEnumerator destroyFire()
	{
        GetComponent<ParticleSystem> ().Stop (true);

        yield return new WaitForSeconds (1f);
        Destroy(transform.parent.gameObject);
    }

    public void destroyFireball()
	{
        isDestroyed = true;
        StartCoroutine (destroyFire ());
	}

	void Update()
	{
        if (!isDestroyed)
        {
            RaycastHit hit;
            foreach (Raycaster caster in casters)
            {
                Debug.DrawRay(caster.transform.position, caster.transform.forward * range, Color.red);
                if (Physics.Raycast(caster.transform.position, caster.transform.forward, out hit, range))
                {
                    if (hit.transform.gameObject.tag == "Enemy")
                    {
                        hit.transform.BroadcastMessage("recieveDamage", dmg, SendMessageOptions.DontRequireReceiver);
                        destroyFireball();
                        return;
                    }
                }
            }
        }
	}
}
