using UnityEngine;
using System.Collections;

public class FireballScript : MonoBehaviour 
{
    private SkillManager sm;
    private float dmg;
    private float range = 0.75f;
    private Raycaster[] casters;
    private bool isDestroyed;
    private int enemiesHit = 0;
    private int maxEnemiesHit = 2;

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
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy" && enemiesHit<maxEnemiesHit)
        {
            col.transform.BroadcastMessage("recieveDamage", dmg, SendMessageOptions.DontRequireReceiver);
            destroyFireball();
            enemiesHit++;
        }
    }
}
