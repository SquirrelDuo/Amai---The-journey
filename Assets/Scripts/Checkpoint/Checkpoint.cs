using UnityEngine;

//Class, attached to each checkpoint.
public class Checkpoint : MonoBehaviour
{
    public Checkpoint_Manager checkpointManager; //Field for the scene's checkpoint manager.
    public bool isActive { get; set; } //Checkpoint boolean.
    public int pointID { get; set; } //Every checkpoint should have an ID.
    public GameObject positionToMove; //The position that the player will take.

    private Variables_To_Save persistentValues;

    [Header("Breakable Platforms")]
    public bool isSpecific;
    public GameObject[] specificObjects;

    [Header("Section 9")]
    public bool isLightingWall;

    [Header("Section 12")]
    public bool isRollingBall;

    public int isRightForSaving = 0; //Default should be 0;

    public void ActivateCheckpoint()
    {
        isActive = true;
        isRightForSaving = 1;

        //We deactivate all the previous activated checkpoints.
        for (int i = 0; i <= checkpointManager.checkpoints.Length - 1; i++)
        {
            if (checkpointManager.checkpoints[i].GetComponent<Checkpoint>().pointID
                    != gameObject.GetComponent<Checkpoint>().pointID)
            {
                checkpointManager.checkpoints[i].GetComponent<Checkpoint>().isActive = false;
                checkpointManager.checkpoints[i].GetComponent<Checkpoint>().isRightForSaving = 0;
            }
        }
    }

    //Reseting breakable platforms.
    public void SpecificTreat()
    {
        for (int j = 0; j <= specificObjects.Length - 1; j++)
        {
            specificObjects[j].GetComponent<SpecificObjects>().ResetObject();
        }
    }

    //Reseting lighting wall position.
    public void LightingWall()
    {
        persistentValues = GameObject.Find("Persistent_Values").GetComponent<Variables_To_Save>();
        persistentValues.lightingWall.GetComponent<Animation>()["Pressure"].time = 0;
    }

    //Reseting the rolling ball.
    public void RollingBall()
    {
        persistentValues = GameObject.Find("Persistent_Values").GetComponent<Variables_To_Save>();
        persistentValues.rollingBall.SetActive(false);

        persistentValues.rollingBall.GetComponent<Animation>()["Rolling_Ball"].time = 0;
    }
}