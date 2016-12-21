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

    private float minDistance = Mathf.Infinity;
    private float playerDistance;
    private float proximityRange = 2.5f;
    private float attackRange = 1.5f;
    [SerializeField]
	private float meleeDamage = 20f;
    [SerializeField]
    private float totalHealth = 100f;
    private float currentHealth;
    [SerializeField]
    private float expOnKill = 10f;

    private bool doneSettingUp = false;
    private bool isAlive = true;

    private Raycaster[] casters;

    //Assigns the enemy's nav agent, animator, and list of players
    public void Start()
    {
		currentHealth = totalHealth;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Health", currentHealth);
        casters = GetComponentsInChildren<Raycaster>();
        doneSettingUp = true;
        targetPlayer = GameObject.Find("Player").transform;
        proximityRange = agent.stoppingDistance + 1f;
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
        if (currentHealth <= 0)
        {
            StartCoroutine(die());
        }
    }

	//Chases the closest player
    private void chasePlayer()
    {
		//Gets a list of all the players
        players = GameObject.FindObjectsOfType<Player>();
		//Gets a list of all the ALIVE players from the list of all players
        IEnumerable<Player> livePlayers = from player in players where player.GetComponent<StatsManager>().getAlive() select player;

		//Get closest player and assign it as the target
        //targetPlayer = GetClosestEnemy(livePlayers.ToList());

        if (targetPlayer != null && isAlive == true)
        {
			//Gets the distance between the player and the enemy
            playerDistance = Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position);
            //If the distance is greater than the enemy's melee range, then walk toward the target player
            if (playerDistance > proximityRange)
            {
                agent.enabled = true;
                agent.SetDestination(targetPlayer.position);
                walkAnim();
                stopAttackAnim();
            }
			//Else if the player is within melee range, attack
            else
            {
                idleAnim();
                attackAnim();
               // agent.enabled = false;
                //rotateTowards(targetPlayer);
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

    /*
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
    */
	//Throws rays to check if the melee attack hit a player
	//If it hit a player, then apply damage
    public void checkAttack()
    {
        if(isAlive)
        {
            RaycastHit hit;
            foreach (Raycaster caster in casters)
            {
                Transform raycaster = caster.transform;
                Debug.DrawRay(raycaster.position, raycaster.forward * attackRange, Color.blue, 1);
                if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, attackRange))
                {
                    if (hit.transform.tag == "Player")
                    {
                        hit.transform.GetComponent<StatsManager>().recieveDamage(meleeDamage);
                        return;
                    }
                }
            }
        }
    }

	//Lower the enemy's health by x amt
    public void recieveDamage(float dmg)
    {
        currentHealth -= dmg;
        anim.SetFloat("Health", currentHealth);
    }

	//Start death animation and trigger everything needed to kill the dying enemy
    public void startDeath()
    {
        if (isAlive == true)
        {
            isAlive = false;
            agent.enabled = false;
            sendPlayersExp();
            GameObject.Find("Managers").transform.GetComponent<EnemyManager>().decreaseEnemyCount();
        }
    }

	//Firsts starts the death animation, waits x seconds, and then destroys the enemy
    private IEnumerator die()
    {

        startDeath();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

	public void setTotalHealth(float h)
	{
		totalHealth = h;
		currentHealth = h;
	}

	public void setMeleeDamage(float d)
	{
		meleeDamage = d;
	}

	public void setExpOnKill(float exp)
	{
		expOnKill = exp;
	}

	//Temporary...might just give the person with the killing blow any exp
	//Send all players exp
    void sendPlayersExp()
    {
        GameManager.currentplayer.GetComponent<StatsManager>().recieveExp(expOnKill);
    }
    

	//Triggers an attack animation on all clients
    void attackAnim()
    {
        GetComponent<Animator>().SetInteger("Skill", 1);
    }

    void stopAttackAnim()
    {
        GetComponent<Animator>().SetInteger("Skill", 0);
    }

    //Triggers an walk animation on all clients
    void walkAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 5);
    }

	//Triggers an idle animation on all clients
    void idleAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 0);
    }

}