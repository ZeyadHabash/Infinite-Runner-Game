using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfiniteRunner
{
    public class GameManager : Singleton<GameManager>
    {
        #region Fields
        [Header("TMP")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _fuelText;
        [SerializeField] private TMP_Text _finalScoreText;

        [Header("Canvas")]
        [SerializeField] private Canvas _gameOverCanvas;
        [SerializeField] private Canvas _hudCanvas;
        [SerializeField] private Canvas _pauseCanvas;

        [Header("First Selected Buttons")]
        [SerializeField] private GameObject _pauseFirstSelectedButton;
        [SerializeField] private GameObject _gameOverFirstSelectedButton;


        [Header("Resources")]
        [SerializeField] private int _initialScore = 0;
        [SerializeField] private int _initialFuel = 50;
        [SerializeField] private int _currentScore = 0;
        [SerializeField] private int _currentFuel = 50;

        private int _fuelDecreaseRate = 1;
        private int _scoreIncreaseRate = 1;

        private bool _isGameOver = false;
        private bool _isPaused = false;

        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods

        private IEnumerator Start()
        {
            // Initialize the score and fuel text
            _scoreText.SetText(_scoreText.text, _initialScore);
            _fuelText.SetText(_fuelText.text, _initialFuel);
            EventSystem.current.SetSelectedGameObject(null);

            while (!_isGameOver)
            {
                yield return new WaitForSeconds(1);
                IncreaseScore(_scoreIncreaseRate);
                DecreaseFuel(_fuelDecreaseRate);
                if (_currentFuel <= 0)
                {
                    GameOver();
                }
            }
        }

        #endregion



        #region Public Methods

        public void PauseGame()
        {
            if (_isGameOver)
                return;
            if (_isPaused)
            {
                Time.timeScale = 1;
                _pauseCanvas.gameObject.SetActive(false);
                _isPaused = false;
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                Time.timeScale = 0;
                _pauseCanvas.gameObject.SetActive(true);
                _isPaused = true;
                EventSystem.current.SetSelectedGameObject(_pauseFirstSelectedButton);
            }

        }
        public void GameOver()
        {
            if (_isGameOver)
                return;

            Time.timeScale = 0; // Pause the game
            _gameOverCanvas.gameObject.SetActive(true);
            _hudCanvas.gameObject.SetActive(false);
            _isGameOver = true;

            EventSystem.current.SetSelectedGameObject(_gameOverFirstSelectedButton);
            _finalScoreText.SetText(_finalScoreText.text, _currentScore);
        }

        public void IncreaseScore(int score)
        {
            _currentScore += score;
            _scoreText.SetText($"Score: {_currentScore}");
        }

        public void DecreaseFuel(int fuel)
        {
            _currentFuel -= fuel;
            _fuelText.SetText($"Fuel: {_currentFuel}");
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            _isGameOver = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        #endregion



        #region Private Methods

        private void ResetScore()
        {
            _currentScore = _initialScore;
            _scoreText.SetText($"Score: {_currentScore}");
        }

        private void ResetFuel()
        {
            _currentFuel = _initialFuel;
            _fuelText.SetText($"Fuel: {_currentFuel}");
        }

        #endregion
    }
}
