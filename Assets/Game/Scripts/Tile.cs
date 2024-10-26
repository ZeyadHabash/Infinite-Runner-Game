using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDK;
using System;

namespace InfiniteRunner
{
    public class Tile : PoolableMonoBehaviour
    {
        [SerializeField] private TileTypeConfig[] _tileTypeConfigs;
        private BoxCollider _collider;
        private Transform _transform;
        public enum TileType
        {
            Normal,
            Empty,
            Obstacle,
            Supplies,
            Burning,
            Boost,
            Sticky
        }

        [System.Serializable]
        public class TileTypeConfig
        {
            public TileType TileType;
            public GameObject Tile;
            public Color color;
        }

        #region Fields

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            _collider = gameObject.GetComponent<BoxCollider>();
            _transform = transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Release();
            }
        }
        #endregion

        #region Public Methods

        public void Configure(TileConfigSO config, Vector3 position)
        {
            _transform.position = position; // set the position of the tile
            foreach (TileType tileType in config.TileTypes)
            {
                foreach (TileTypeConfig tileTypeConfig in _tileTypeConfigs)
                {
                    if (tileType == tileTypeConfig.TileType)
                    {
                        tileTypeConfig.Tile.SetActive(true); // activate the tile

                        Renderer tileRenderer = tileTypeConfig.Tile.GetComponent<Renderer>(); // get the renderer of the tile
                        if (tileRenderer != null) // if the renderer is not null
                            tileRenderer.material.color = tileTypeConfig.color; // set the color of the tile
                    }
                }
            }

        }



        public override void OnObjectPoolReturn()
        {
            // Deactivate all tiles when returning to pool
            foreach (TileTypeConfig tileTypeConfig in _tileTypeConfigs)
            {
                if (tileTypeConfig.Tile != null)
                {
                    tileTypeConfig.Tile.SetActive(false);
                }
            }
        }

        #endregion

        #region Private Methods



        #endregion

    }
}