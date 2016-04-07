using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {

    public bool walkByDefault = false;

    private CharacterMovement character;
    private Transform cam;
    private Vector3 camForward;
    private Vector3 move;

    void Start()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }

        character = GetComponent<CharacterMovement>();
    }

    void FixedUpdate()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        if (cam != null)
        {
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = vertical * camForward + horizontal * cam.right;
        }
        else
        {
            move = vertical * Vector3.forward + horizontal * Vector3.right;
        }

        if (move.magnitude > 1)
            move.Normalize();

        //Debug.Log(move);

        bool walkToggle = Input.GetKey(KeyCode.LeftShift);

        float walkMultiplier = 1;

        if (walkByDefault)
        {
            if (walkToggle)
            {
                walkMultiplier = 1;
            }
            else
            {
                walkMultiplier = 0.5f;
            }
        }
        else
        {
            if (walkToggle)
            {
                walkMultiplier = 0.5f;
            }
            else
            {
                walkMultiplier = 1;
            }
        }

        move *= walkMultiplier;
        Debug.Log("Move: " + move);
        character.Move(move);
    }
}
