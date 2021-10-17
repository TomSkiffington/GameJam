using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class UserInterface : MonoBehaviour
{
    
    public Player player;
    
    public SO_Inventory inventory;

    public GameObject groundItemPrefab;
    
    protected Vector2 mousePosition;


    protected Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    private void Start() {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this; //so we can know which inventory these inventory slots belong to
        }

        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void Update() {
        mousePosition = player.InputHandler.MousePosition;
        slotsOnInterface.UpdateSlotDisplay();   //extension method
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject button, EventTriggerType type, UnityAction<BaseEventData> action) {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();

        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    protected void OnEnterInterface(GameObject obj) {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    protected void OnExitInterface(GameObject obj) {
        MouseData.interfaceMouseIsOver = null;
    }

    protected void OnEnter(GameObject obj) {
        MouseData.slotHoveredOver = obj;
    }

    protected void OnExit(GameObject obj) {
        MouseData.slotHoveredOver = null;
    }

    protected void OnDragStart(GameObject obj) {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj) {

        GameObject tempItem = null;

        if (slotsOnInterface[obj].item.id >= 0) {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();

            rt.sizeDelta = new Vector2(1,1);
            tempItem.transform.SetParent(transform.parent);

            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.ui_Display;   //database stores graphical part of items
            img.raycastTarget = false;  //object cant be targeted by mouse raycast
        }
        return tempItem;
    }
    
    protected void OnDragEnd(GameObject obj) {  //obj is original object being dragged

        Destroy(MouseData.tempItemBeingDragged);

        if (MouseData.interfaceMouseIsOver == null) {    //mouse not currently over ui interface

            GameObject droppedItem = groundItemPrefab;
            var groundItem = droppedItem.GetComponent<GroundItem>();

            //bug: mods reroll values every time the item is re dropped
            //UPDATE: think I fixed it
            groundItem.SetGroundItem(slotsOnInterface[obj].ItemObject, slotsOnInterface[obj].stackSize, slotsOnInterface[obj].item.mods);

            //scuffed item drop positions at the moment
            Instantiate(droppedItem, (player.transform.position + (Vector3.right * player.Core.Movement.FacingDirection) + Vector3.up * .5f), Quaternion.identity);

            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoveredOver) {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }

    protected void OnDrag(GameObject obj) {
        if (MouseData.tempItemBeingDragged != null) {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = mousePosition;
        }
    }
}

public static class MouseData {
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public static class ExtensionMethods {
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface) {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface) {
            if (_slot.Value.item.id >= 0) {  //slot has item in it
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.ui_Display;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);  //255 255 255 255 white 100% alpha
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.stackSize == 1 ? "" : _slot.Value.stackSize.ToString("n0");
            }
            else {  //no item in slot
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);  //0% alpha
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}
