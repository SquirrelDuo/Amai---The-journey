using UnityEngine;

//Class controlling rotating platforms.
public class RotatingPlatform : MonoBehaviour
{
    //Variables.
    public int[] RotationSpeed; //Range of different rotation speed.
    private int randomNumber; //Number by which we are going to choose speed.

    private void Start()
    {
        InvokeRepeating("RotatingRoutine", 1, 3);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (RotationSpeed[randomNumber] * Time.deltaTime));
    }

    //Routine for choosing random speed.
    void RotatingRoutine()
    {
        randomNumber = Random.Range(0, RotationSpeed.Length);
    }
}