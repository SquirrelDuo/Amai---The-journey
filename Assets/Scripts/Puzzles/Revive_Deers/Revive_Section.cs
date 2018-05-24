using UnityEngine;

//Class checking the revive section.
public class Revive_Section : MonoBehaviour
{
    //Variables.
    public GameObject[] deers;
    public GameObject objectToActivate;

    public bool[] deerBools;

    private void Update()
    {
        if(deerBools[0] && deerBools[1] && deerBools[2] && deerBools[3] && deerBools[4])
        {
            objectToActivate.SetActive(true);
            Destroy(gameObject, 2f);
        }
    }
}