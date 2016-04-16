using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Combat
{
    [RequireComponent (typeof (Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        public GameObject mParticalSystem;
        public int damage;
        public bool destroyOnContact = true;

        void Awake()
        {
            mParticalSystem = Resources.Load("effects/impactMetal") 
                as GameObject;
        }

        void Start()
        {

        }
        //[HideInInspector]public Vector3 velocity = Vector3.zero;
        //private Rigidbody mRigidBody;
       /* public void Awake()
        {
            mRigidBody = gameObject.GetComponent<Rigidbody>();
            //mRigidBody.isKinematic = true;
        }
        public void Update()
        {
            Vector3 newPosition = mRigidBody.transform.position + velocity * Time.deltaTime;
            mRigidBody.MovePosition(newPosition);
        }*/
        public void OnCollisionEnter(Collision collision)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if(health)
            {
                health.TakeDamage(damage);
            }
            if(destroyOnContact)
            {
                Instantiate(mParticalSystem, collision.contacts[0].point, Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)); 
                //then we'll instantiate a random bullet hole texture from our array and apply it where we click and adjust
                // the position and rotation of textures to match the object being hit
                Destroy(gameObject);
            }
        }
    }
}
