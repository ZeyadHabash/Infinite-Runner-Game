using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
public class TileType : ScriptableObject
{
    [field: SerializeField] public string TileName { get; private set; }
    [field: SerializeField] public Color TileColor { get; private set; }

    public void OnTileEntered()
    {
        Debug.Log("Tile Entered: " + TileName);
    }
}
