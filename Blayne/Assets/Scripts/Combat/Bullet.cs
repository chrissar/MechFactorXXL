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
        public int damage;
        public bool destroyOnContact = true;
        [HideInInspector]public Vector3 velocity = Vector3.zero;
        private Rigidbody mRigidBody;
        public void Awake()
        {
            mRigidBody = gameObject.GetComponent<Rigidbody>();
            mRigidBody.isKinematic = true;
        }
        public void Update()
        {
            mRigidBody.MovePosition(velocity * Time.deltaTime);
        }
        public void OnCollisionEnter(Collision collision)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if(health)
            {
                health.TakeDamage(damage);
            }
            if(destroyOnContact)
            {
                Destroy(gameObject);
            }
        }
    }
}
