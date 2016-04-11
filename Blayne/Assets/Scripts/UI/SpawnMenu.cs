using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using MenuActions;
public class SpawnMenu : MonoBehaviour
{
    public enum ActionTarget
    {
        Friend,
        FriendGroup,
        Enemy,
        EnemyGroup,
        Nothing,
        Anything
    }
    public GameObject menuPrefab;
    public Camera viewCamera;
    public static SpawnMenu Instance
    {
        get;
        private set;
    }
    private Canvas mCanvas;
    private List<GameObject> mLastSpawnedMenu;
    private MenuAction[] mAllActions;

    private Vector3 mSelectedLocation;
    private Vector3 mTargetLocation;
    private GameObject mSelectedObject;
    private GameObject mTargetObject;
    private ActionTarget mSelectedType = ActionTarget.Nothing;
    private ActionTarget mTargetType = ActionTarget.Nothing;
	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        mAllActions = menuPrefab.GetComponentsInChildren<MenuAction>();
        mLastSpawnedMenu = new List<GameObject>();
        mCanvas = GetComponent<Canvas>();
        if (!viewCamera) throw new UnityException("No View Camera attached to the SpawnMenu Script.");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectObject(out mSelectedObject, out mSelectedType, out mSelectedLocation);
            }
            if (Input.GetMouseButtonDown(1))
            {
                SelectObject(out mTargetObject, out mTargetType, out mTargetLocation);
                CreateMenu();
            }
        }
	}
    private void SelectObject(out GameObject obj, out ActionTarget type, out Vector3 location)
    {
        RaycastHit hit = CastRayOnMouse();
        obj = hit.collider ? hit.collider.gameObject : null;
        location = hit.point;
        if (obj)
        {
            FireTeam team = obj.GetComponent<FireTeam>();
            FireTeamAlly ally = obj.GetComponent<FireTeamAlly>();
            if (team != null)
            {
				type = team.TeamSide == 
					FireTeam.Side.Friend ? ActionTarget.FriendGroup : ActionTarget.EnemyGroup;
            }
            else if(ally != null)
            {
				type = ally.fireTeam.TeamSide == 
					FireTeam.Side.Friend ? ActionTarget.Friend : ActionTarget.Enemy;
            }
            else
            {
                type = ActionTarget.Nothing;
            }
        }
        else
        {
            type = ActionTarget.Nothing;
        }
        string name = obj ? obj.name : "None";
        Debug.Log("Selected Object: " + name + " Location: " + location + " Type: " + type);
    }
    private List<MenuAction> GetAllActions(ActionTarget selected, ActionTarget target)
    {
        List<MenuAction> result = new List<MenuAction>();
        foreach(MenuAction action in mAllActions)
        {
            ActionTarget selectedType = action.GetSelectedType();
            ActionTarget targetType = action.GetTargetType();
            if((selectedType == selected
                || selectedType == ActionTarget.Anything)
                && (targetType == target
                || targetType == ActionTarget.Anything))
            {
                result.Add(action);
            }
            Debug.Log("Action S: " + selectedType + " T: " + targetType);
        }
        return result;
    }
    
    public void Toggle()
    {
        enabled = !enabled;
    }

    public void ClearMenu()
    {
        if (mLastSpawnedMenu.Count > 0)
        {
            foreach (GameObject menuItem in mLastSpawnedMenu)
            {
                Destroy(menuItem);
            }
            mLastSpawnedMenu.Clear();
        }
    }
    private void CreateMenu()
    {
        ClearMenu();
        List<MenuAction> actions = GetAllActions(mSelectedType, mTargetType);
        Vector3 position = Input.mousePosition - GetRectSize(mCanvas.pixelRect) * 0.5f;
        float verticalOffset = 0.0f;
        foreach (MenuAction action in actions)
        {
            GameObject item = Instantiate(action.gameObject);
            mLastSpawnedMenu.Add(item);
            item.transform.SetParent(mCanvas.transform);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            Vector3 rectSize = GetRectSize(itemRect.rect);
            Vector3 invertedYRectSize = new Vector3(rectSize.x, -rectSize.y, rectSize.z) * 0.5f - Vector3.up * verticalOffset;
            verticalOffset += rectSize.y;
            itemRect.localPosition = position + invertedYRectSize;
        }
    }
    private Vector3 GetRectSize(Rect rect)
    {
        Vector2 size = rect.size;
        return new Vector3(size.x, size.y, 0);
    }
    private RaycastHit CastRayOnMouse()
    {
        RaycastHit result;
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out result);
        return result;
    }
    public void CopySelectionData(
        out Vector3 selectedLocation, 
        out Vector3 targetLocation, 
        out GameObject selectedObject, 
        out GameObject targetObject)
    {
        selectedLocation = this.mSelectedLocation;
        targetLocation = this.mTargetLocation;
        selectedObject = this.mSelectedObject;
        targetObject = this.mTargetObject;
    }
}
