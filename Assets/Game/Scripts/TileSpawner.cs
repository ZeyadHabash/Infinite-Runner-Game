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
            int tileToBeNormal = Random.Range(0, 3);

            // A tile is chosen to be normal if it is the first spawn
            // or randomly after the first spawn (1/3 chance)
            // Just to make sure that there are normal tiles every row so the game is playable
            // (game is too easy now :( )

            // Spawn Left Tile
            SpawnTile(_spawnPosition + Vector3.left * 5, tileToBeNormal == 0 || _firstSpawn);
            // Spawn Middle Tile
            SpawnTile(_spawnPosition, tileToBeNormal == 1 || _firstSpawn);
            // Spawn Right Tile
            SpawnTile(_spawnPosition + Vector3.right * 5, tileToBeNormal == 2 || _firstSpawn);

            // Update spawn position
            _spawnPosition += Vector3.forward * 5;
        }

        private void SpawnTile(Vector3 position, bool isNormal = false)
        {
            PoolableMonoBehaviour tileGameObject = _tilePool.Get(); // Get a tile from the pool
            if (tileGameObject.TryGetComponent(out Tile tile)) // Get the Tile component from the tileGameObject
            {
                // if isNormal, only spawn normal tiles. else, spawn random tiles
                int configIndex = isNormal ? 0 : Random.Range(0, _tileConfigs.Length);
                tile.Configure(_tileConfigs[configIndex], position); // Configure the tile
                if (position.x == 0) // If the tile is in the middle
                    tile.onRelease += OnMiddleTileReleased; // Subscribe to the onRelease event
            }
        }

        private void OnMiddleTileReleased(PoolableMonoBehaviour tile)
        {
            tile.onRelease -= OnMiddleTileReleased; // Unsubscribe from the onRelease event
            SpawnTileGroup(); // Spawn a new tile group after this tilegroup is released
        }
        #endregion
    }
}