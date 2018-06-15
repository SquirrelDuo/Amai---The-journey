using UnityEngine;
using MalbersAnimations;

//Class controlling cubemap change.
public class Change_Cubemap : MonoBehaviour
{
    //Variables.
    private Variables_To_Save persistentValues;
    public Material cubeMap;
    public int chooseLight; //Number corresponding to a specific light source.
    public int fallTriggers;
    //public int currentPlayer;

    public void ChangeSettings()
    {
        persistentValues = GameObject.Find("Persistent_Values").GetComponent<Variables_To_Save>();

        //Player.
        //persistentValues.currentPlayerPrefab = currentPlayer;
        //Checkpoint_Manager manager = GameObject.Find("CheckPoint_Manager").GetComponent<Checkpoint_Manager>();
        //manager.player = persistentValues.playerPrefabs[currentPlayer];

        //Camera change.
        //persistentValues.cameraPrefab.GetComponent<MFreeLookCamera>().m_Target = persistentValues.playerPrefabs[currentPlayer].transform;

        //Skybox.
        RenderSettings.skybox = cubeMap;
        DynamicGI.UpdateEnvironment();

        for (int i = 0; i <= persistentValues.skyboxes.Length - 1; i++)
        {
            if(cubeMap == persistentValues.skyboxes[i])
            {
                persistentValues.currentSkybox = i;
            }
        }

        //Light Source.
        persistentValues.lights[chooseLight].SetActive(true);
        persistentValues.currentLight = chooseLight;

        for(int i = 0; i <= persistentValues.lights.Length - 1; i++)
        {
            if(persistentValues.lights[i] != persistentValues.lights[chooseLight])
            {
                persistentValues.lights[i].SetActive(false);
            }
        }

        //Fall triggers.
        persistentValues.fallTriggers[fallTriggers].SetActive(true);
        persistentValues.currentFallTriggers = fallTriggers;

        for (int i = 0; i <= persistentValues.fallTriggers.Length - 1; i++)
        {
            if (persistentValues.fallTriggers[i] != persistentValues.fallTriggers[fallTriggers])
            {
                persistentValues.fallTriggers[i].SetActive(false);
            }
        }
    }
}