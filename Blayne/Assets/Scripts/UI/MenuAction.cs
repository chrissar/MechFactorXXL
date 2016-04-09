using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuActions
{
    public class MenuAction : MonoBehaviour
    {
        public SpawnMenu.ActionTarget selectedType;
        public SpawnMenu.ActionTarget targetType;
        private Button button;
        protected Vector3 selectedLocation, targetLocation;
        protected GameObject selectedObject, targetObject;
        public void Awake()
        {
            button = GetComponent<Button>();
            if (!button) throw new UnityException("A Menu Action is attached to an object that is not a button!");
            button.onClick.AddListener(delegate { OnClick(); });
        }
        private void OnClick()
        {
            SpawnMenu.instance.CopySelectionData(out selectedLocation, out targetLocation, out selectedObject, out targetObject);
            Execute();
        }
        protected virtual void Execute()
        {

        }
    }
}
