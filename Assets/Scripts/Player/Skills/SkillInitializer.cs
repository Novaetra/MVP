using UnityEngine;
using System.Collections;

public class SkillInitializer : MonoBehaviour 
{

	public Transform leftSpawner;
    public Transform middleSpawner;
    public Transform floorSpawner;

	public void createFireball()
	{
		//Instantiate a spell spawner at the location, then instantite a fireball. Set the rotation of the spell spawner to make the fireball go straight

		GameObject spellSpawner = (GameObject)PhotonNetwork.Instantiate ("Spawner", leftSpawner.position, leftSpawner.rotation, 0);
		GameObject fireBall = (GameObject)PhotonNetwork.Instantiate ("Fireball", spellSpawner.transform.position, spellSpawner.transform.rotation, 0);
		fireBall.transform.SetParent(spellSpawner.transform);
		fireBall.transform.localScale = new Vector3(2.4f,2.4f,2.4f);
	}

    public void createFlamethrower()
    {
        GameObject flameThrower = (GameObject)PhotonNetwork.Instantiate("Flamethrower", middleSpawner.transform.position, middleSpawner.transform.rotation, 0);
        flameThrower.transform.SetParent(middleSpawner.transform);
        flameThrower.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        flameThrower.transform.localScale = new Vector3(3, 3, 3);
    }

    public void createHeal()
    {
        GameObject healParticles = (GameObject)PhotonNetwork.Instantiate("Heal", floorSpawner.transform.position, floorSpawner.transform.rotation, 0);
        healParticles.transform.SetParent(floorSpawner.transform);
        healParticles.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        healParticles.transform.localScale = new Vector3(1, 1, 1);
    }

}
