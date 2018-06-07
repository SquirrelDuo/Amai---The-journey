using System.Collections.Generic;
using UnityEngine;

//Class controlling checkpoints
public class Checkpoint_Manager : MonoBehaviour
{
    //Variables.
    public GameObject player; //A field for the player object.
    public GameObject[] checkpoints; //A list with all checkpoints.
    public bool isDead; //Bool triggered by falling.
    public List<int> checkpointValues;
    public int listSize; //Value that keeps track of checkpoints.
    public List<int> tempList; //Temporary list for our values.
    public GameObject fadeObject; //Object we modify for fading.

    private void Start()
    {
        //We assign ID number to each checkpoint.
        for (int i = 0; i <= checkpoints.Length - 1; i++)
        {
            checkpoints[i].GetComponent<Checkpoint>().pointID = i;
        }

        listSize = checkpoints.Length - 1;
        fadeObject.SetActive(false);
    }

    private void Update()
    {
        //If we detect falling.
        if (isDead)
        {
            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            FadeStart();
            Invoke("ReturnToCheckpoint", 1.2f);
        }
    }

    //Method we call when we trigger falling.
    public void ReturnToCheckpoint()
    {
        for(int i = 0; i <= checkpoints.Length - 1; i++)
        {
            //If we find an activated checkpoint.
            if(checkpoints[i].GetComponent<Checkpoint>().isRightForSaving == 1)
            {
                //If the checkpoint is one with breakable platforms.
                if (checkpoints[i].GetComponent<Checkpoint>().isSpecific)
                {
                    checkpoints[i].GetComponent<Checkpoint>().SpecificTreat();

                    player.transform.position = checkpoints[i].GetComponent<Checkpoint>().positionToMove.transform.position; //The player takes the last checkpoint's position.
                    isDead = false;
                }
                //If the checkpoint is one with lighting wall.
                else if (checkpoints[i].GetComponent<Checkpoint>().isLightingWall)
                {
                    checkpoints[i].GetComponent<Checkpoint>().LightingWall();

                    player.transform.position = checkpoints[i].GetComponent<Checkpoint>().positionToMove.transform.position; //The player takes the last checkpoint's position.
                    isDead = false;
                }
                //If the checkpoint is one with rolling ball.
                else if (checkpoints[i].GetComponent<Checkpoint>().isRollingBall)
                {
                    checkpoints[i].GetComponent<Checkpoint>().RollingBall();

                    player.transform.position = checkpoints[i].GetComponent<Checkpoint>().positionToMove.transform.position; //The player takes the last checkpoint's position.
                    isDead = false;
                }
                //If the checkpoint has falling platforms.
                else if (checkpoints[i].GetComponent<Checkpoint>().fallingPlatforms)
                {
                    checkpoints[i].GetComponent<Checkpoint>().FallingPlatforms();

                    player.transform.position = checkpoints[i].GetComponent<Checkpoint>().positionToMove.transform.position; //The player takes the last checkpoint's position.
                    isDead = false;
                }
                //If the checkpoint is ordinary.
                else
                {
                    player.transform.position = checkpoints[i].GetComponent<Checkpoint>().positionToMove.transform.position; //The player takes the last checkpoint's position.
                    isDead = false;
                }
            }
        }
    }

    //We add these values to a list.
    public void CheckpointsList()
    {
        if(checkpointValues.Count == 0)
        {
            for (int i = 0; i <= checkpoints.Length - 1; i++)
            {
                checkpointValues.Add(checkpoints[i].GetComponent<Checkpoint>().isRightForSaving);
            }
        } else
        {
            checkpointValues.Clear();
            for (int i = 0; i <= checkpoints.Length - 1; i++)
            {
                checkpointValues.Add(checkpoints[i].GetComponent<Checkpoint>().isRightForSaving);
            }
        }
    }

    void FadeStart()
    {
        fadeObject.SetActive(true);
        Invoke("FadeEnd", 3);
    }

    void FadeEnd()
    {
        fadeObject.SetActive(false);
    }
}