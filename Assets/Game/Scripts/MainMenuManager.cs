using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace InfiniteRunner
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Fields
        [SerializeField] private GameObject _playButton;
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_playButton);
        }
        #endregion



        #region Public Methods
        public void StartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
        public void QuitGame()
        {
            Application.Quit();
        }
        public void SetFirstSelectedButton(GameObject button)
        {
            EventSystem.current.SetSelectedGameObject(button);
        }
        #endregion


        #region Private Methods

        #endregion
    }
}
