using UnityEngine;
using System.Collections;

//Class controlling scalling platforms.
public class ScalingPlatform : MonoBehaviour
{
    //Variables.
    public float maxSize;
    public float growFactor;
    public float waitTime;

    void Start()
    {
            StartCoroutine(Scale());
    }

    //Routine for changing object's scale.
    IEnumerator Scale()
    {
        float timer = 0;

        while (true)
        {
            while (maxSize > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            //Reset the scalling.
            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}