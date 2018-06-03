using UnityEngine;
using MalbersAnimations;

//Class containing the Flamethrower Trigger routine.
public class Flamethrower_Trigger : MonoBehaviour
{
    //Variables.
    public Checkpoint_Manager checkpointManager; //Field for the scene's checkpoint manager.
    public Animator animalAnimator; //The Animator attached to the player.
    public MalbersInput malbersInput;
    public Animal animalScript;

    //If the player falls through.
    private void OnTriggerEnter(Collider other)
    {
        //Assign the "Animal" Layer to the Player.
        if (other.gameObject.layer == 20)
        {
            Debug.Log("In it!");
            InitDead();
        }
    }

    private void InitDead()
    {
        //malbersInput.enabled = false;
        //animalScript.enabled = false;
        //animalAnimator.Play("Death1");
        Invoke("InitRestart", 2f);
    }

    private void InitRestart()
    {
        checkpointManager.isDead = true;
        //malbersInput.enabled = true;
        //animalScript.enabled = true;
        Invoke("IdleReset", 2f);
    }

    private void IdleReset()
    {
        //animalAnimator.Play("Idle 01");
    }
}