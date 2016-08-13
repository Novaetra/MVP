using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{

    public Transform targetPlayer;
    private NavMeshAgent agent;
    private Animator anim;
    private Player[] players;
    private float distanceToPlayer;
    private float minDistance = Mathf.Infinity;
    private float playerDistance;
    private float meleeRange = 4.0f;
    private float meleeDamage = 20f;
    private float health = 100f;
    private int expOnKill = 10;
    private float pntValue = 100;
    private bool doneSettingUp = false;
    private bool isAlive = true;
    [SerializeField]
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

    //Finds closest player, follow it, and if within melee range, attacks player
    void Update()
    {
        if (doneSettingUp == true && isAlive == true)
        {
            chasePlayer();
        }
        checkAlive();
    }

    private void checkAlive()
    {
        if (health <= 0)
        {
            StartCoroutine(die());
        }
    }

    private void chasePlayer()
    {
        players = PhotonView.FindObjectsOfType<Player>();
        IEnumerable<Player> livePlayers = from player in players where player.GetComponent<StatsManager>().getAlive() select player;
        List<Player> playersList = new List<Player>(players);
        targetPlayer = GetClosestEnemy(livePlayers.ToList());
        if (targetPlayer != null && isAlive == true)
        {
            playerDistance = Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position);
            if (playerDistance > meleeRange)
            {
                agent.enabled = true;
                agent.SetDestination(targetPlayer.position);
                GetComponent<PhotonView>().RPC("walkAnim", PhotonTargets.AllBuffered, null);
            }
            else
            {
                GetComponent<PhotonView>().RPC("idleAnim", PhotonTargets.AllBuffered, null);
                GetComponent<PhotonView>().RPC("attackAnim", PhotonTargets.AllBuffered, null);
                agent.enabled = false;
                rotateTowards(targetPlayer);
            }
        }
    }

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

    public void recieveDamage(float dmg)
    {
        health -= dmg;
        anim.SetFloat("Health", health);
    }

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

    private IEnumerator die()
    {

        startDeath();
        yield return new WaitForSeconds(2f);
        GetComponent<PhotonView>().RPC("destroyRPC", PhotonTargets.AllBuffered, null);
    }

    public void destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void sendPlayersExp()
    {
        PhotonGameManager.currentplayer.GetComponent<StatsManager>().recieveExp(expOnKill);
        PhotonGameManager.currentplayer.GetComponent<StatsManager>().addCurrentPoints(pntValue);
    }

    [PunRPC]
    void destroyRPC()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            destroy();
        }
    }

    [PunRPC]
    void attackAnim()
    {
        GetComponent<Animator>().SetInteger("Skill", 1);
    }

    [PunRPC]
    void walkAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 5);
    }

    [PunRPC]
    void idleAnim()
    {
        GetComponent<Animator>().SetFloat("Speed", 0);
    }

}