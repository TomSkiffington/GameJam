using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")]
public class SO_Equipment : SO_Item
{
    public int defence;

    public void Awake() {
        //type = ItemType.BodyArmour;
    }
}
