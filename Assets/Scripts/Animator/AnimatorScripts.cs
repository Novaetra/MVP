using UnityEngine;
using System.Collections;

public class AnimatorScripts : MonoBehaviour
{
    private Animator anim;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        updateColliderHeight();
	}
    void updateColliderHeight()
    {
        if (GetComponent<CharacterController>() != null)
        {
            GetComponent<CharacterController>().height = anim.GetFloat("colliderHeight");
        } else if(GetComponent<CapsuleCollider>()!=null)
        {
            GetComponent<CapsuleCollider>().height = anim.GetFloat("colliderHeight");
            GetComponent<NavMeshAgent>().height = anim.GetFloat("colliderHeight");
            GetComponent<NavMeshAgent>().baseOffset = anim.GetFloat("navOffset");
        }
    }
}
