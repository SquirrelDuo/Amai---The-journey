using UnityEngine;

//Class checking the revive section.
public class Revive_Section : MonoBehaviour
{
    //Variables.
    public GameObject objectToActivate;
    private bool[] keyBools;

    private void Update()
    {
        //We check if the conditions are met.
        if(keyBools[0] && keyBools[1] && keyBools[2])
        {
            objectToActivate.SetActive(true);
            Destroy(gameObject, 2f);
        }
    }

    //Method for modifying booleans.
    public void SetBool(int boolNum)
    {
        keyBools[boolNum] = true;
    }
}