using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")]
public class SO_Food : SO_Item
{
    public int healAmount;

    public void Awake() {
        type = ItemType.Food;
    }
}
