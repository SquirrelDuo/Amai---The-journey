using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script manages the OptionsPanel.
    /// </summary>
    public class Options : MonoBehaviour
    {

        [Header("Video")]
        public UnityEngine.UI.Toggle fullScreenToggle;
        public string fullScreenPrefsKey = "options.fullScreen";
        public UnityEngine.UI.Dropdown resolutionDropdown;
        public string resolutionPrefsKey = "options.resolution";
        public UnityEngine.UI.Dropdown graphicsQualityDropdown;
        public string graphicsQualityPrefsKey = "options.quality";

        [Header("Audio")]
        public AudioMixer mainMixer;
        public string musicVolumeMixerParameter = "musicVol";
        public string musicVolumePrefsKey = "options.musicVol";
        public UnityEngine.UI.Slider musicVolumeSlider;
        public string sfxVolumeMixerParameter = "sfxVol";
        public string sfxVolumePrefsKey = "options.sfxVol";
        public UnityEngine.UI.Slider sfxVolumeSlider;

        [Header("Subtitles")]
        public UnityEngine.UI.Toggle subtitles;
        public bool setNPCSubtitlesDuringLine = true;
        public bool setNPCSubtitlesWithResponseMenu = true;
        public bool setPCSubtitlesDuringLine = false;
        public string subtitlesPrefsKey = "options.subtitles";

        private bool m_started = false;

        private void Start()
        {
            m_started = true;
            RefreshMenuElements();
        }

        private void OnDisable()
        {
            m_started = false;
        }

        public void RefreshMenuElements()
        {
            RefreshResolutionDropdown();
            RefreshFullscreenToggle();
            RefreshGraphicsQualityDropdown();
            RefreshMusicVolumeSlider();
            RefreshSfxVolumeSlider();
            RefreshSubtitlesToggle();
        }

        private void RefreshFullscreenToggle()
        {
            fullScreenToggle.isOn = GetFullScreen();
        }

        private bool GetFullScreen()
        {
            return PlayerPrefs.HasKey(fullScreenPrefsKey) ? (PlayerPrefs.GetInt(fullScreenPrefsKey) == 1) : Screen.fullScreen;
        }

        public void SetFullScreen(bool on)
        {
            Screen.fullScreen = on;
            PlayerPrefs.SetInt(fullScreenPrefsKey, on ? 1 : 0);
        }

        private void RefreshResolutionDropdown()
        {
            if (PlayerPrefs.HasKey(resolutionPrefsKey)) SetResolutionIndex(PlayerPrefs.GetInt(resolutionPrefsKey));
            var list = new List<string>();
            foreach (var resolution in Screen.resolutions)
            {
                list.Add(resolution.width + " x " + resolution.height);
            }
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(list);
            var index = GetResolutionIndex();
            resolutionDropdown.value = index;
            resolutionDropdown.captionText.text = list[index];
        }

        private int GetResolutionIndex()
        {
            if (PlayerPrefs.HasKey(resolutionPrefsKey)) return PlayerPrefs.GetInt(resolutionPrefsKey);
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Screen.resolutions[i].width == Screen.currentResolution.width &&
                    Screen.resolutions[i].height == Screen.currentResolution.height) return i;
            }
            return 0;
        }

        public void SetResolutionIndex(int index)
        {
            if (!(0 <= index && index < Screen.resolutions.Length)) return;
            var resolution = Screen.resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, GetFullScreen());
            PlayerPrefs.SetInt(resolutionPrefsKey, index);
        }

        private void RefreshGraphicsQualityDropdown()
        {
            if (PlayerPrefs.HasKey(graphicsQualityPrefsKey)) SetGraphicsQualityIndex(PlayerPrefs.GetInt(graphicsQualityPrefsKey));
            var list = new List<string>(QualitySettings.names);
            graphicsQualityDropdown.ClearOptions();
            graphicsQualityDropdown.AddOptions(list);
            var index = GetGraphicsQualityIndex();
            graphicsQualityDropdown.value = index;
            graphicsQualityDropdown.captionText.text = list[index];
        }

        private int GetGraphicsQualityIndex()
        {
            return PlayerPrefs.HasKey(graphicsQualityPrefsKey) ? PlayerPrefs.GetInt(graphicsQualityPrefsKey) : QualitySettings.GetQualityLevel();
        }

        public void SetGraphicsQualityIndex(int index)
        {
            QualitySettings.SetQualityLevel(index);
            PlayerPrefs.SetInt(graphicsQualityPrefsKey, index);
        }

        private void RefreshMusicVolumeSlider()
        {
            if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat(musicVolumePrefsKey, 0);
        }

        public void SetMusicLevel(float musicLevel)
        {
            if (!m_started) return;
            if (mainMixer != null) mainMixer.SetFloat(musicVolumeMixerParameter, musicLevel);
            PlayerPrefs.SetFloat(musicVolumePrefsKey, musicLevel);
        }

        private void RefreshSfxVolumeSlider()
        {
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxVolumeMixerParameter, 0);
        }

        public void SetSfxLevel(float sfxLevel)
        {
            if (!m_started) return;
            if (mainMixer != null) mainMixer.SetFloat(sfxVolumeMixerParameter, sfxLevel);
            PlayerPrefs.SetFloat(sfxVolumePrefsKey, sfxLevel);
        }

        private void RefreshSubtitlesToggle()
        {
            subtitles.isOn = PlayerPrefs.GetInt(subtitlesPrefsKey, GetDefaultSubtitlesSetting() ? 1 : 0) == 1;
        }

        public void OnSubtitlesToggleChanged()
        {
            if (!m_started) return;
            SetSubtitles(subtitles.isOn);
        }

        public void SetSubtitles(bool on)
        {
            var subtitleSettings = DialogueManager.DisplaySettings.subtitleSettings;
            subtitleSettings.showNPCSubtitlesDuringLine = subtitles.isOn && setNPCSubtitlesDuringLine;
            subtitleSettings.showNPCSubtitlesWithResponses = subtitles.isOn && setNPCSubtitlesWithResponseMenu;
            subtitleSettings.showPCSubtitlesDuringLine = subtitles.isOn && setPCSubtitlesDuringLine;
            PlayerPrefs.SetInt(subtitlesPrefsKey, on ? 1 : 0);
        }

        private bool GetDefaultSubtitlesSetting()
        {
            var subtitleSettings = DialogueManager.DisplaySettings.subtitleSettings;
            return subtitleSettings.showNPCSubtitlesDuringLine || subtitleSettings.showNPCSubtitlesWithResponses || subtitleSettings.showPCSubtitlesDuringLine;
        }
    }
}