using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem.MenuSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// This script implements the sequencer command LoadingSceneTo(levelName, [loadingSceneIndex], [spawnpoint]).
    /// 
    /// - levelName: The destination level.
    /// - loadingSceneIndex: If specified and not -1, loads it as a loading scene with the final destination
    /// set to levelName. If omitted or -1, goes directly to levelName. Default: -1.
    /// - spawnpoint: Tells the player's PersistentPositionData component to move the player to the 
    /// location of this GameObject in the destination level. Assumes the Player actor is named "Player".
    /// </summary>
    public class SequencerCommandLoadingSceneTo : SequencerCommand
    {

        public void Start()
        {
            var levelName = GetParameter(0);
            var loadingSceneIndex = GetParameterAsInt(1, -1);
            var spawnpoint = GetParameter(2);
            if (string.IsNullOrEmpty(levelName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: LoadingSceneTo(" + GetParameters() + ") level name is an empty string");
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: LoadingSceneTo(" + levelName + ", " + loadingSceneIndex + ", " + spawnpoint + ")");
                DialogueLua.SetActorField("Player", "Spawnpoint", spawnpoint);
                var saveHelper = FindObjectOfType<SaveHelper>();
                if (saveHelper != null)
                {
                    saveHelper.LoadLevel(levelName, loadingSceneIndex);
                }
                else
                {
                    PersistentDataManager.LevelWillBeUnloaded();
                    LoadingScene.SetDestinationScene(levelName);
                    SceneManager.LoadScene(loadingSceneIndex);
                }
            }
            Stop();
        }
    }
}
