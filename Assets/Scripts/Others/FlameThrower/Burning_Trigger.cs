using UnityEngine;

//Class responcible for triggering burned effect.
public class Burning_Trigger : MonoBehaviour
{
    //Variables.
    public Material_Smooth_Changer playerChanger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            playerChanger.isBurning =  true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            playerChanger.isBurning = false;
        }
    }
}