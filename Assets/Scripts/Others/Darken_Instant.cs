using UnityEngine;

//Class we use for instant darkening after the Old Gate.
public class Darken_Instant : MonoBehaviour
{
    //Variables.
    public Material newSkybox;
    public GameObject lightSource;

    public void DarkenScene()
    {
        RenderSettings.ambientLight = Color.black;
        RenderSettings.skybox = newSkybox;
        lightSource.SetActive(false);
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 1;
        RenderSettings.ambientIntensity = 0;
    }
}