using UnityEngine;

//Class managing the collectors.
public class Collector_Manager : MonoBehaviour
{
    //Variables.
    public GameObject levelFirst;
    public GameObject levelSecond;
    public GameObject levelThird;
    public GameObject levelForth;
    public GameObject levelFifth;
    public GameObject levelSixth;

    public GameObject[] collectables;

    public bool[] areTaken = new bool[6];

    public GameObject[] objectsToActivate;

    public GameObject trigger;

    private void Update()
    {
        if(levelFirst.activeSelf && levelSecond.activeSelf && levelThird.activeSelf && levelForth.activeSelf &&
            levelFifth.activeSelf && levelSixth.activeSelf)
        {
            trigger.SetActive(true);
        } 
    }

    public void ActivatePlatforms()
    {
        foreach (GameObject g in objectsToActivate)
        {
            g.SetActive(true);
        }
    }

    public void ActivateBool(int boolNum)
    {
        areTaken[boolNum] = true;
    }
}