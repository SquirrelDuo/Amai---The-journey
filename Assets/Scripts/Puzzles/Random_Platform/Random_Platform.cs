using UnityEngine;

//Class controlling random platform movement.
public class Random_Platform : MonoBehaviour
{
    //Variables.
    public GameObject[] platformsToMove;

    //Manager type.
    public bool isDouble;
    public bool isTripple;

    //Method for random moving the platforms in the group.
    public void MovePlatforms()
    {
        if (isDouble)
        {
            foreach (GameObject g in platformsToMove)
            {
                g.GetComponent<Random_Platform_Each>().MovePlatformDouble();
            }
        }

        if (isTripple)
        {
            foreach (GameObject g in platformsToMove)
            {
                g.GetComponent<Random_Platform_Each>().MovePlatformTripple();
            }
        }
    }
}