using UnityEngine;

//Class we use for storing falling platforms values.
public class Falling_Platform : MonoBehaviour
{
    //Variables.
    public Vector3 startingPosition;
    public Quaternion startingRotation;

    private void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }
}