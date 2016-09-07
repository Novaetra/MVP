using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    private Transform targetPlayer;
    private NavMeshAgent agent;
    private Animator anim;
    private Player[] players;

    private float distanceToPlayer;
    private float minDistance = Mathf.Infinity;
    private float playerDistance;
	[SerializeField]
    private float meleeRange = 2.0f;
	private float meleeDamage = 20f;
	[SerializeField]
    private float health = 100f;
	private float pntValue = 100;
	private int expOnKill = 10;

    private bool doneSettingUp = false;
    private bool isAlive = true;

    private Raycaster[] casters;

    //Assigns the enemy's nav agent, animator, and list of players
    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Health", health);
        casters = GetComponentsInChildren<Raycaster>();
        doneSettingUp = true;
    }

    //Finds closest player, follow it, and if player is within melee range, attacks player
	//Check if enemy is alive
    void Update()
    {
        if (doneSettingUp == true && isAlive == true)
        {
            chasePlayer();
        }
        checkAlive();
    }

	//Checks if enemy's health is greater than 0 (if enemy is alive)
    private void checkAlive()
    {
        if (health <= 0)
        {
            StartCoroutine(die());
        }
    }

	//Chases the closest player
    private void chasePlayer()
    {
		//Gets a list of all the players
        players = PhotonView.FindObjectsOfType<Player>();
		//Gets a list of all the ALIVE players from the list of all players
        IEnumerable<Player> livePlayers = from player in players where player.GetComponent<StatsManager>().getAlive() select player;

		//Get closest player and assign it as the target
        targetPlayer = GetClosestEnemy(livePlayers.ToList());

        if (targetPlayer != null && isAlive == true)
        {
			//Gets the distance between the player and the enemy
            playerDistance = Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position);
			//If the distance is greater than the enemy's melee range, then walk toward the target player
            if (playerDistance > meleeRange)
            {
                agent.enabled = true;
                agent.SetDestination(targetPlayer.position);
                GetComponent<PhotonView>().RPC("walkAnim", PhotonTargets.AllBuffered, null);
            }
			//Else if the player is within melee range, attack
            else
            {
                GetComponent<PhotonView>().RPC("idleAnim", PhotonTargets.AllBuffered, null);
                GetComponent<PhotonView>().RPC("attackAnim", PhotonTargets.AllBuffered, null);
                agent.enabled = false;
                rotateTowards(targetPlayer);
            }
        }
    }

	//Rotate towards the target player
    private void rotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
    }


    //Returns the cloest enemy
    private Transform GetClosestEnemy(List<Player> players)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = gameObject.transform.position;
        foreach (Player potentialTarget in players)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }
	//Throws rays to check if the melee attack hit a player
	//If it hit a player, then apply damage
    public void checkAttack()
    {
        RaycastHit hit;
        foreach (Raycaster caster in casters)
        {
            Transform raycaster = caster.transform;
            Debug.DrawRay(raycaster.position, raycaster.forward * meleeRange, Color.blue, 1);
            if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, meleeRange))
            {
                if (hit.transform.tag == "Player")
                {
                    hit.transform.GetComponent<StatsManager>().recieveDamage(meleeDamage);
                    return;
                }
            }
        }
    }

	//Lower the enemy's health by x amt
    public void recieveDamage(float dmg)
    {
        health -= dmg;
        anim.SetFloat("Health", health);
    }

	//Start death animation and trigger everything needed to kill the dying enemy
    public void startDeath()
    {
        if (isAlive == true)
        {
            isAlive = false;
            agent.enabled = false;
            GetComponent<PhotonView>().RPC("sendPlayersExp", PhotonTargets.AllBuffered, null);
            GameObject.Find("Network").transform.GetComponent<EnemyManager>().decreaseEnemyCount();
        }
    }

	//Firsts starts the death animation, waits x seconds, and then destroys the enemy
    private IEnumerator die()
    {

        startDeath();
        yield return new WaitForSeconds(2f);
        GetComponent<PhotonView>().RPC("destroyRPC", PhotonTargets.AllBuffered, null);
    }

	//Destroys the enemy
    public void destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

	//Temporary...might just give the person with the killing blow any exp
	//Send all players exp
    [PunRPC]
    void sendPlayersExp()
    {
        PhotonGameManager.currentplayer.GetComponent<StatsManager>().recieveExp(expOnKill);
        PhotonGameManager.currentplayer.GetComponent<StatsManager>().addCurrentPoints(pntValue);
    }

	//Destroys the enemy on all clients
    [PunRPC]
    void destroyRPC()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            destroy();
        }
    }

	//Triggers an attack animation on all clients
    [PunRPC]
    void attackAnim()
    {
        GetComponent<Animator>().SetInteger("Skill", 1);
    }


	//Triggers an walk animation on all clients
    [PunRPC]
    void walkAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 5);
    }

	//Triggers an idle animation on all clients
    [PunRPC]
    void idleAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 0);
    }

}