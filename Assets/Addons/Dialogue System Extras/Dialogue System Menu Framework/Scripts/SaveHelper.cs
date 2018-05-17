using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PixelCrushers.DialogueSystem.MenuSystem
{
    /// <summary>
    /// This script provides utility methods to save, load, and restart games.
    /// 
    /// If you want to record a description with saved games, set the Dialogue System 
    /// variable "CurrentStage". The contents of this variable will be recorded as the
    /// saved game's description.
    /// 
    /// This saves to PlayerPrefs by default. To save elsewhere, assign methods to 
    /// SaveSlotDelegate and LoadSlotDelegate, or override the SaveGame and LoadGameDelayed
    /// methods.
    /// </summary>
    public class SaveHelper : MonoBehaviour
    {
        [Tooltip("Show a separate loading scene when loading saved games.")]
        public bool useLoadingScene = false;

        [Tooltip("If Use Loading Scene is ticked, show this scene as the loading scene.")]
        public int loadingSceneIndex = 1;

        [Tooltip("When starting a new game, load this scene.")]
        public int firstGameplaySceneIndex = 2;

        [Tooltip("Show this text in empty saved game slots.")]
        public string emptySlotText = "-empty-";

        [Tooltip("Panel to show when loading a saved game.")]
        public SelectablePanel loadInProgressPanel;

        [Tooltip("Panel to show when saving a game.")]
        public SelectablePanel saveInProgressPanel;

        [Tooltip("Quick save when this button is pressed.")]
        public string quickSaveButton = string.Empty;

        [Tooltip("Quick load when this button is pressed.")]
        public string quickLoadButton = string.Empty;

        [Tooltip("Add this text to the saved game slot summary, where {0} is the regular slot number text.")]
        public string quickSaveSummaryText = "{0} [QUICK SAVE]";

        public enum SlotFormatInSummary { Omit, CountFrom0, CountFrom1 }

        [Tooltip("How to include the slot number in the saved game summary.")]
        public SlotFormatInSummary slotFormatInSummary = SlotFormatInSummary.CountFrom1;

        [Tooltip("Include the current time in the saved game summary.")]
        public bool includeTimeInSummary = true;

        public int currentSlotNum { get; set; }

        public delegate void SaveSlotDelegate(int slotNum, string saveData);
        public delegate string LoadSlotDelegate(int slotNum);
        public delegate void DeleteSlotDelegate(int slotNum);
        public delegate void RecordExtraSlotDetailsDelegate(int slotNum, ref string summaryInfo);

        public SaveSlotDelegate SaveSlotHandler = null;
        public LoadSlotDelegate LoadSlotHandler = null;
        public DeleteSlotDelegate DeleteSlotHandler = null;
        public RecordExtraSlotDetailsDelegate RecordExtraSlotDetailsHandler = null;

        protected LevelManager levelManager = null;
        protected int slotToLoad = 0;

        protected InputDeviceManager m_inputDeviceManager = null;

        protected virtual void Awake()
        {
            m_inputDeviceManager = GetComponent<InputDeviceManager>();
        }

        protected virtual void Update()
        {
            if (m_inputDeviceManager == null) return;
            if (!string.IsNullOrEmpty(quickSaveButton) && m_inputDeviceManager.GetButtonDown(quickSaveButton))
            {
                QuickSave();
            }
            else if (!string.IsNullOrEmpty(quickLoadButton) && m_inputDeviceManager.GetButtonDown(quickLoadButton))
            {
                QuickLoad();
            }
        }

        protected string GetLastSavedGameKey()
        {
            return "savedgame_lastSlotNum";
        }

        public string GetSlotSummaryKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_summary";
        }

        public string GetSlotDetailsKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_details";
        }

        public string GetSlotDataKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_data";
        }

        public int GetQuickSaveSlot()
        {
            if (!PlayerPrefs.HasKey("savedgame_quickSaveSlot"))
            {
                int slot = 0;
                while (IsGameSavedInSlot(slot))
                {
                    slot++;
                }
                PlayerPrefs.SetInt("savedgame_quickSaveSlot", slot);
            }
            return PlayerPrefs.GetInt("savedgame_quickSaveSlot", 0);
        }

        public virtual bool IsGameSavedInSlot(int slotNum)
        {
            return PlayerPrefs.HasKey(GetSlotDataKey(slotNum));
        }

        public virtual string GetSlotSummary(int slotNum)
        {
            return IsGameSavedInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotSummaryKey(slotNum)) : emptySlotText;
        }

        public virtual string GetSlotDetails(int slotNum)
        {
            return IsGameSavedInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotDetailsKey(slotNum)) : string.Empty;
        }

        public virtual string GetCurrentSummary(int slotNum)
        {
            var summary = GetSlotNumText(slotNum);
            if (includeTimeInSummary)
            {
                if (!string.IsNullOrEmpty(summary)) summary += "\n";
                summary += "Time: " + System.DateTime.Now;
            }
            return summary;
        }

        private string GetSlotNumText(int slotNum)
        {
            switch (slotFormatInSummary)
            {
                case SlotFormatInSummary.CountFrom0:
                    return "Slot " + slotNum;
                case SlotFormatInSummary.CountFrom1:
                    return "Slot " + (slotNum + 1);
                default:
                    return string.Empty;
            }
        }

        public virtual string GetCurrentDetails(int slotNum)
        {
            var details = GetCurrentSummary(slotNum);
            if (DialogueLua.DoesVariableExist("CurrentStage"))
            {
                details += "\n" + DialogueLua.GetVariable("CurrentStage").AsString;
            }
            if (RecordExtraSlotDetailsHandler != null)
            {
                RecordExtraSlotDetailsHandler(slotNum, ref details);
            }
            return details;
        }

        public virtual bool HasLastSavedGame()
        {
            return PlayerPrefs.HasKey(GetLastSavedGameKey());
        }

        public virtual void SaveGame(int slotNum)
        {
            StartCoroutine(SaveGameWithProgressPanel(slotNum));
        }
        
        public virtual void SaveGameNow(int slotNum)
        {
            var saveData = PersistentDataManager.GetSaveData();
            if (SaveSlotHandler == null)
            {
                PlayerPrefs.SetString(GetSlotDataKey(slotNum), saveData);
            }
            else
            {
                PlayerPrefs.SetString(GetSlotDataKey(slotNum), "nil");
                SaveSlotHandler(slotNum, saveData);
            }
            PlayerPrefs.SetString(GetSlotSummaryKey(slotNum), GetCurrentSummary(slotNum));
            PlayerPrefs.SetString(GetSlotDetailsKey(slotNum), GetCurrentDetails(slotNum));
            PlayerPrefs.SetInt(GetLastSavedGameKey(), slotNum);
        }

        protected virtual IEnumerator SaveGameWithProgressPanel(int slotNum)
        {
            ShowSaveInProgress();
            yield return null;
            SaveGameNow(slotNum);
            HideSaveInProgress();
        }

        public void ShowSaveInProgress()
        {
            if (saveInProgressPanel != null) saveInProgressPanel.Open();
        }

        public void HideSaveInProgress()
        {
            if (saveInProgressPanel != null) saveInProgressPanel.Close();
        }

        public virtual void LoadGame(int slotNum)
        {
            StartCoroutine(LoadGameWithProgressPanel(slotNum));
        }

        public virtual void LoadGameNow(int slotNum)
        {
            var saveData = (LoadSlotHandler == null) ? PlayerPrefs.GetString(GetSlotDataKey(slotNum)) : LoadSlotHandler(slotNum);
            if (useLoadingScene)
            {
                PersistentDataManager.LevelWillBeUnloaded();
                LoadingScene.SetLoadGameData(saveData);
                SceneManager.LoadScene(loadingSceneIndex);
            }
            else
            {
                FindLevelManager();
                if (levelManager != null)
                {
                    levelManager.LoadGame(saveData);
                }
                else
                {
                    Lua.Run(saveData, true);
                    PersistentDataManager.LevelWillBeUnloaded();
                    if (DialogueLua.DoesVariableExist("SavedLevelName"))
                    {
                        Tools.LoadLevel(DialogueLua.GetVariable("SavedLevelName").AsString);
                    }
                    else
                    {
                        RestartGame();
                    }
                    PersistentDataManager.ApplySaveData(saveData, DatabaseResetOptions.KeepAllLoaded);
                }
            }
        }

        protected virtual IEnumerator LoadGameWithProgressPanel(int slotNum)
        {
            ShowLoadInProgress();
            yield return null;
            LoadGameNow(slotNum);
            HideLoadInProgress();
        }

        public void ShowLoadInProgress()
        {
            if (loadInProgressPanel != null) loadInProgressPanel.Open();
        }

        public void HideLoadInProgress()
        {
            if (loadInProgressPanel != null) loadInProgressPanel.Close();
        }

        public virtual void LoadLastSavedGame()
        {
            if (HasLastSavedGame())
            {
                LoadGame(PlayerPrefs.GetInt(GetLastSavedGameKey()));
            }
        }

        public virtual void LoadCurrentSlot()
        {
            LoadGame(currentSlotNum);
        }

        public virtual void QuickSave()
        {
            SaveGame(GetQuickSaveSlot());
            Invoke("SetQuickSaveSlotSummaryText", 0.5f);
        }

        protected void SetQuickSaveSlotSummaryText()
        {
            var slotNum = PlayerPrefs.GetInt(GetLastSavedGameKey());
            var slotSummaryKey = GetSlotSummaryKey(slotNum);
            var summary = PlayerPrefs.GetString(slotSummaryKey);
            PlayerPrefs.SetString(slotSummaryKey, string.Format(quickSaveSummaryText, summary));
        }

        public virtual void QuickLoad()
        {
            LoadLastSavedGame();
        }

        public virtual void RestartGame()
        {
            ShowLoadInProgress();
            if (useLoadingScene)
            {
                PersistentDataManager.LevelWillBeUnloaded();
                DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
                LoadingScene.SetDestinationScene(firstGameplaySceneIndex);
                SceneManager.LoadScene(loadingSceneIndex);
            }
            else
            {
                FindLevelManager();
                if (levelManager != null)
                {
                    levelManager.RestartGame();
                }
                else
                {
                    PersistentDataManager.LevelWillBeUnloaded();
                    DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
                    SceneManager.LoadScene(firstGameplaySceneIndex);
                }
            }
        }

        public virtual void LoadLevel(string levelName, int loadingSceneIndex = -1)
        {
            ShowLoadInProgress();
            var loadingSceneIndexToUse = (loadingSceneIndex == -1) ? this.loadingSceneIndex : loadingSceneIndex;
            if (useLoadingScene)
            {
                PersistentDataManager.Record();
                LoadingScene.SetDestinationScene(levelName);
                PersistentDataManager.LevelWillBeUnloaded();
                SceneManager.LoadScene(loadingSceneIndexToUse);
            }
            else
            {
                FindLevelManager();
                if (levelManager != null)
                {
                    levelManager.LoadLevel(levelName);
                }
                else
                {
                    PersistentDataManager.Record();
                    PersistentDataManager.LevelWillBeUnloaded();
                    SceneManager.LoadScene(levelName);
                    PersistentDataManager.Apply();
                }
            }
        }

        public virtual void HandleLevelLoaded(int level)
        {
            HideLoadInProgress();
            DialogueManager.SendUpdateTracker();
        }

        public virtual void DeleteSavedGame(int slotNum)
        {
            PlayerPrefs.DeleteKey(GetSlotDataKey(slotNum));
            PlayerPrefs.DeleteKey(GetSlotSummaryKey(slotNum));
            PlayerPrefs.DeleteKey(GetSlotDetailsKey(slotNum));
            var lastSlotNum = PlayerPrefs.GetInt(GetLastSavedGameKey());
            if (lastSlotNum == slotNum) PlayerPrefs.DeleteKey(GetLastSavedGameKey());
            if (DeleteSlotHandler != null) DeleteSlotHandler(slotNum);
        }

        public virtual void ReturnToTitleMenu()
        {
            DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
            PersistentDataManager.LevelWillBeUnloaded();
            SceneManager.LoadScene(FindObjectOfType<TitleMenu>().titleSceneIndex);
            FindObjectOfType<TitleMenu>().titleMenuPanel.Open();
        }

        protected void FindLevelManager()
        {
            if (levelManager == null) levelManager = FindObjectOfType<LevelManager>();
        }

        public void HaltProgram()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

}