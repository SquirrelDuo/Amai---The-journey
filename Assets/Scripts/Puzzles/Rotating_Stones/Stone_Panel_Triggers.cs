using UnityEngine;

//Class we use for detecting trigger events in the Rotating Stones puzzle.
public class Stone_Panel_Triggers : MonoBehaviour
{
    //Variables.
    public MeshRenderer ourMeshRenderer; //The renderer we work with.
    public Material oldMaterial;
    public Material newMaterial;
    public bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "StonePanel")
        {
            isActivated = true;
            ourMeshRenderer.material = newMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "StonePanel")
        {
            isActivated = false;
            ourMeshRenderer.material = oldMaterial;
        }
    }
}