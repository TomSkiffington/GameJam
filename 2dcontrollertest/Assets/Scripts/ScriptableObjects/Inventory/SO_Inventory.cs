using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class SO_Inventory : ScriptableObject
{
    public string savePath;
    public SO_ItemDatabase database;
    public Inventory Container;

    private void OnEnable() {
        #if UNITY_EDITOR
        database = (SO_ItemDatabase)AssetDatabase.LoadAssetAtPath("Assets/Resources/Database.asset", typeof(SO_ItemDatabase));
        #else
        database = Resources.Load<SO_ItemDatabase>("Database");
        #endif
    }

    public bool AddItem(Item _item, int _amount) {

        if (EmptySlotCount <= 0) {
            return false;
        }

        InventorySlot slot = FindItemInInventory(_item);    //slot item is in (if not null)

        if (!database.Items[_item.id].stackable || slot == null) {  //if item isn't stackable or it's not found in the inventory
            SetNextEmptySlot(_item, _amount);
            return true;
        }

        slot.AddAmountToStack(_amount);
        return true;
    }

    public int EmptySlotCount {
        get {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.id <= -1) {
                    counter++;
                }
            }
            return counter;
        }
    }

    public InventorySlot FindItemInInventory(Item _item) {  //looks for item in inventory and returns the slot it's in
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.id == _item.id) {
                return Container.Items[i];
            }
        }
        return null;
    }

    public InventorySlot SetNextEmptySlot(Item _item, int _amount) {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.id <= -1) {  //-1 is item Id for an empty inventoryslot
                Container.Items[i].UpdateSlot( _item, _amount);
                return Container.Items[i];
            }
        }
        //set up functionality for what happens when inventory is full (no more slots to set)
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2) {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject)) {
            InventorySlot temp = new InventorySlot(item2.item, item2.stackSize);

            item2.UpdateSlot(item1.item, item1.stackSize);
            item1.UpdateSlot(temp.item, temp.stackSize);
        }
        
    }

    public void DropItem(Item _item) {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item) {
                Container.Items[i].UpdateSlot(null, 0);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save() {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load() {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath))) {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);

            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].item, newContainer.Items[i].stackSize);
            }

            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear() {
        Container.Clear();
    }
}

[System.Serializable]
public class Inventory 
{
    public InventorySlot[] Items = new InventorySlot[35];

    public void Clear() {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
        }
    }
}

[System.Serializable]
public class InventorySlot 
{
    public ItemType[] allowedItems = new ItemType[0];   //when there are 0 items in array, all item types allowed in slot
    public Item item;

    public int stackSize;
    
    [System.NonSerialized]
    public UserInterface parent;    //can't be serialized

    public SO_Item ItemObject {
        get {
            if (item.id >= 0) {
                return parent.inventory.database.Items[item.id];
            }
            else {
                return null;
            }
        }
    }

    public InventorySlot() {
        item = new Item();  
        stackSize = 0;
    }

    public InventorySlot(Item _item, int _stackSize) {
        item = _item;  
        stackSize = _stackSize;
    }

    public void UpdateSlot(Item _item, int _stackSize) {
        item = _item;  
        stackSize = _stackSize;
    }

    public void RemoveItem() {
        item = new Item();
        stackSize = 0;
    }

    public void AddAmountToStack(int value) {
        stackSize += value;
    }

    public bool CanPlaceInSlot(SO_Item _itemObject) {
        if (allowedItems.Length <= 0 || _itemObject == null || _itemObject.data.id < 0) {
            return true;
        }

        for (int i = 0; i < allowedItems.Length; i++)
        {
            if (_itemObject.type == allowedItems[i]) {
                return true;
            }
        }
        return false;
    }
}
