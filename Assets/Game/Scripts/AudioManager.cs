using System.Collections;
using System.Collections.Generic;
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
        #endregion



        #region Properties

        #endregion



        #region Events/Delegates

        #endregion



        #region MonoBehaviour Methods
        private void Start()
        {
            _backgroundMusicAudioSource = PlayBackgroundMusic(_backgroundMusic, transform, _backgroundMusicVolume);
        }
        #endregion



        #region Public Methods

        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1, float length = 1)
        {
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

        #endregion



        #region Private Methods

        #endregion
    }
}
