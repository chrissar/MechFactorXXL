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
        public bool CanShoot = true;

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
                //bulletObj.transform.localScale = gameObject.transform.localScale;
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.GetComponent<Rigidbody>().AddForce(muzzle.transform.forward * bulletSpeed * 50, ForceMode.Impulse);
                Destroy(bulletObj, bulletLifeTime);
                CanShoot = false;
            }
        }
        public void Update()
        {
            mTimeToNextShot -= Time.deltaTime;
            if (mTimeToNextShot <= 0)
            {
                CanShoot = true;
            }
        }
    }
}
