using UnityEngine;
using System.Collections;

public class SkillInitializer : MonoBehaviour 
{
    public GameObject SPAWNER, FIREBALL, FLAMETHROWER, HEAL;
	public Transform leftSpawner;
    public Transform middleSpawner;
    public Transform floorSpawner;

	public void createFireball()
	{
		//Instantiate a spell spawner at the location, then instantite a fireball. Set the rotation of the spell spawner to make the fireball go straight

		GameObject spellSpawner = (GameObject)GameManager.Instantiate (SPAWNER, leftSpawner.position, leftSpawner.rotation);
		GameObject fireBall = (GameObject)GameObject.Instantiate (FIREBALL, spellSpawner.transform.position, spellSpawner.transform.rotation);
		fireBall.transform.SetParent(spellSpawner.transform);
		fireBall.transform.localScale = new Vector3(2.4f,2.4f,2.4f);
	}

    public void createFlamethrower()
    {
        GameObject flameThrower = (GameObject)GameObject.Instantiate(FLAMETHROWER, leftSpawner.transform.position, leftSpawner.transform.rotation);
        flameThrower.transform.SetParent(leftSpawner.transform);
        flameThrower.transform.localScale = new Vector3(3, 3, 3);
        flameThrower.transform.localEulerAngles += new Vector3(0f, 180, 0f);
    }

    public void createHeal()
    {
        GameObject healParticles = (GameObject)GameObject.Instantiate(HEAL, floorSpawner.transform.position, floorSpawner.transform.rotation);
        healParticles.transform.SetParent(floorSpawner.transform);
        healParticles.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        healParticles.transform.localScale = new Vector3(1, 1, 1);
    }

}
