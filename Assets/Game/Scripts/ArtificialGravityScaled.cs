using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniteRunner
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArtificialGravityScaled : MonoBehaviour
    {
        #region Fields
        [SerializeField] private float _gravityScale = 1.0f;
        [SerializeField] private float _golbalGravity = -9.81f;

        private Rigidbody _rigidBody;
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.useGravity = false;
        }
        private void FixedUpdate()
        {
            Vector3 gravity = _golbalGravity * _gravityScale * Vector3.up;
            _rigidBody.AddForce(gravity, ForceMode.Acceleration);

        }
        #endregion



        #region Public Methods

        #endregion



        #region Private Methods

        #endregion
    }
}
