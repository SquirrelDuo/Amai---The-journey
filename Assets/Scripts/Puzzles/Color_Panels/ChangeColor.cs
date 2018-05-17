using UnityEngine;

//Class for each color panel.
public class ChangeColor : MonoBehaviour
{
    //Variables.
    public AudioSource audioToPlay;
    public GameObject objectToInteractWith;
    public GameObject objectToActivate;

    public bool thereIsAnimation;
    public Animator animator;

    public bool isChangingMaterial;
    public Material newMaterial;

    public bool isActivated;

    public void ChangeColorPanel()
    {
        isActivated = true;

        if (thereIsAnimation)
        {
            animator.enabled = true;
        }

        objectToInteractWith.transform.position = new Vector3(gameObject.transform.position.x, objectToInteractWith.transform.position.y,
                gameObject.transform.position.z);
        objectToInteractWith.GetComponent<Rigidbody>().isKinematic = true;
        //objectToInteractWith.GetComponent<Rigidbody>().detectCollisions = false;

        if(objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (isChangingMaterial)
        {
            objectToInteractWith.GetComponent<Renderer>().material = newMaterial;
        }
      
        audioToPlay.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == objectToInteractWith)
        {
            ChangeColorPanel();
        }
    }
}