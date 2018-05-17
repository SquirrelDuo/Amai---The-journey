using UnityEngine;
using System.Collections;

//Class controlling Ping Pong type of movement.
public class PingPongPlatform : MonoBehaviour
{
    //Variables.
    public float speed = 1.0f;
    private bool dirRight;
    public float timeToWait;
    public GameObject startPosition;
    public GameObject endPosition;

    private void Start()
    {
        StartCoroutine(PingPong());
    }

    void Update()
    {
        if (transform.position == endPosition.transform.position)
        {
                dirRight = false;
        } 

        if (transform.position == startPosition.transform.position)
        {
                dirRight = true;
        }
    }

    IEnumerator PingPong()
    {
        float timer = 0;

        while (true)
        {
            while (dirRight)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition.transform.position, endPosition.transform.position, Mathf.PingPong(Time.time * speed, 1.0f));

                yield return null;
            }

            //Reset the scalling.
            yield return new WaitForSeconds(timeToWait);

            timer = 0;
            while (!dirRight)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(endPosition.transform.position, startPosition.transform.position, Mathf.PingPong(Time.time * speed, 1.0f));

                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(timeToWait);
        }
    }
}