using UnityEngine;

//Class we use for changing renderer on collision.
public class Renderer_Collision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 20)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GetComponent<Renderer>().enabled = false;
    }
}