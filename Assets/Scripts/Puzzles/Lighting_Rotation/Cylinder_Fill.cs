using UnityEngine;
using System.Collections;
using MalbersAnimations;

//Class we use for filling cylinder with lighting.
public class Cylinder_Fill : MonoBehaviour
{
    //Variables.
    public GameObject[] cylinderParts;
    public GameObject[] particleToActivate;
    public Material newMaterial;
    public GameObject[] triggerToDeactivate;

    public Animal animal;

    float objectNum = 0;

    public void FillCylinder()
    {
        if(objectNum <= 20)
        {
            StartCoroutine(LateCall());
        }else
        {
            StopCoroutine(LateCall());
           
            foreach(GameObject g in triggerToDeactivate)
            {
                g.SetActive(false);
            }

            foreach(GameObject t in particleToActivate)
            {
                t.SetActive(true);
            }

            animal.LightingRoutine = false;

            Variables_To_Save persistentValues = GameObject.Find("Persistent_Values").GetComponent<Variables_To_Save>();
            persistentValues.cylinderIsFull = true;
        }
    }

    IEnumerator LateCall()
    {
        for(int i = 0; i <= cylinderParts.Length - 1; i++)
        {
            yield return new WaitForSeconds(0.2f);
            cylinderParts[i].GetComponent<Renderer>().material = newMaterial;
            if(i == 19) { objectNum = 21; }
            yield return new WaitForSeconds(0.2f);
        }
    }
}