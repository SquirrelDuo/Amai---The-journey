using UnityEngine;

//Class for manipulation quality and graphics settings in menu.
public class Menu_Porcessing : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.shadowDistance = 600; 
    }
}