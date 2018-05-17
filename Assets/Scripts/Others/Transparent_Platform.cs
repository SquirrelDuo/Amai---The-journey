using UnityEngine;

//Class controlling shader change on platforms.
public class Transparent_Platform : MonoBehaviour
{
    //Variables.
    public Material newMaterial;
    public Material oldMaterialMain;
    public Material oldMaterialSecondary;
    public GameObject[] platformObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            foreach (GameObject g in platformObjects)
            {
                g.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            foreach (GameObject g in platformObjects)
            {
                g.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            platformObjects[0].GetComponent<Renderer>().material = oldMaterialMain;

            for (int i = 1; i <= platformObjects.Length - 1; i++)
            {
                platformObjects[i].GetComponent<Renderer>().material = oldMaterialSecondary;
            }
        }
    }
}