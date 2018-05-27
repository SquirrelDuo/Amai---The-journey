using UnityEngine;

//Class we attach to each Rotating Stone.
public class Each_Rotating_Stone : MonoBehaviour
{
    //Variables.
    public Transform[] possibleRotations;
    public int currentRotation;

    private void OnEnable()
    {
        currentRotation = 5;
    }
}