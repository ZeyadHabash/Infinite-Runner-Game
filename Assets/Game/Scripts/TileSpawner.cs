using System.Collections;
using System.Collections.Generic;
using GDK;
using UnityEngine;
using UnityEngine.Pool;

namespace InfiniteRunner
{
    public class TileSpawner : MonoBehaviour
    {
        #region Fields
        [Header("Configs")]
        [SerializeField] private TileConfigSO[] _tileConfigs;

        [Header("Object Pools")]
        [SerializeField] private SuperObjectPoolSO _tilePool;

        private Vector3 _spawnPosition;
        private bool _firstSpawn = true;

        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            _spawnPosition = _tilePool.defaultSpawnLocation;

            // Spawn initial tiles (no random tiles)
            for (int i = 0; i < 10; i++)
            {
                SpawnTileGroup();
            }
            _firstSpawn = false;

            // Spawn random tiles after initial tiles
            for (int i = 0; i < 20; i++)
            {
                SpawnTileGroup();
            }
        }

        private void Update()
        {

        }
        #endregion



        #region Public Methods

        #endregion



        #region Private Methods
        private void SpawnTileGroup()
        {
            // Spawn Left Tile
            SpawnTile(_spawnPosition + Vector3.left * 5);
            // Spawn Middle Tile
            SpawnTile(_spawnPosition);
            // Spawn Right Tile
            SpawnTile(_spawnPosition + Vector3.right * 5);
            _spawnPosition += Vector3.forward * 5;
        }

        private void SpawnTile(Vector3 position)
        {
            PoolableMonoBehaviour tileGameObject = _tilePool.Get();
            if (tileGameObject.TryGetComponent(out Tile tile))
            {
                int configIndex = _firstSpawn ? 0 : Random.Range(0, _tileConfigs.Length);
                tile.Configure(_tileConfigs[configIndex], position);
                if (position.x == 0)
                    tile.onRelease += OnMiddleTileReleased;
            }
        }

        private void OnMiddleTileReleased(PoolableMonoBehaviour tile)
        {
            tile.onRelease -= OnMiddleTileReleased;
            SpawnTileGroup();
        }
        #endregion
    }
}