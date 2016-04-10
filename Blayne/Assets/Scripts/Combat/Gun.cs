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

        private float mTimeToNextShot = 0;
        public void Shoot()
        {
            
            float delay = 1.0f / (float)fireRate;
            if (mTimeToNextShot <= 0)
            {
                mTimeToNextShot = 1.0f / (float)fireRate;
                GameObject bulletObj = Instantiate(
                    bulletPrefab.gameObject,
                    muzzle.transform.position,
                    muzzle.transform.rotation)
                    as GameObject;
                bulletObj.transform.localScale = gameObject.transform.localScale;
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.GetComponent<Rigidbody>().AddForce(muzzle.transform.forward * bulletSpeed, ForceMode.Impulse);
                Destroy(bulletObj, bulletLifeTime);
            }
        }
        public void Update()
        {
            mTimeToNextShot -= Time.deltaTime;
        }
    }
}
