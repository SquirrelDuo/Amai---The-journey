using UnityEngine;

//Class controlling transform change.
public class Change_Position : MonoBehaviour
{
    //Variables.
    public GameObject playerObject;
    public Transform newPosition;

    public void ChangePosition()
    {
        playerObject.transform.position = newPosition.position; 
    }
}