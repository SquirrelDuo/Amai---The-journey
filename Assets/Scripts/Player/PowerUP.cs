using UnityEngine;
using MalbersAnimations;

public enum PowerUpType { speed, shield }

//Class we use for establishing the power ups.
public class PowerUP : MonoBehaviour
{
    //Variables.
    public PowerUpType powerType = PowerUpType.speed;
    public AbilityManager abilityManager; //The manager attached to the player.

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 20)
        {
            if(powerType == PowerUpType.speed)
            {
                //Invoke the store method.
                abilityManager.haveSpeedAbility = true;
            }
            else if(powerType == PowerUpType.shield)
            {
                //Invoke the store method.
                abilityManager.haveShieldAbility = true;
            }
        }
    }
}