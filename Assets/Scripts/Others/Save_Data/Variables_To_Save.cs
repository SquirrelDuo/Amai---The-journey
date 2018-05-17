using UnityEngine;
using MalbersAnimations;
using System.Collections;

//Class we use to save/load different variables and objects in the scene.
public class Variables_To_Save : MonoBehaviour
{
    //Variables.
    [Header("Player position and prefabs")]
    public GameObject[] playerPrefabs;
    public int currentPlayerPrefab;
    public GameObject[] possiblePlayerPositions;
    public bool[] isCurrentlyActive;
    public Checkpoint_Manager checkpointManager;

    [Header("Player camera")]
    public GameObject cameraPrefab;

    [Header("Skybox Settings")]
    public Material[] skyboxes;
    public int currentSkybox;

    [Header("Lighting Sources")]
    public GameObject[] lights;
    public int currentLight;

    [Header("Fall Triggers")]
    public GameObject[] fallTriggers;
    public int currentFallTriggers;

    [Header("Section 2")]
    public Section_Two_Displace secondDisplace;

    [Header("Section 6")]
    public GameObject particleZoneOne;
    public GameObject particleZoneTwo;

    [Header("Section 7")]
    public GameObject deerPlatform;
    public bool lightingIsTriggered;
    public GameObject lightingTrigger;
    public GameObject lightingTriggerObject;

    [Header("Section 9")]
    public GameObject lightingWall;
    public float lightingWallAnimationTime;

    [Header("Section 11")]
    public GameObject[] newComers; //New objects to keep track of.
    public GameObject[] oldComers;
    public bool cylinderIsFull;
    public float rollingBallAnimationTime;
    public GameObject rollingBall;
    public bool isRolling;

    [Header("Section 12")]
    public GameObject[] collectables;
    public Collector_Manager collectorManager;

    //Method for player prefab and position on start.
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        playerPrefabs[currentPlayerPrefab].SetActive(true);
        playerPrefabs[currentPlayerPrefab].BroadcastMessage("OnApplyPersistentData", SendMessageOptions.DontRequireReceiver);

        cameraPrefab.GetComponent<MFreeLookCamera>().m_Target = playerPrefabs[currentPlayerPrefab].transform;
        checkpointManager.player = playerPrefabs[currentPlayerPrefab];

        for (int i = 0; i <= playerPrefabs.Length - 1; i++)
        {
            if (playerPrefabs[i] != playerPrefabs[currentPlayerPrefab])
            {
                playerPrefabs[i].SetActive(false);
            }
        }
    }

    public void ActivatePlayerPosition(int positionNum)
    {
        for(int i = 0; i <= isCurrentlyActive.Length - 1; i++)
        {
            if(i != positionNum)
            {
                isCurrentlyActive[i] = false;
            }
            else
            {
                isCurrentlyActive[i] = true;
            }

            MovePlayer(positionNum);
        }
    }

    private void MovePlayer(int positionNumber)
    {
        playerPrefabs[currentPlayerPrefab].transform.position = possiblePlayerPositions[positionNumber].transform.position;
    }

    //Method for getting animation time in Section 9.
    public void GetAnimationTime()
    {
        lightingWallAnimationTime = lightingWall.GetComponent<Animation>()["Pressure"].time;
    }

    //Method for setting animation time in Section 9.
    public void SetAnimationTime()
    {
        lightingWall.GetComponent<Animation>().Play("Pressure");
        lightingWall.GetComponent<Animation>()["Pressure"].time = lightingWallAnimationTime; 
    }

    //Method for getting animation time in Section 11.
    public void RollingAnimationTimeGet()
    {
        rollingBallAnimationTime = rollingBall.GetComponent<Animation>()["Rolling_Ball"].time;
    }

    //Method for setting animation time in Section 11.
    public void SetAnimationTimeRollingBall()
    {
        rollingBall.GetComponent<Animation>().Play("Rolling_Ball");
        rollingBall.GetComponent<Animation>()["Rolling_Ball"].time = rollingBallAnimationTime;
    }

    //Method for enabling ball in Section 11.
    public void EnableBall()
    {
        isRolling = true;
    }

    //Method for checking displace in Section 2.
    public void CheckDisplaceSectionTwo()
    {
        if (!secondDisplace.stopDissolveSectionTwo)
        {
            secondDisplace.gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveSize", 0f);
            secondDisplace.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    //Method for checking on skybox on start.
    public void AdjustSkybox()
    {
        RenderSettings.skybox = skyboxes[currentSkybox];
        DynamicGI.UpdateEnvironment();

        if(RenderSettings.skybox == skyboxes[1])
        {
            RenderSettings.ambientIntensity = 1;
        }
        else if(RenderSettings.skybox == skyboxes[0])
        {
            RenderSettings.ambientIntensity = 1.46f;
        }
    }

    //Method for checking on light source on start.
    public void AdjustLight()
    {
        for(int i = 0; i <= lights.Length - 1; i++)
        {
            if(i != currentLight)
            {
                lights[i].SetActive(false);
            }
            else if(i == currentLight) { lights[i].SetActive(true); }
        }

        DynamicGI.UpdateEnvironment();
    }

    //Method for checking on fall triggers on start.
    public void AdjustFallTriggers()
    {
        for(int i = 0; i < fallTriggers.Length - 1; i++)
        {
            if(i != currentFallTriggers)
            {
                fallTriggers[i].SetActive(false);
            }
            else if(i == currentFallTriggers) { fallTriggers[i].SetActive(true); }
        }
    }

    //Method for enabling deer platform in Section 7.
    public void LightingTriggered()
    {
       lightingIsTriggered = true;
    }

    //Method for checking lighting trigger in Section 7.
    public void CheckLightingTrigger()
    {
        if (lightingIsTriggered)
        {
            lightingTrigger.SetActive(false);
            lightingTriggerObject.SetActive(false);
        }
    }

    //Method for checking on objects in Section 11.
    public void CheckSectionEleven()
    {
        if (cylinderIsFull)
        {
            foreach(GameObject g in newComers)
            {
                g.SetActive(true);
            }

            foreach (GameObject g in oldComers)
            {
                g.SetActive(false);
            }
        }
    }

    //Method for checking collectables in Section 14.
    public void CheckCollectables()
    {
        for(int i = 0; i <= collectorManager.areTaken.Length - 1; i++)
        {
            if(collectorManager.areTaken[i] == true)
            {
                if(i == 0) { collectorManager.levelFirst.SetActive(true); collectorManager.collectables[0].SetActive(false); }
                else if(i == 1) { collectorManager.levelSecond.SetActive(true); collectorManager.collectables[1].SetActive(false); }
                else if(i == 2) { collectorManager.levelThird.SetActive(true); collectorManager.collectables[2].SetActive(false); }
                else if(i == 3) { collectorManager.levelForth.SetActive(true); collectorManager.collectables[3].SetActive(false); }
                else if(i == 4) { collectorManager.levelFifth.SetActive(true); collectorManager.collectables[4].SetActive(false); }
                else if(i == 5) { collectorManager.levelSixth.SetActive(true); collectorManager.collectables[5].SetActive(false); }
            }
        }
    }
}