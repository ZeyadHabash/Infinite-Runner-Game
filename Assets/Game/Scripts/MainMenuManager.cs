using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniteRunner
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Fields

        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods

        #endregion



        #region Public Methods
        public void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion



        #region Private Methods

        #endregion
    }
}
