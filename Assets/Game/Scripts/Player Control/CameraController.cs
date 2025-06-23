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
        [SerializeField] private float _baseFOVValue = 60;
        [SerializeField] private float _wideFOVValue = 80;
        [SerializeField] private float _fovLerpDuration = 0.5f;
        private Coroutine _currentFOVCoroutine;
        private bool _wideFOV = false;
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            Camera.main.fieldOfView = _baseFOVValue;
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
        public void ToggleFOV()
        {
            if (_currentFOVCoroutine != null)
            {
                StopCoroutine(_currentFOVCoroutine);
            }

            float targetFOV = _wideFOV ? _baseFOVValue : _wideFOVValue;
            _currentFOVCoroutine = StartCoroutine(LerpFOV(targetFOV, _fovLerpDuration));
            _wideFOV = !_wideFOV;
        }
        #endregion



        #region Private Methods
        private IEnumerator LerpFOV(float targetFOV, float duration)
        {
            float startFOV = Camera.main.fieldOfView;
            float time = 0;
            while (time < duration)
            {
                Camera.main.fieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.fieldOfView = targetFOV;
            _currentFOVCoroutine = null;
        }
        #endregion
    }
}
