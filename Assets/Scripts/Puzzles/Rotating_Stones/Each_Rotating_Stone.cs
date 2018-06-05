using UnityEngine;

//Class we attach to each Rotating Stone.
public class Each_Rotating_Stone : MonoBehaviour
{
    //Variables.
    public string[] possibleStateNames;
    public Animator thisAnimator;
    public int currentRotation;

    private void OnEnable()
    {
        currentRotation = 5; //We set an impossible rotation.
    }
}