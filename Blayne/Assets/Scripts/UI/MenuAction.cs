using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuActions
{
    [RequireComponent (typeof (Button))]
    public abstract class MenuAction : MonoBehaviour
    {
        private Button button;
        protected Vector3 selectedLocation, targetLocation;
        protected GameObject selectedObject, targetObject;
        public abstract SpawnMenu.ActionTarget GetSelectedType();
        public abstract SpawnMenu.ActionTarget GetTargetType();
        protected abstract void Execute();
        public void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate { OnClick(); });
        }
        private void OnClick()
        {
            SpawnMenu.Instance.CopySelectionData(out selectedLocation, out targetLocation, out selectedObject, out targetObject);
            SetSquadControl(GetSelectedType(), selectedObject);
            SetSquadControl(GetTargetType(), targetObject);
            Execute();
            SpawnMenu.Instance.ClearMenu();
        }
        private void SetSquadControl(SpawnMenu.ActionTarget target, GameObject targetObj)
        {
            if(target == SpawnMenu.ActionTarget.FriendGroup)
            {
                FireTeam squad = targetObj.GetComponent<FireTeam>();
				if (squad != null) {
					squad.ControlledByPlayer = true;
				}
            }
        }
        
    }
}
