using UnityEngine;

//Class controlling movement towards a transform.
public class MoveTowardsTransform : MonoBehaviour
{
    //Variables.
    public Transform target;
    public float speed;

    public bool isTriggered; //If we are ready to move.

    void Update()
    {
        if (isTriggered)
        {
            float step = (speed * Time.deltaTime) / 5;

            //We move towards the target with the current speed.
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    //Use to trigger event from Action script.
    public void triggerTrigger()
    {
        isTriggered = true;
    }
}