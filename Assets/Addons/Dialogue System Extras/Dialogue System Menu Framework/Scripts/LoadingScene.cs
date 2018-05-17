using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script transitions from a loading scene to a gameplay scene. To use it,
    /// set the destination gameplay scene using the static method SetDestinationScene()
    /// or use SetLoadGameData() to load whatever scene is specified in a saved game.
    /// Then load the loading scene. The loading scene should have a GameObject that
    /// has this script.
    /// </summary>
    public class LoadingScene : MonoBehaviour
    {

        public float minDurationToShowLoadingScene = 0;
        public bool hideDialogueManagerCanvases = true;

        public static bool inLoadingScene = false;

        private static int s_sceneBuildIndex = 2;
        private static string s_sceneName = string.Empty;
        private static string s_saveData = string.Empty;

        public static void SetDestinationScene(int sceneBuildIndex)
        {
            s_sceneBuildIndex = sceneBuildIndex;
            s_sceneName = string.Empty;
            s_saveData = string.Empty;
        }

        public static void SetDestinationScene(string sceneName)
        {
            s_sceneBuildIndex = -1;
            s_sceneName = sceneName;
            s_saveData = string.Empty;
        }

        public static void SetLoadGameData(string saveData)
        {
            s_sceneBuildIndex = -1;
            s_sceneName = string.Empty;
            s_saveData = saveData;
        }

        private IEnumerator Start()
        {
            if (hideDialogueManagerCanvases) HideDialogueManagerCanvases();
            if (minDurationToShowLoadingScene > 0) yield return new WaitForSeconds(minDurationToShowLoadingScene);
            inLoadingScene = true;
            var levelManager = FindObjectOfType<LevelManager>();
            if (s_sceneBuildIndex != -1)
            {
                levelManager.LoadLevel(s_sceneBuildIndex);
            }
            else if (!string.IsNullOrEmpty(s_sceneName))
            {
                levelManager.LoadLevel(s_sceneName);
            }
            else
            {
                if (string.IsNullOrEmpty(s_saveData))
                {
                    var mySceneIndex = SceneManager.GetActiveScene().buildIndex;
                    var asyncOp = StartLoadSceneAsync();
                    yield return asyncOp;
#if UNITY_5_5_OR_NEWER
                    SceneManager.UnloadSceneAsync(mySceneIndex);
#else
                    SceneManager.UnloadScene(mySceneIndex);
#endif
                    PersistentDataManager.Apply();
                }
                else
                {
                    levelManager.LoadGame(s_saveData);
                }
            }
        }

        private void OnDestroy()
        {
            inLoadingScene = false;
            if (hideDialogueManagerCanvases) ShowDialogueManagerCanvases();
        }

        private AsyncOperation StartLoadSceneAsync()
        {
            return string.IsNullOrEmpty(s_sceneName)
                ? SceneManager.LoadSceneAsync(s_sceneBuildIndex)
                : SceneManager.LoadSceneAsync(s_sceneName);
        }

        private List<Canvas> hiddenCanvases = new List<Canvas>();

        private void HideDialogueManagerCanvases()
        {
            var dialogueManager = FindObjectOfType<DialogueSystemController>();
            if (dialogueManager == null) return;
            var canvases = dialogueManager.GetComponentsInChildren<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
            {
                var canvas = canvases[i];
                if (canvas.isActiveAndEnabled)
                {
                    canvas.enabled = false;
                    hiddenCanvases.Add(canvas);
                }
            }
        }

        private void ShowDialogueManagerCanvases()
        {
            for (int i = 0; i < hiddenCanvases.Count; i++)
            {
                hiddenCanvases[i].enabled = true;
            }
        }
    }
}