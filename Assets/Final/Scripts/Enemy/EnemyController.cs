using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform targetPlayer;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private Player[] players;

    private float minDistance = Mathf.Infinity;
    private float playerDistance;
    private float proximityRange = 2.5f;
    private float attackRange = 1.5f;
	private float meleeDamage;
    private float movementSpeed;
    //private float totalHealth;
    private float totalHealth = 100;
    [SerializeField]
    private float currentHealth;
    private float expOnKill;

    private bool doneSettingUp = false;
    private bool isAlive = true;

    private Raycaster[] casters;

    //Assigns the enemy's nav agent, animator, and list of players
    public void Start()
    {
		currentHealth = totalHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Health", currentHealth);
        casters = GetComponentsInChildren<Raycaster>();
        doneSettingUp = true;
        targetPlayer = GameObject.Find("Player").transform;
        //proximityRange = agent.stoppingDistance + 1f;
        movementSpeed = 1.5f;
    }

    //Finds closest player, follow it, and if player is within melee range, attacks player
	//Check if enemy is alive
    void Update()
    {
        if (doneSettingUp == true && isAlive == true)
        {
            chasePlayer();
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
                    rotateTowards(targetPlayer);
                }
                //Else if the player is within melee range, attack
                else
                {
                    idleAnim();
                    attackAnim();
                    // agent.enabled = false;
                    rotateTowards(targetPlayer);
                }
        }
    }

    private void rotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * movementSpeed);
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
        if (isAlive)
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
        anim.SetTrigger("TakeDamage");
    }

	//Is called from animation event and starts the die coroutine
    public void startDeath()
    {
        StartCoroutine(die());
    }

	//Firsts starts the death animation, waits x seconds, and then destroys the enemy
    private IEnumerator die()
    {
        if (isAlive == true)
        {
            isAlive = false;
            agent.enabled = false;

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<Rigidbody>());
            sendPlayersExp();
            GameObject.Find("Managers").transform.GetComponent<EnemyManager>().decreaseEnemyCount();
        }
        yield return new WaitForSeconds(30f);
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

    public void setMovementSpeed(float speed)
    {
        GetComponent<NavMeshAgent>().speed = speed;
        movementSpeed = speed;
    }

    public void setAttackProximity(float range)
    {
        proximityRange = range;
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