using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Combat
{
    public class HealthIndicator : MonoBehaviour
    {
        public Color healthyColor;
        public Color damagedColor;
        public Color criticalColor;
        public Health health;
        [Range(0, 100)]
        public int damagedPercentage;
        [Range(0, 100)]
        public int criticalPercentage;

        public void Awake()
        {
            if (!health) throw new UnityException("No Health linked to a Health Indicator");
        }
        public void Update()
        {
            int percentage = (int)(((float)health.health / (float)health.maxHealth) * 100.0f);
            Color newColor = percentage > criticalPercentage ? percentage > damagedPercentage ? healthyColor : damagedColor : criticalColor;
            gameObject.GetComponent<MeshRenderer>().material.color = newColor;
        }
    }
}
