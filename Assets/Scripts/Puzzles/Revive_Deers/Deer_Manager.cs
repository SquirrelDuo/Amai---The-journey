using MalbersAnimations;
using UnityEngine;

//Class we attach to deers for the Revive section.
public class Deer_Manager : MonoBehaviour
{
    //Variables.
    private Animator animator;
    private AnimalAIControl animalAi;

    private bool isReadyToGo;

    public GameObject objectToActivate;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animalAi = GetComponent<AnimalAIControl>();
        animator.Play("Sleep");
    }

    private void Update()
    {
        if (isReadyToGo)
        {
            Invoke("GetGoing", 3);
        }
    }

    public void GetUp()
    {
        animator.Play("Seat to Stand");
        isReadyToGo = true;

        if(objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }

    public void GetGoing()
    {
        animalAi.enabled = true;
        Destroy(gameObject, 5);
    }
}