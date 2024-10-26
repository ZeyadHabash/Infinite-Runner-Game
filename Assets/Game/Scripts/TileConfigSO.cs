using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniteRunner
{
    [CreateAssetMenu(fileName = "TileConfigSO", menuName = "ScriptableObjects/TileConfigSO")]
    public class TileConfigSO : ScriptableObject
    {
        public Tile.TileType[] TileTypes;
    }
}
