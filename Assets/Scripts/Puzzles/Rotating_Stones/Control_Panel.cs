using UnityEngine;

//Class controlling the managing panel in the Rotating Stones puzzle.
public class Control_Panel : MonoBehaviour
{
    //Variables.
    public Each_Rotating_Stone[] rotatingStones; //We store a reference to the stones.
    public Stone_Panel_Triggers[] triggerObjects;

    private void Update()
    {
        if(triggerObjects[0].isActivated &&
            triggerObjects[1].isActivated &&
            triggerObjects[2].isActivated &&
            triggerObjects[3].isActivated &&
            triggerObjects[4].isActivated &&
            triggerObjects[5].isActivated &&
            triggerObjects[6].isActivated &&
            triggerObjects[7].isActivated)
        {
            GetComponent<Animator>().enabled = true;
            this.enabled = false;
        }
    }

    //Moving 1 and 3.
    public void FirstMovement()
    {
        //If this is the first time we rotate.
        if(rotatingStones[0].currentRotation == 5)
        {
            rotatingStones[0].currentRotation = 0;
            rotatingStones[0].thisAnimator.enabled = true;
            rotatingStones[0].thisAnimator.Play("Stone_One_First");
        }
        //If its not.
        else if(rotatingStones[0].currentRotation != 5)
        {
            if(rotatingStones[0].currentRotation == 3)
            {
                rotatingStones[0].currentRotation = -1;
            }

            rotatingStones[0].currentRotation += 1;
            rotatingStones[0].thisAnimator.
                Play(rotatingStones[0].possibleStateNames[rotatingStones[0].currentRotation]);
        }

        //If this is the first time we rotate.
        if (rotatingStones[2].currentRotation == 5)
        {
            rotatingStones[2].currentRotation = 0;
            rotatingStones[2].thisAnimator.enabled = true;
            rotatingStones[2].thisAnimator.Play("Stone_Three_First");
        }
        //If its not.
        else if (rotatingStones[2].currentRotation != 5)
        {
            if (rotatingStones[2].currentRotation == 3)
            {
                rotatingStones[2].currentRotation = -1;
            }

            rotatingStones[2].currentRotation += 1;
            rotatingStones[2].thisAnimator.
                Play(rotatingStones[2].possibleStateNames[rotatingStones[2].currentRotation]);
        }
    }

    //Moving 2 and 4.
    public void SecondMovement()
    {
        //If this is the first time we rotate.
        if (rotatingStones[1].currentRotation == 5)
        {
            rotatingStones[1].currentRotation = 0;
            rotatingStones[1].thisAnimator.enabled = true;
            rotatingStones[1].thisAnimator.Play("Stone_Two");
        }
        //If its not.
        else if (rotatingStones[1].currentRotation != 5)
        {
            if (rotatingStones[1].currentRotation == 3)
            {
                rotatingStones[1].currentRotation = -1;
            }

            rotatingStones[1].currentRotation += 1;
            rotatingStones[1].thisAnimator.
                Play(rotatingStones[1].possibleStateNames[rotatingStones[1].currentRotation]);
        }

        //If this is the first time we rotate.
        if (rotatingStones[3].currentRotation == 5)
        {
            rotatingStones[3].currentRotation = 0;
            rotatingStones[3].thisAnimator.enabled = true;
            rotatingStones[3].thisAnimator.Play("Stone_Four_One");
        }
        //If its not.
        else if (rotatingStones[3].currentRotation != 5)
        {
            if (rotatingStones[3].currentRotation == 3)
            {
                rotatingStones[3].currentRotation = -1;
            }

            rotatingStones[3].currentRotation += 1;
            rotatingStones[3].thisAnimator.
                Play(rotatingStones[3].possibleStateNames[rotatingStones[3].currentRotation]);
        }
    }

    //Moving 1 and 4.
    public void ThirdMovement()
    {
        //If this is the first time we rotate.
        if (rotatingStones[0].currentRotation == 5)
        {
            rotatingStones[0].currentRotation = 0;
            rotatingStones[0].thisAnimator.enabled = true;
            rotatingStones[0].thisAnimator.Play("Stone_One_First");
        }
        //If its not.
        else if (rotatingStones[0].currentRotation != 5)
        {
            if (rotatingStones[0].currentRotation == 3)
            {
                rotatingStones[0].currentRotation = -1;
            }

            rotatingStones[0].currentRotation += 1;
            rotatingStones[0].thisAnimator.
                Play(rotatingStones[0].possibleStateNames[rotatingStones[0].currentRotation]);
        }

        //If this is the first time we rotate.
        if (rotatingStones[3].currentRotation == 5)
        {
            rotatingStones[3].currentRotation = 0;
            rotatingStones[3].thisAnimator.enabled = true;
            rotatingStones[3].thisAnimator.Play("Stone_Four_One");
        }
        //If its not.
        else if (rotatingStones[3].currentRotation != 5)
        {
            if (rotatingStones[3].currentRotation == 3)
            {
                rotatingStones[3].currentRotation = -1;
            }

            rotatingStones[3].currentRotation += 1;
            rotatingStones[3].thisAnimator.
                Play(rotatingStones[3].possibleStateNames[rotatingStones[3].currentRotation]);
        }
    }

    //Moving 2 and 3.
    public void ForthMovement()
    {
        //If this is the first time we rotate.
        if (rotatingStones[1].currentRotation == 5)
        {
            rotatingStones[1].currentRotation = 0;
            rotatingStones[1].thisAnimator.enabled = true;
            rotatingStones[1].thisAnimator.Play("Stone_Two");
        }
        //If its not.
        else if (rotatingStones[1].currentRotation != 5)
        {
            if (rotatingStones[1].currentRotation == 3)
            {
                rotatingStones[1].currentRotation = -1;
            }

            rotatingStones[1].currentRotation += 1;
            rotatingStones[1].thisAnimator.
                Play(rotatingStones[1].possibleStateNames[rotatingStones[1].currentRotation]);
        }

        //If this is the first time we rotate.
        if (rotatingStones[2].currentRotation == 5)
        {
            rotatingStones[2].currentRotation = 0;
            rotatingStones[2].thisAnimator.enabled = true;
            rotatingStones[2].thisAnimator.Play("Stone_Three_First");
        }
        //If its not.
        else if (rotatingStones[2].currentRotation != 5)
        {
            if (rotatingStones[2].currentRotation == 3)
            {
                rotatingStones[2].currentRotation = -1;
            }

            rotatingStones[2].currentRotation += 1;
            rotatingStones[2].thisAnimator.
                Play(rotatingStones[2].possibleStateNames[rotatingStones[2].currentRotation]);
        }
    }
}