using UnityEngine;

//Class controlling routine to opening the gate in Section 6.
public class Open_Gate : MonoBehaviour
{
    //Variables.
    public bool firstOpen;
    public bool secondOpen;
    public GameObject gateToOpen;
    public GameObject sectionSeven;
    public GameObject replaceGate;

    private void OnEnable()
    {
        gateToOpen.SetActive(true);
        replaceGate.SetActive(false);
    }

    private void Update()
    {
        //We check if both bools are true.
        if(firstOpen && secondOpen)
        {
            gateToOpen.SetActive(false);
            replaceGate.SetActive(true);
            sectionSeven.SetActive(true);

            Destroy(gameObject, 2f);
        }
    }

    public void OpenFirst()
    {
        firstOpen = true;
    }

    public void OpenSecond()
    {
        secondOpen = true;
    }
}