using UnityEngine;

//Class we attach to the replacement platforms.
public class New_Break : MonoBehaviour
{
    public Animator animatorBreak; //Animator holding the animation of the platform.
    public GameObject objectToDeactivate;
    public GameObject particles;
    public GameObject breakPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("We are in");

            animatorBreak.enabled = true; //We start the animation.
            Invoke("ParticleAppear", 2);
            Invoke("DeactivateMe", 3);
        }
    }

    void DeactivateMe()
    {
        objectToDeactivate.SetActive(false);
        animatorBreak.enabled = false;
        particles.SetActive(false);
        Instantiate(breakPrefab);
    }

    void ParticleAppear()
    {
        particles.SetActive(true);
    }
}