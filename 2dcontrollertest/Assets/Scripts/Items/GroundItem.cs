using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public SO_Item item;
    public Item itemData;
    public int stackSize;
    public ItemMod[] mods;

    private CapsuleCollider2D itemCollider;
    [SerializeField] private LayerMask obstacle;

    private void Start() {
        itemCollider = gameObject.GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate() {
        if (!itemCollider.IsTouchingLayers(obstacle)) {
            gameObject.transform.Translate(0, -0.07f, 0);
        }
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
//#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = item.ui_Display;  //set sprite before object is serialized
        EditorUtility.SetDirty(GetComponent<SpriteRenderer>());   //lets unity know something changed on object
//#endif
    }

    public void SetGroundItem (SO_Item _item, int _stackSize, ItemMod[] _mods) {
        item = _item;
        stackSize = _stackSize;
        mods = _mods;
    }

    public GroundItem (SO_Item _item, int _stackSize, ItemMod[] _mods) {
        item = _item;
        stackSize = _stackSize;
        mods = _mods;
    }
}
