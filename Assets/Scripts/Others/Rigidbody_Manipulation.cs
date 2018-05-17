using UnityEngine;

//Class for manipulation player's rigidbody.
public class Rigidbody_Manipulation : MonoBehaviour
{
    //public Rigidbody playersRigidbody;
    public Rigidbody playerRigidbody;

    public void ChangeParent()
    {
        playerRigidbody.AddForce(-transform.forward * 200, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            ChangeParent();
        }
    }
}