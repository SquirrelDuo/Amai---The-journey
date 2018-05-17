using UnityEngine;

//Class we attach to fawns for dissolving.
public class Fawn_Dissolve : MonoBehaviour
{
    //Variables.
    public Renderer rend;
    public float dissolviness = 0f;
    public bool isDissolving;
    public Material holoShader;
	
    public void StartDissolve()
    {
        Invoke("BoolActivating", 2.5f);
    }

    private void BoolActivating()
    {
        isDissolving = true;
    }

    private void Update()
    {
        if (isDissolving)
        {
            rend.material = holoShader;

            dissolviness += 0.13f * Time.deltaTime;

            if (dissolviness < 1)
            {
                rend.material.SetFloat("_Dis", dissolviness);
            }
            else if(dissolviness >= 1)
            {
                isDissolving = false;
            }
        }
    }
}