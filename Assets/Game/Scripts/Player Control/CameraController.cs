using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniteRunner
{
    public class CameraController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Vector3 _offset = Vector3.back * 10;
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            if (_playerTransform == null)
            {
                _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        private void LateUpdate()
        {
            transform.position = _playerTransform.position + _offset;
            transform.LookAt(_playerTransform);
        }
        #endregion



        #region Public Methods

        #endregion



        #region Private Methods

        #endregion
    }
}
