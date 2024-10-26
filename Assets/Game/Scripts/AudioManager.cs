using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

namespace InfiniteRunner
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region Fields
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private float _backgroundMusicVolume = 1;
        private AudioSource _backgroundMusicAudioSource;

        [SerializeField] private BoolVariable _isMuted;
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            if (!_isMuted)
                _backgroundMusicAudioSource = PlayBackgroundMusic(_backgroundMusic, transform, _backgroundMusicVolume);
        }

        private void OnEnable()
        {
            _isMuted.OnValueChanged += OnMute;
        }
        private void OnDisable()
        {
            _isMuted.OnValueChanged -= OnMute;
        }
        #endregion



        #region Public Methods

        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1, float length = 1)
        {
            if (_isMuted)
            {
                return;
            }
            // Create a new GameObject
            AudioSource audioSource = Instantiate(_audioSourcePrefab, spawnTransform.position, Quaternion.identity);

            // Assign the audio clip to the AudioSource component
            audioSource.clip = audioClip;

            // Set the volume of the AudioSource component
            audioSource.volume = volume;

            // Play the audio clip
            audioSource.Play();

            // Get the length of the audio clip
            float audioClipLength = audioClip.length > 1 ? length : audioClip.length;

            // Destroy the GameObject after the length of the audio clip
            Destroy(audioSource.gameObject, audioClipLength);
        }

        public AudioSource PlayBackgroundMusic(AudioClip audioClip, Transform spawnTransform, float volume = 1)
        {
            if (_isMuted)
            {
                return null;
            }
            // Create a new GameObject
            AudioSource audioSource = Instantiate(_audioSourcePrefab, spawnTransform.position, Quaternion.identity);

            // Assign the audio clip to the AudioSource component
            audioSource.clip = audioClip;

            // Set the volume of the AudioSource component
            audioSource.volume = volume;

            // Set the loop property of the AudioSource component to true
            audioSource.loop = true;

            // Play the audio clip
            audioSource.Play();

            return audioSource;
        }

        public void PauseBackgroundMusic()
        {
            _backgroundMusicAudioSource.Pause();
        }
        public void ResumeBackgroundMusic()
        {
            _backgroundMusicAudioSource.Play();
        }
        public void StopBackgroundMusic()
        {
            _backgroundMusicAudioSource.Stop();
        }
        public void StartBackgroundMusic()
        {
            _backgroundMusicAudioSource.Play();
        }
        public void SetBackgroundMusicVolume(float volume)
        {
            _backgroundMusicAudioSource.volume = volume;
        }
        public void ResetBackgroundMusicVolume()
        {
            _backgroundMusicAudioSource.volume = _backgroundMusicVolume;
        }

        public void OnMute(bool isMuted)
        {
            if (isMuted)
            {
                StopAllAudio();
            }
            else
            {
                StartBackgroundMusic();
            }
        }
        public void StopAllAudio()
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }
        #endregion



        #region Private Methods

        #endregion
    }
}
