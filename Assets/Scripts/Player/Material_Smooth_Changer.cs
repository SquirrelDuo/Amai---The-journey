using UnityEngine;

//Class we use for smooth material change of the wolf cub.
public class Material_Smooth_Changer : MonoBehaviour
{
    //Variables.
    public Material playerMaterial; //The material we work with.

    //Flamethrower variables.
    public bool isBurning;
    public Color playerColor;
    private Color burnedColor = Color.red;
    private bool hasBurned;

    private void OnEnable()
    {
        playerColor = playerMaterial.color;
    }

    private void Update()
    {
        //If we are burning.
        if (isBurning)
        {
            playerMaterial.color = Color.Lerp(playerColor, burnedColor, Mathf.PingPong(Time.time, 1));
            playerMaterial.SetColor("_EmissionColor", Color.Lerp(playerColor, burnedColor, Mathf.PingPong(Time.time, 1)));
            hasBurned = true;
        }
        else
        {
            if(hasBurned)
            {
                Debug.Log("Here");
                EndBurning();
            }  
        }   
    }

    private void EndBurning()
    {
        playerMaterial.color = Color.Lerp(burnedColor, playerColor, Mathf.PingPong(Time.time, 1));
        playerMaterial.SetColor("_EmissionColor", Color.Lerp(burnedColor, playerColor, Mathf.PingPong(Time.time, 1)));
        hasBurned = false;
    }
}