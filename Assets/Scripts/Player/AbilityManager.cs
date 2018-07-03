using UnityEngine;
using MalbersAnimations;

//Class we use for managing abilities.
public class AbilityManager : MonoBehaviour
{
    //Variables.
    public Animal ourAnimal; //The animal script attached to the player.

    public bool haveSpeedAbility;
    public bool haveShieldAbility;

    /// <summary>
    /// On bool change we have to activate/deactivate icon
    /// on the ability UI panel. On button press we will
    /// invoke the ability method.
    /// </summary>

    private float timer = 0f;

    private void Update()
    {
        if (haveSpeedAbility)
        {
            timer += 1 * Time.deltaTime;
            SpeedAbility();

            if (timer > 15f)
            {
                haveSpeedAbility = false;
                ourAnimal.powerUp = PowerUp.none;
            }
        }

        if (haveShieldAbility)
        {

        }
    }

    //Speed method.
    public void SpeedAbility()
    {
        ourAnimal.powerUp = PowerUp.speed;

        //Visual representation of the routine.
        //Particles.
        //Sound effect.
    }

    //Shield ability.
    public void ShieldAbility()
    {
        ourAnimal.powerUp = PowerUp.shield;

        //Visual representation of the routine.
        //Particles.
        //Sound effect.
    }
}