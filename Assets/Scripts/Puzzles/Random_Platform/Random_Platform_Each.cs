using UnityEngine;

//Class we attack to each platform in section 12.
public class Random_Platform_Each : MonoBehaviour
{
    //Variables.
    public Transform[] possiblePosition;
    private Transform lastTransform;

    public void MovePlatformDouble()
    {
        //We move the platform to a new position;
        for (int i = 0; i <= possiblePosition.Length - 1; i++)
        {
            if (gameObject.transform.position != possiblePosition[i].position)
            {
                GetComponent<MoveTowardsTransform>().target = possiblePosition[i];
                GetComponent<MoveTowardsTransform>().triggerTrigger();
            }
        }
    }

    public void MovePlatformTripple()
    {
        if(lastTransform != null)
        {
            int randomNum = Random.Range(0, 2);

            if (randomNum == 1)
            {
                //We move the platform to a new position;
                if (gameObject.transform.position != possiblePosition[0].position && gameObject != lastTransform.gameObject)
                {
                    GetComponent<MoveTowardsTransform>().target = possiblePosition[0];
                    GetComponent<MoveTowardsTransform>().triggerTrigger();
                    lastTransform = possiblePosition[0];
                }
                else if (gameObject.transform.position != possiblePosition[1].position && gameObject != lastTransform.gameObject)
                {
                    GetComponent<MoveTowardsTransform>().target = possiblePosition[1];
                    GetComponent<MoveTowardsTransform>().triggerTrigger();
                    lastTransform = possiblePosition[1];
                }
            }
            else if (randomNum == 0)
            {
                //We move the platform to a new position;
                if (gameObject.transform.position != possiblePosition[0].position && gameObject != lastTransform.gameObject)
                {
                    GetComponent<MoveTowardsTransform>().target = possiblePosition[0];
                    GetComponent<MoveTowardsTransform>().triggerTrigger();
                    lastTransform = possiblePosition[0];
                }
                else if (gameObject.transform.position != possiblePosition[2].position && gameObject != lastTransform.gameObject)
                {
                    GetComponent<MoveTowardsTransform>().target = possiblePosition[2];
                    GetComponent<MoveTowardsTransform>().triggerTrigger();
                    lastTransform = possiblePosition[2];
                }
            }
        }
        else
        {
            //We move the platform to a new position;
            if (gameObject.transform.position != possiblePosition[0].position)
            {
                GetComponent<MoveTowardsTransform>().target = possiblePosition[0];
                GetComponent<MoveTowardsTransform>().triggerTrigger();
                lastTransform = possiblePosition[0];
            }
            else if (gameObject.transform.position != possiblePosition[1].position)
            {
                GetComponent<MoveTowardsTransform>().target = possiblePosition[1];
                GetComponent<MoveTowardsTransform>().triggerTrigger();
                lastTransform = possiblePosition[1];
            }
            else if (gameObject.transform.position != possiblePosition[2].position)
            {
                GetComponent<MoveTowardsTransform>().target = possiblePosition[2];
                GetComponent<MoveTowardsTransform>().triggerTrigger();
                lastTransform = possiblePosition[2];
            }
        }
    }
}