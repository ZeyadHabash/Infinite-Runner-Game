using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
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
        [SerializeField] private TMP_Text _playerSpeedText;
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
        [SerializeField] private float _initialFuel = 50;
        private float _currentScore;
        private float _currentFuel;

        public float Score => _currentScore;
        private int _scoreIncreaseRate = 1;

        [Header("SOAP")]
        [SerializeField] private IntVariable _fuelDecreaseRate;
        [SerializeField] private StringVariable _playerSpeed;
        [SerializeField] private BoolVariable _isGameOver;
        [SerializeField] private BoolVariable _isPaused;
        [SerializeField] private ScriptableEventNoParam _onGameOver;

        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods

        private void OnEnable()
        {
            _playerSpeed.OnValueChanged += OnPlayerSpeedChanged;
        }

        private void OnDisable()
        {
            _playerSpeed.OnValueChanged -= OnPlayerSpeedChanged;
        }

        private void Start()
        {
            _fuelDecreaseRate.Value = 1;
            // Initialize the score and fuel
            ResetFuel();
            ResetScore();

            // Initialize the score and fuel text
            _scoreText.SetText(_scoreText.text, _initialScore);
            _fuelText.SetText(_fuelText.text, _initialFuel);
            _playerSpeedText.SetText($"Speed: {_playerSpeed.Value}");

            // Initialize selected buttons in event system
            EventSystem.current.SetSelectedGameObject(null);

            // Reset the isGameOver and isPaused variables
            _isPaused.Value = false;
            _isGameOver.Value = false;
        }

        private void Update()
        {
            if (_isPaused || _isGameOver)
                return;
            IncreaseScore(_scoreIncreaseRate * Time.deltaTime);
            DecreaseFuel(_fuelDecreaseRate * Time.deltaTime);
            if (_currentFuel <= 0)
            {
                GameOver();
            }
        }
        #endregion



        #region Public Methods

        public void PauseGame()
        {
            if (_isGameOver)
                return;
            AudioManager.Instance.SwitchBackgroundMusic();
            if (_isPaused)
            {
                Time.timeScale = 1;
                _pauseCanvas.gameObject.SetActive(false);
                _isPaused.Value = false;
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                Time.timeScale = 0;
                _pauseCanvas.gameObject.SetActive(true);
                _isPaused.Value = true;
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
            _isGameOver.Value = true;

            EventSystem.current.SetSelectedGameObject(_gameOverFirstSelectedButton);
            _finalScoreText.SetText(_finalScoreText.text, _currentScore);
            AudioManager.Instance.SwitchBackgroundMusic();
            _onGameOver.Raise();
        }

        public void IncreaseScore(float score)
        {
            _currentScore += score;
            _scoreText.SetText($"Score: {_currentScore:F0}");
        }

        public void DecreaseFuel(float fuel)
        {
            _currentFuel -= fuel;
            _fuelText.SetText($"Fuel: {_currentFuel:F0}");
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            _isGameOver.Value = false;
            ResetFuel();
            ResetScore();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        public void ResetFuel()
        {
            _currentFuel = _initialFuel;
            _fuelText.SetText($"Fuel: {_currentFuel}");
        }

        #endregion



        #region Private Methods

        private void ResetScore()
        {
            _currentScore = _initialScore;
            _scoreText.SetText($"Score: {_currentScore}");
        }

        private void OnPlayerSpeedChanged(string speed)
        {
            _playerSpeedText.SetText($"Speed: {speed}");
        }

        #endregion
    }
}
