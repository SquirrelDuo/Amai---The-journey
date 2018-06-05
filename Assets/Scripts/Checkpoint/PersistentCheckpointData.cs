using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

public class PersistentCheckpointData : MonoBehaviour
{
    //Variables.
    public Checkpoint_Manager checkpointManager;
    public Open_Gate gateManager;
    public Variables_To_Save variablesToSave;

    public void OnRecordPersistentData()
    {
        checkpointManager.CheckpointsList();

        for (int i = 0; i <= checkpointManager.checkpoints.Length - 1; i++)
        {
            DialogueLua.SetVariable("CheckpointValue" + i, checkpointManager.checkpointValues[i]);
        }

        DialogueLua.SetVariable("ListSize", checkpointManager.listSize);

        //Player prefab and position.
        DialogueLua.SetVariable("CurrentPlayerPrefab", variablesToSave.currentPlayerPrefab);

        //Skybox.
        DialogueLua.SetVariable("CurrentSkybox", variablesToSave.currentSkybox);

        //Light source.
        DialogueLua.SetVariable("CurrentLight", variablesToSave.currentLight);

        //Fall Trigger.
        DialogueLua.SetVariable("CurrentFallTriggers", variablesToSave.currentFallTriggers);

        //Values from section 2.
        DialogueLua.SetVariable("secondStopDissolve", variablesToSave.secondDisplace.stopDissolveSectionTwo);

        //Bools for section 6.
        DialogueLua.SetVariable("LeftGate", gateManager.firstOpen);
        DialogueLua.SetVariable("RightGate", gateManager.secondOpen);

        //Bools for section 7.
        DialogueLua.SetVariable("isLightingTriggered", variablesToSave.lightingIsTriggered);

        //Record lighting wall position in Section 9.
        variablesToSave.GetAnimationTime();
        DialogueLua.SetVariable("LightingWallAnimationTime", variablesToSave.lightingWallAnimationTime);

        //Values from section 11.
        DialogueLua.SetVariable("CylinderFull", variablesToSave.cylinderIsFull);
        variablesToSave.RollingAnimationTimeGet();
        DialogueLua.SetVariable("RollingBallAnimationTime", variablesToSave.rollingBallAnimationTime);
        DialogueLua.SetVariable("isRolling", variablesToSave.isRolling);

        //Collectables from Section 14.
        DialogueLua.SetVariable("BoolOne", variablesToSave.collectorManager.areTaken[0]);
        DialogueLua.SetVariable("BoolTwo", variablesToSave.collectorManager.areTaken[1]);
        DialogueLua.SetVariable("BoolThree", variablesToSave.collectorManager.areTaken[2]);
        DialogueLua.SetVariable("BoolFour", variablesToSave.collectorManager.areTaken[3]);
        DialogueLua.SetVariable("BoolFive", variablesToSave.collectorManager.areTaken[4]);
        DialogueLua.SetVariable("BoolSix", variablesToSave.collectorManager.areTaken[5]);

        //Enviro Symbols from Section 16.
        DialogueLua.SetVariable("KeyOne", variablesToSave.reviveManager.keyBools[0]);
        DialogueLua.SetVariable("KeyTwo", variablesToSave.reviveManager.keyBools[1]);
        DialogueLua.SetVariable("KeyThree", variablesToSave.reviveManager.keyBools[2]);

        //Spirit Animals from Section 16.
        if(variablesToSave.spiritObjects[0] != null) { variablesToSave.spiritStatus[0] = false; }
        else if(variablesToSave.spiritObjects[0] == null) { variablesToSave.spiritStatus[0] = true; }
        DialogueLua.SetVariable("SpiritOne", variablesToSave.spiritStatus[0]);

        if (variablesToSave.spiritObjects[1] != null) { variablesToSave.spiritStatus[1] = false; }
        else if (variablesToSave.spiritObjects[1] == null) { variablesToSave.spiritStatus[1] = true; }
        DialogueLua.SetVariable("SpiritTwo", variablesToSave.spiritStatus[1]);

        if (variablesToSave.spiritObjects[2] != null) { variablesToSave.spiritStatus[2] = false; }
        else if (variablesToSave.spiritObjects[2] == null) { variablesToSave.spiritStatus[2] = true; }
        DialogueLua.SetVariable("SpiritThree", variablesToSave.spiritStatus[2]);

        if (variablesToSave.spiritObjects[3] != null) { variablesToSave.spiritStatus[3] = false; }
        else if (variablesToSave.spiritObjects[3] == null) { variablesToSave.spiritStatus[3] = true; }
        DialogueLua.SetVariable("SpiritFour", variablesToSave.spiritStatus[3]);

        if (variablesToSave.spiritObjects[4] != null) { variablesToSave.spiritStatus[4] = false; }
        else if (variablesToSave.spiritObjects[4] == null) { variablesToSave.spiritStatus[4] = true; }
        DialogueLua.SetVariable("SpiritFour", variablesToSave.spiritStatus[4]);
    }

    public void OnApplyPersistentData()
    {
        checkpointManager.listSize = DialogueLua.GetVariable("ListSize").AsInt;

        for (int j = 0; j <= checkpointManager.listSize; j++)
        {
            checkpointManager.tempList.Add(j);
        }

        if(checkpointManager.checkpointValues.Count == 0)
        {
            checkpointManager.checkpointValues.InsertRange(0, checkpointManager.tempList);

            for (int i = 0; i <= checkpointManager.checkpoints.Length - 1; i++)
            {
                checkpointManager.checkpointValues[i] = DialogueLua.GetVariable("CheckpointValue" + i).AsInt;
            }

            for (int k = 0; k <= checkpointManager.checkpoints.Length - 1; k++)
            {
                checkpointManager.checkpoints[k].GetComponent<Checkpoint>().isRightForSaving = checkpointManager.checkpointValues[k];
            }
        }
        else
        {
            for (int i = 0; i <= checkpointManager.checkpoints.Length - 1; i++)
            {
                checkpointManager.checkpointValues[i] = DialogueLua.GetVariable("CheckpointValue" + i).AsInt;
            }

            for (int k = 0; k <= checkpointManager.checkpoints.Length - 1; k++)
            {
                checkpointManager.checkpoints[k].GetComponent<Checkpoint>().isRightForSaving = checkpointManager.checkpointValues[k];
            }
        }

        //Player position and prefab.
        variablesToSave.currentPlayerPrefab = DialogueLua.GetVariable("CurrentPlayerPrefab").AsInt;

        //Skybox.
        variablesToSave.currentSkybox = DialogueLua.GetVariable("CurrentSkybox").AsInt;
        variablesToSave.AdjustSkybox();

        //Light source.
        variablesToSave.currentLight = DialogueLua.GetVariable("CurrentLight").AsInt;
        variablesToSave.AdjustLight();

        //Fall triggers.
        variablesToSave.currentFallTriggers = DialogueLua.GetVariable("CurrentFallTriggers").AsInt;
        variablesToSave.AdjustFallTriggers();

        //Values from section 2.
        variablesToSave.secondDisplace.stopDissolveSectionTwo = DialogueLua.GetVariable("secondStopDissolve").AsBool;
        variablesToSave.CheckDisplaceSectionTwo();

        //Values from section 6.
        gateManager.firstOpen = DialogueLua.GetVariable("LeftGate").AsBool;
        gateManager.secondOpen = DialogueLua.GetVariable("RightGate").AsBool;
        if (gateManager.firstOpen) { variablesToSave.particleZoneOne.SetActive(false); }
        if (gateManager.secondOpen) { variablesToSave.particleZoneTwo.SetActive(false); }

        //Values from section 7.
        variablesToSave.lightingIsTriggered = DialogueLua.GetVariable("isLightingTriggered").AsBool;
        variablesToSave.CheckLightingTrigger();

        //Record lighting wall position in Section 9.
        variablesToSave.lightingWallAnimationTime = DialogueLua.GetVariable("LightingWallAnimationTime").AsFloat;
        variablesToSave.SetAnimationTime();

        //Values from section 11.
        variablesToSave.cylinderIsFull = DialogueLua.GetVariable("CylinderFull").AsBool;
        variablesToSave.CheckSectionEleven();

        variablesToSave.isRolling = DialogueLua.GetVariable("isRolling").AsBool;

        if (variablesToSave.isRolling)
        {
            variablesToSave.rollingBall.SetActive(true);
            variablesToSave.rollingBallAnimationTime = DialogueLua.GetVariable("RollingBallAnimationTime").AsFloat;
            variablesToSave.SetAnimationTimeRollingBall();
        }

        //Collectables from Section 14.
        variablesToSave.collectorManager.areTaken[0] = DialogueLua.GetVariable("BoolOne").AsBool;
        variablesToSave.collectorManager.areTaken[1] = DialogueLua.GetVariable("BoolTwo").AsBool;
        variablesToSave.collectorManager.areTaken[2] = DialogueLua.GetVariable("BoolThree").AsBool;
        variablesToSave.collectorManager.areTaken[3] = DialogueLua.GetVariable("BoolFour").AsBool;
        variablesToSave.collectorManager.areTaken[4] = DialogueLua.GetVariable("BoolFive").AsBool;
        variablesToSave.collectorManager.areTaken[5] = DialogueLua.GetVariable("BoolSix").AsBool;
        variablesToSave.CheckCollectables();

        //Enviro Symbols from Section 16.
        variablesToSave.reviveManager.keyBools[0] = DialogueLua.GetVariable("KeyOne").AsBool;
        variablesToSave.reviveManager.keyBools[1] = DialogueLua.GetVariable("KeyTwo").AsBool;
        variablesToSave.reviveManager.keyBools[2] = DialogueLua.GetVariable("KeyThree").AsBool;
        variablesToSave.EnviroSymbols();

        //Spirit Animals from Section 16.
        variablesToSave.spiritStatus[0] = DialogueLua.GetVariable("SpiritOne").AsBool;
        variablesToSave.spiritStatus[1] = DialogueLua.GetVariable("SpiritTwo").AsBool;
        variablesToSave.spiritStatus[2] = DialogueLua.GetVariable("SpiritThree").AsBool;
        variablesToSave.spiritStatus[3] = DialogueLua.GetVariable("SpiritFour").AsBool;
        variablesToSave.spiritStatus[4] = DialogueLua.GetVariable("SpiritFive").AsBool;
        variablesToSave.SpiritSetup();
    }

    public void OnEnable()
    {
        // This optional code registers this GameObject with the PersistentDataManager.
        // One of the options on the PersistentDataManager is to only send notifications
        // to registered GameObjects. The default, however, is to send to all GameObjects.
        // If you set PersistentDataManager to only send notifications to registered
        // GameObjects, you need to register this component using the line below or it
        // won't receive notifications to save and load.
        PersistentDataManager.RegisterPersistentData(this.gameObject);
    }

    public void OnDisable()
    {
        // Unsubscribe the GameObject from PersistentDataManager notifications:
        PersistentDataManager.RegisterPersistentData(this.gameObject);
    }

    //--- Uncomment this method if you want to implement it:
    //public void OnLevelWillBeUnloaded() 
    //{
    // This will be called before loading a new level. You may want to add code here
    // to change the behavior of your persistent data script. For example, the
    // IncrementOnDestroy script disables itself because it should only increment
    // the variable when it's destroyed during play, not because it's being
    // destroyed while unloading the old level.
    //}
}