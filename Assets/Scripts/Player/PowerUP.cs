using UnityEngine;
using MalbersAnimations;

public enum PowerUpType { speed, shield }

//Class we use for establishing the power ups.
public class PowerUP : MonoBehaviour
{
    //Variables.
    public PowerUpType powerType = PowerUpType.speed;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            if(powerType == PowerUpType.speed)
            {
                //Invoke the speed method.
            }
            else if(powerType == PowerUpType.shield)
            {
                //Invoke the shield method.
            }
        }
    }

    dsad
}