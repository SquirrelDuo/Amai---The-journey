using UnityEngine;

//Class triggering MoveTowards Platform movement.
public class TriggerPlatform : MonoBehaviour
{
    public GameObject platformObject; //The platform from which we take the trigger.

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            platformObject.GetComponent<MoveTowardsTransform>().isTriggered = true;
        }
    }
}