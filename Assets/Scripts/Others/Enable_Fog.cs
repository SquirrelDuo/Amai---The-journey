using UnityEngine;

//Class we use for enabling/disabling fog in a scene.
public class Enable_Fog : MonoBehaviour
{
    public void EnableFog()
    {
        RenderSettings.fog = true;
        DynamicGI.UpdateEnvironment();
    }

    public void DisableFog()
    {
        RenderSettings.fog = false;
        DynamicGI.UpdateEnvironment();
    }
}