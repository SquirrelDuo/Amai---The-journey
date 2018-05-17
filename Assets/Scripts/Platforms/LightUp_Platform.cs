using UnityEngine;

//Class for lighting up platforms.
public class LightUp_Platform : MonoBehaviour
{
    //Variables.
    public GameObject[] objectsToLight;
    public Material materialToChange;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            foreach(GameObject g in objectsToLight)
            {
                g.GetComponent<Renderer>().material = materialToChange;
            }
        }
    }
}