using UnityEngine;

//Class controlling the managing panel in the Rotating Stones puzzle.
public class Control_Panel : MonoBehaviour
{
    //Variables.
    public Each_Rotating_Stone[] rotatingStones; //We store a reference to the stones.
    public float rotatingSpeed = 5f;

    private void Update()
    {
        
    }

    //Moving 1 and 4.
    public void FirstMovement()
    {
        //If this is the first time we rotate.
        if(rotatingStones[0].currentRotation == 5)
        {
            rotatingStones[0].transform.rotation = Quaternion.Lerp(rotatingStones[0].transform.rotation,
                rotatingStones[0].possibleRotations[0].rotation, Time.time * rotatingSpeed);

            rotatingStones[0].currentRotation = 0;
        }
        //If its not.
        else if(rotatingStones[0].currentRotation != 5)
        {
            rotatingStones[0].transform.rotation = Quaternion.Lerp(rotatingStones[0].transform.rotation,
                rotatingStones[0].possibleRotations[rotatingStones[0].currentRotation + 1].rotation, Time.time * rotatingSpeed);

            rotatingStones[0].currentRotation = rotatingStones[0].currentRotation + 1;
        }

        //Repeat for the other stone.
        //If this is the first time we rotate.
        if (rotatingStones[3].currentRotation == 5)
        {
            rotatingStones[3].transform.rotation = Quaternion.Lerp(rotatingStones[3].transform.rotation,
                rotatingStones[3].possibleRotations[0].rotation, Time.time * rotatingSpeed);

            rotatingStones[3].currentRotation = 0;
        }
        //If its not.
        else if (rotatingStones[3].currentRotation != 5)
        {
            rotatingStones[3].transform.rotation = Quaternion.Lerp(rotatingStones[3].transform.rotation,
                rotatingStones[3].possibleRotations[rotatingStones[0].currentRotation + 1].rotation, Time.time * rotatingSpeed);

            rotatingStones[3].currentRotation = rotatingStones[0].currentRotation + 1;
        }
    }

    public void SecondMovement()
    {

    }

    public void ThirdMovement()
    {

    }

    public void ForthMovement()
    {

    }
}