using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Combat
{
    public class Health : MonoBehaviour
    {
        public int health;
        public int maxHealth;

        public void Awake()
        {
            health = maxHealth;
        }
        public void TakeDamage(int damage)
        {
            health = Math.Max(health - damage, 0);
        }
        public virtual void Die()
        {
            Destroy(gameObject);
        }
        public void Update()
        {
            if(health == 0)
            {
                Die();
            }
        }
    }
}
