using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileType
{
    public int id;
    public string name;
    public float hardness;
    public float health;
    //toolDamageType = pickaxe, axe, aggressive weapon etc
    //lightreduction?
    //hitsound
    //tag

    public static TileType CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<TileType>(jsonString);
    }
}

public class TileTypeLoader 
{
    public IEnumerable<TileType> LoadTileTypes() {
        var tileTypes = new List<TileType>();
        var tileTypeStrings = File.ReadAllLines("C:/Users/jrlok/Desktop/code/Unitystuff/2D-Controller/2dcontrollertest/Assets/Data/Tiles");

        foreach (var tileTypeString in tileTypeStrings) {
            var tileType = TileType.CreateFromJSON(tileTypeString);
            tileTypes.Add(tileType);
        }

        return tileTypes;
    }
}

/*

JSON file tile data example:

{"id": 0, "name": "air", "hardness": 0, "health": 0}
{"id": 1, "name": "dirt", "hardness": 1, "health": 10}
{"id": 2, "name": "clay", "hardness": 1, "health": 10}
{"id": 3, "name": "iron", "hardness": 2, "health": 25}

*/
