using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    float moveSpeedMultiplier = 1;
    float stationaryTurnSpeed = 180;
    float movingTurnSpeed = 360;

    public bool onGround;

    Animator animator;

    Vector3 moveInput;
    float turnAmount;
    float forwardAmount;
    Vector3 velocity;
    float jumpPower = 10;

    IComparer rayHitComparer;
    Rigidbody rigidBody;
    Collider col; //Collider reference

    //our two physics materials where we assign them depending on the occasion
    public PhysicMaterial highFriction;
    public PhysicMaterial lowFriction;

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        SetUpAnimator();

        rigidBody = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void SetUpAnimator()
    {
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

    void OnAnimatorMove()
    {
        if (onGround && Time.deltaTime > 0)
        {
            Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
            v.y = rigidBody.velocity.y;
            rigidBody.velocity = v;
        }
    }

    void SetFriction()
    {
        //Set the appropriate friction depending if we are on the ground or not
        if (onGround)
        {
            //If we are not moving and are on the ground
            if (moveInput.magnitude == 0)
            {
                //we give it a high friction material so that we down slide down slopes
                col.material = highFriction;
            }
            else
            {
                //However if we are moving we don't want friction
                col.material = lowFriction;
            }
        }
        else
        {
            col.material = lowFriction;
        }
    }

    public void Move(Vector3 move)
    {
        if (move.magnitude > 1)
            move.Normalize();

        this.moveInput = move;

        velocity = GetComponent<Rigidbody>().velocity;

        ConvertMoveInput();
        ApplyExtraTurnRotation();

        GroundCheck();
        //SetFriction();

        velocity.y = 0;
        if (moveInput.magnitude == 0)
        {
            velocity.x = 0;
            velocity.z = 0;
        }


        UpdateAnimator();

    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        turnAmount = Mathf.Atan2(localMove.x, localMove.z);

        forwardAmount = localMove.z;
    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void UpdateAnimator()
    {
        animator.applyRootMotion = true;
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }

    void GroundCheck()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, -Vector3.up, Color.red, 0.5f);

        RaycastHit[] hits = Physics.RaycastAll(ray, 0.5f);
        rayHitComparer = new RayHitComparer();
        System.Array.Sort(hits, rayHitComparer);

        if (velocity.y < jumpPower * 0.5f)
        {
            onGround = false;
            rigidBody.useGravity = true;

            foreach (var hit in hits)
            {
                if (!hit.collider.isTrigger)
                {
                    if (velocity.y <= 0)
                    {
                        rigidBody.position = Vector3.MoveTowards(rigidBody.position, hit.point, Time.deltaTime * 5);
                    }

                    onGround = true;
                    rigidBody.useGravity = false;
                    break; //ignore the rest of the hits
                }
            }
        }
    }

    class RayHitComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
