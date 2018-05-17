using UnityEngine;

//Class managing color panels puzzle.
public class ColorPanelManager : MonoBehaviour
{
    //Variables.
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;

    public GameObject objectToActivate;
    public GameObject[] objectToDeactivate;

    void Update()
    {
        CheckColors();
    }

    void CheckColors()
    {
        if (object1.GetComponentInChildren<ChangeColor>().isActivated &&
                object2.GetComponentInChildren<ChangeColor>().isActivated &&
                    object3.GetComponentInChildren<ChangeColor>().isActivated)
        {
            objectToActivate.SetActive(true);
            gameObject.SetActive(false);

            foreach(GameObject g in objectToDeactivate)
            {
                g.SetActive(false);
            }
        }
    }
}