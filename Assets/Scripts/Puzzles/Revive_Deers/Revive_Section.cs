using UnityEngine;

//Class checking the revive section.
public class Revive_Section : MonoBehaviour
{
    //Variables.
    public GameObject[] deers;
    public GameObject objectToActivate;

    private void Update()
    {
        foreach(GameObject g in deers)
        {
            if(g != null)
            {
                break;
            }

            objectToActivate.SetActive(true);
        }
    }
}