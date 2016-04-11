using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Combat
{
    [RequireComponent (typeof(Gun))]
    public class TestGunController : MonoBehaviour
    {
        private Gun mGun;
        public void Awake()
        {
            mGun = gameObject.GetComponent<Gun>();
        }
        public void Update()
        {
            if(Input.GetKey(KeyCode.Space))
            {
                mGun.Shoot();
            }
        }
    }
}
