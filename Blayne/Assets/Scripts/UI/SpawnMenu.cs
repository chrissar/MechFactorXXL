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
        Nothing
    }
    public GameObject menuPrefab;
    public Camera viewCamera;
    public static SpawnMenu instance
    {
        get;
        private set;
    }
    private Canvas _canvas;
    private List<GameObject> lastSpawnedMenu;
    private MenuAction[] allActions;

    private Vector3 selectedLocation;
    private Vector3 targetLocation;
    private GameObject selectedObject;
    private GameObject targetObject;
    private ActionTarget selectedType = ActionTarget.Nothing;
    private ActionTarget targetType = ActionTarget.Nothing;
	// Use this for initialization
	void Awake ()
    {
        instance = this;
        allActions = menuPrefab.GetComponentsInChildren<MenuAction>();
        lastSpawnedMenu = new List<GameObject>();
        _canvas = GetComponent<Canvas>();
        if (!viewCamera) throw new UnityException("No View Camera attached to the SpawnMenu Script.");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectObject(out selectedObject, out selectedType, out selectedLocation);
        }
	    if(Input.GetMouseButtonDown(1))
        {
            SelectObject(out targetObject, out targetType, out targetLocation);
            CreateMenu();
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
                type = team.side == FireTeam.Side.Friend ? ActionTarget.FriendGroup : ActionTarget.EnemyGroup;
            }
            else if(ally != null)
            {
                type = ally.fireTeam.side == FireTeam.Side.Friend ? ActionTarget.Friend : ActionTarget.Enemy;
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
        foreach(MenuAction action in allActions)
        {
            if(action.selectedType == selected && action.targetType == target)
            {
                result.Add(action);
            }
        }
        return result;
    }
    private void CreateMenu()
    {
        if(lastSpawnedMenu.Count > 0)
        {
            foreach(GameObject menuItem in lastSpawnedMenu)
            {
                Destroy(menuItem);
            }
            lastSpawnedMenu.Clear();
        }
        List<MenuAction> actions = GetAllActions(selectedType, targetType);
        Vector3 position = Input.mousePosition - GetRectSize(_canvas.pixelRect) * 0.5f;
        float verticalOffset = 0.0f;
        foreach (MenuAction action in actions)
        {
            GameObject item = Instantiate(action.gameObject);
            lastSpawnedMenu.Add(item);
            item.transform.SetParent(_canvas.transform);
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
        selectedLocation = this.selectedLocation;
        targetLocation = this.targetLocation;
        selectedObject = this.selectedObject;
        targetObject = this.targetObject;
    }
}
