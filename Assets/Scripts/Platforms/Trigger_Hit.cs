using UnityEngine;
using MalbersAnimations;

//Class for checking trigger hits on platforms.
public class Trigger_Hit : MonoBehaviour
{
    //Variables.
    public GameObject player;
    private Checkpoint_Manager checkpointManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            checkpointManager = GameObject.Find("CheckPoint_Manager").GetComponent<Checkpoint_Manager>();
            player = checkpointManager.player;
            PushPlayer();
        }
    }

    public void PushPlayer()
    {
        player.GetComponent<Animal>().Stun = true;
        player.GetComponent<Rigidbody>().AddExplosionForce(100, transform.up, 20);
        Invoke("ReleaseStun", 2f);
    }

    public void ReleaseStun()
    {
        player.GetComponent<Animal>().Stun = false;
    }
}