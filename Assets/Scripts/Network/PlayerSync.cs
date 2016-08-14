using UnityEngine;
using System.Collections;

public class PlayerSync : MonoBehaviour 
{

	private Vector3 correctPos;
	private Quaternion correctRot;
	private float speed;
	private float direction;
    private float health;
    private int skill = -1;
    private bool setup = false;
	private PhotonView photonView;
	private Animator anim;
    private StatsManager sm;

	void Start()
	{
		photonView = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
        sm = GetComponent<StatsManager>();
        setup = true;
	}

	void Update()
	{
        if(setup == true)
        {
            if (!photonView.isMine)
            {
                transform.position = Vector3.Lerp(this.transform.position, this.correctPos, Time.deltaTime);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, this.correctRot, Time.deltaTime);
                sm.setCurrentHealth(health);
                anim.SetFloat("Speed", speed);
                anim.SetFloat("Direction", direction);
                //anim.SetInteger("Skill", skill);
            }
        }
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        if(setup == true)
        {
            if (stream.isWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(sm.getCurrentHealth());
                stream.SendNext(anim.GetFloat("Speed"));
                stream.SendNext(anim.GetFloat("Direction"));
               // stream.SendNext(anim.GetInteger("Skill"));
            }
            else
            {
                this.correctPos = (Vector3)stream.ReceiveNext();
                this.correctRot = (Quaternion)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();
                this.speed = (float)stream.ReceiveNext();
                this.direction = (float)stream.ReceiveNext();
                //this.skill = (int)stream.ReceiveNext();
            }
        }
	}
}
