using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class SO_ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    //graphical parts of item are in database, system/logic parts of item on Item class

    public SO_Item[] Items; //items that exist in game to save to database

    [ContextMenu("Update IDs")]
    public void UpdateId() {
        for (int i = 0; i < Items.Length; i++)  //when an item in Item[] is deserialized, the dictionary is populated with those items and their ids
        {
            if (Items[i].data.id != i) {
                Items[i].data.id = i;    
            }
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateId(); //item id always set during serialization
    }

    public void OnBeforeSerialize()
    {
    }
}
