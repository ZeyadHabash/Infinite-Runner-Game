using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniteRunner
{
    public class SpeedLines : MonoBehaviour
    {
        #region Fields

        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            gameObject.SetActive(false);
        }
        #endregion



        #region Public Methods
        public void ToggleSpeedLines()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        #endregion



        #region Private Methods

        #endregion
    }
}
