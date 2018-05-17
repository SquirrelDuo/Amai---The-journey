using UnityEngine;
using PixelCrushers.DialogueSystem;

//Class managing the languages in the game.
public class Language_Manager : MonoBehaviour
{
    //Variables.
    public int currentLanguage;

    public GameObject languageChoice;
    public GameObject mainMenu;

    public bool languageIsChosen;
    private int languageIsChosenInt;

    private void Awake()
    {
        OnApplyPersistentData();
    }

    public void OnEnable()
    {
        if (languageIsChosen) { languageIsChosenInt = 1; }
        else if(!languageIsChosen) { languageIsChosenInt = 0; }

        PersistentDataManager.RegisterPersistentData(this.gameObject);
    }

    public void OnDisable()
    {
        if (languageIsChosen) { languageIsChosenInt = 1; }
        else if (!languageIsChosen) { languageIsChosenInt = 0; }

        PersistentDataManager.RegisterPersistentData(this.gameObject);
    }

    public void OnRecordPersistentData()
    {
        PlayerPrefs.SetInt("CurrentLanguage", currentLanguage);
        PlayerPrefs.SetInt("isChosenLanguage", languageIsChosenInt);
    }

    public void OnApplyPersistentData()
    {
        languageIsChosenInt = PlayerPrefs.GetInt("isChosenLanguage");

        if(languageIsChosenInt == 1) { languageIsChosen = true; }
        else if(languageIsChosenInt == 0) { languageIsChosen = false; }

        currentLanguage = PlayerPrefs.GetInt("CurrentLanguage");

        if (languageIsChosen)
        {
            //We assign the language automatically.
            if (currentLanguage == 1) { DialogueManager.SetLanguage("English"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 2) { DialogueManager.SetLanguage("Български"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 3) { DialogueManager.SetLanguage("Russian"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 4) { DialogueManager.SetLanguage("German"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 5) { DialogueManager.SetLanguage("Spanish"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 6) { DialogueManager.SetLanguage("French"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 7) { DialogueManager.SetLanguage("Italian"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 8) { DialogueManager.SetLanguage("Netherlands"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 9) { DialogueManager.SetLanguage("Polish"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 10) { DialogueManager.SetLanguage("Czech"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
            else if (currentLanguage == 11) { DialogueManager.SetLanguage("Turkish"); languageChoice.SetActive(false); mainMenu.SetActive(true); }
        } 
        else if (!languageIsChosen)
        {
            languageChoice.SetActive(true); mainMenu.SetActive(false);
        }  
    }
}