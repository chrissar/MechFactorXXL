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
        public KeyCode shootKey = KeyCode.Space;
        private Gun mGun;
        public void Awake()
        {
            mGun = gameObject.GetComponent<Gun>();
        }
        public void Update()
        {
            if(Input.GetKey(shootKey))
            {
                mGun.Shoot();
            }
        }
    }
}
