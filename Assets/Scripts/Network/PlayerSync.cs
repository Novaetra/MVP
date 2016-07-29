using UnityEngine;
using System.Collections;

public class PlayerSync : MonoBehaviour 
{

	private Vector3 correctPos;
	private Quaternion correctRot;
	private float speed;
	private float direction;
    private bool setup = false;
	private PhotonView photonView;
	private Animator anim;

	void Start()
	{
		photonView = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
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
                anim.SetFloat("Speed", speed);
                anim.SetFloat("Direction", direction);
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
                stream.SendNext(anim.GetFloat("Speed"));
                stream.SendNext(anim.GetFloat("Direction"));
            }
            else
            {
                this.correctPos = (Vector3)stream.ReceiveNext();
                this.correctRot = (Quaternion)stream.ReceiveNext();
                this.speed = (float)stream.ReceiveNext();
                this.direction = (float)stream.ReceiveNext();
            }
        }
	}
}
