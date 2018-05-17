using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Add this to the Menu System if you want to manage music tracks.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {

        public AudioSource musicAudioSource;

        public AudioClip titleMusic;
        public AudioClip[] gameplayMusic;

        private void Start()
        {
            if (musicAudioSource == null) musicAudioSource = GetComponent<AudioSource>();
        }

        public void PlayTitleMusic()
        {
            PlayAudioClip(titleMusic);
        }

        public void PlayGameplayMusic(int index)
        {
            if (gameplayMusic == null) return;
            if (0 <= index && index < gameplayMusic.Length)
            {
                PlayAudioClip(gameplayMusic[index]);
            }
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            if (musicAudioSource == null || audioClip == null) return;
            musicAudioSource.Stop();
            musicAudioSource.clip = audioClip;
            musicAudioSource.Play();
        }

        public void StopMusic()
        {
            if (musicAudioSource == null || !musicAudioSource.isPlaying) return;
            musicAudioSource.Stop();
        }

    }
}