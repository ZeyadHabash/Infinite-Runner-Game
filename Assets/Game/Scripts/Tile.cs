using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Fields
    [SerializeField] private TileType _tileType;
    [SerializeField] private int _destroyDelay = 2;
    private Rigidbody _rigidbody;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    #endregion

    #region Public Methods
    public void InitializeTile(TileType tileType)
    {
        SetTileType(tileType);
    }
    public void DestroyTile()
    {
        StartCoroutine(DestroyTileRoutine());
    }
    public void OnTileEntered()
    {
        _tileType.OnTileEntered();
    }
    #endregion

    #region Private Methods
    private IEnumerator DestroyTileRoutine()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }
    private void SetTileType(TileType tileType)
    {
        _tileType = tileType;
        GetComponent<Renderer>().material.color = tileType.TileColor;
    }
    #endregion

}
