using UnityEngine;

//Class controlling breakable platforms.
public class BreakPlatform : MonoBehaviour
{
    //Variables.
    public GameObject breakablePrefab; //The replace prefab.
    public GameObject[] platformToReplace; //The platform we replace.
    public GameObject particleToAppear; 
    public Animator animatorBreak; //Animator holding the animation of the platform.
    public int timeToBreak;

    private void Start()
    {
        breakablePrefab.SetActive(false);
        particleToAppear.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        animatorBreak.enabled = true; //We start the animation.
        Invoke("ParticleAppear", timeToBreak - 1);
        Invoke("ReplacablePlatform", timeToBreak);
    }

    void ParticleAppear()
    {
        particleToAppear.SetActive(true);
    }

    void ReplacablePlatform()
    {
        gameObject.SetActive(false);
        
        foreach(GameObject g in platformToReplace)
        {
            g.SetActive(false);
        }

        breakablePrefab.SetActive(true);
    }
}