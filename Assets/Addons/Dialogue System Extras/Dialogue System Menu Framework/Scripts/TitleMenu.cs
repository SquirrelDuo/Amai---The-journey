using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Handles the title menu.
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {

        [Tooltip("Index of title scene in build settings.")]
        public int titleSceneIndex = 0;

        [Tooltip("Index of credits scene in build settings.")]
        public int creditsSceneIndex = 2;

        public SelectablePanel titleMenuPanel;
        public UnityEngine.UI.Button startButton;
        public UnityEngine.UI.Button continueButton;
        public UnityEngine.UI.Button restartButton;

        private SaveHelper m_saveHelper;
        private MusicManager m_musicManager;

        private static TitleMenu m_instance = null;

        private void Awake()
        {
            if (m_instance != null)
            {
                Destroy(gameObject);
            }
            m_instance = this;
            if (transform.root != null) transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);
            m_saveHelper = GetComponent<SaveHelper>();
            m_musicManager = GetComponent<MusicManager>();
        }

        private void Start()
        {
            UpdateAvailableButtons();
            if (m_musicManager != null) m_musicManager.PlayTitleMusic();
        }

        public void OnSceneLoaded(int index)
        {
            if (index == titleSceneIndex)
            {
                titleMenuPanel.Open();
                if (InputDeviceManager.deviceUsesCursor) Tools.SetCursorActive(true);
            }
            else
            {
                titleMenuPanel.Close();
            }
        }

        public void UpdateAvailableButtons()
        {
            var hasSavedGame = (m_saveHelper != null) ? m_saveHelper.HasLastSavedGame() : false;
            startButton.gameObject.SetActive(!hasSavedGame);
            continueButton.gameObject.SetActive(hasSavedGame);
            restartButton.gameObject.SetActive(hasSavedGame);
            var selectableToFocus = hasSavedGame ? continueButton.gameObject : startButton.gameObject;
            titleMenuPanel.firstSelected = selectableToFocus;
        }

        public void ShowCreditsScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(creditsSceneIndex);
        }

    }

}