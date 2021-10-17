using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
public class SO_Default : SO_Item
{
    public void Awake() {
        type = ItemType.Default;
    }
}
