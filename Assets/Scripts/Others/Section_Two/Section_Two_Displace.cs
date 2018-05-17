using UnityEngine;

//Class we use for Section Two Dissolve.
public class Section_Two_Displace : MonoBehaviour
{
    //Variables.
    public float dissolviness = 3;

    public bool stopDissolveSectionTwo;

    public void StartRoutine()
    {
        stopDissolveSectionTwo = true;
    }

    private void Update()
    {
        if (stopDissolveSectionTwo)
        {
            dissolviness -= 0.45f * Time.deltaTime;

            if (dissolviness > 1.5f)
            {
                GetComponent<Renderer>().material.SetFloat("_DissolveSize", dissolviness);
               
            }
            else if (dissolviness <= 1.5f)
            {
                GetComponent<Renderer>().material.SetFloat("_DissolveSize", dissolviness);
                GetComponent<BoxCollider>().enabled = false;
                
            }
            else if(dissolviness < 0)
            {
                stopDissolveSectionTwo = false;
            }
        }
    }
}