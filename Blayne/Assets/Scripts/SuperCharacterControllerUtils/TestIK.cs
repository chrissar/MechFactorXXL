using UnityEngine;
using System.Collections;

public class TestIK : MonoBehaviour {
    // IK Stuff
    public Transform spine;
    public float aimingZ = 200;
    public float aimingY = 200;
    public float aimingX = 200;
    public float point;

    public Transform cube;
    public Transform cam; //reference to our case
    Animator animator;
    public bool aim;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();

        foreach (var childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != animator)
            {
                animator.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
