using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

   public Camera camera;
    PostProcessingProfile profile;
    DepthOfFieldComponent depthOfField;
    public Text text;
    public CanvasGroup menu;
    

	void Start ()
    {
        profile = camera.GetComponent<PostProcessingBehaviour>().profile;
        profile.depthOfField.enabled = false;
        text.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        menu.alpha = 0;
    }
	
	
	void Update ()
    {
	if(Input.anyKeyDown)
        {
            profile.depthOfField.enabled = true;
            text.gameObject.SetActive(false);
            menu.gameObject.SetActive(true);
            menu.alpha = 1;
        }
	}
}
