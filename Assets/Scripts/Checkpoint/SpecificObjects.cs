using UnityEngine;

//We attach this class to objects that need to be reset.
public class SpecificObjects : MonoBehaviour
{
    public GameObject objectToReset; //Prefab to destroy.
    public GameObject objectToReplace; //Prefab to disable.

    public void ResetObject()
    {
        objectToReset.SetActive(false);
        objectToReplace.SetActive(true);
    }
}