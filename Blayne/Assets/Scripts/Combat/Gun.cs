using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Combat
{
    public class Gun : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public GameObject muzzle;
        public int fireRate;
        public float bulletSpeed;
        public float bulletLifeTime;

        private float mTimeBetweenShots = 0;
        public void Shoot()
        {
            mTimeBetweenShots += Time.deltaTime;
            if (mTimeBetweenShots >= 1.0f / (float)fireRate)
            {
                mTimeBetweenShots = 0;
                GameObject bulletObj = Instantiate(
                    bulletPrefab.gameObject,
                    muzzle.transform.position,
                    muzzle.transform.rotation)
                    as GameObject;

                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.velocity = muzzle.transform.right * bulletSpeed;
            }
        }
    }
}
