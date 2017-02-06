using UnityEngine;
using System.Collections;

public class FlamethrowerScript : MonoBehaviour {

    private float currentTime, totalTimer;
    private bool doneSetup = false;
    private SkillManager sm;
    [SerializeField]
    private float dmg;

	private PlayerController ps;

	void Start ()
    {
		currentTime = 0f;
		totalTimer = 3f;

        sm = GameManager.currentplayer.GetComponent<SkillManager>();
        for (int i = 0; i < sm.getKnownSkills().Count; i++)
        {
            if (sm.getKnownSkills()[i].getName().Equals("Flamethrower"))
            {
                dmg = sm.getKnownSkills()[i].getEffectAmount();
            }
        }

		ps = GetComponentInParent<PlayerController> ();

        doneSetup = true;
	}
    
    void Update()
    {
        if (doneSetup)
        {
            if (currentTime < totalTimer)
            {
                currentTime+=Time.deltaTime;
            }
            else
            {
                StartCoroutine(destroyFire());
            }
        }
    }

    private IEnumerator destroyFire()
    {
        GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
    

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Enemy")
        {
            col.transform.GetComponent<EnemyController>().recieveDamage(dmg);
        }
    }

}
