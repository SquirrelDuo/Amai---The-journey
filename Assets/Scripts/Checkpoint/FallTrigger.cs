using UnityEngine;

//Class attached to the fall trigger object.
public class FallTrigger : MonoBehaviour
{
    public Checkpoint_Manager checkpointManager; //Field for the scene's checkpoint manager.

    //If the player falls through.
    private void OnTriggerEnter(Collider other)
    {
       //Assign the "Animal" Layer to the Player.
       if(other.gameObject.layer == 20)
       {
            Debug.Log("The player has fallen");
            checkpointManager.isDead = true;
       } 
    }
}