using UnityEngine;
using PixelCrushers.DialogueSystem;

//Class containing all language choices.
public class Language_Choice : MonoBehaviour
{
    private Language_Manager languageManager;

    private void Awake()
    {
        languageManager = GameObject.Find("Language_Manager").GetComponent<Language_Manager>();
    }

    public void EnglishChoice()
    {
        DialogueManager.SetLanguage("English");
        languageManager.currentLanguage = 1;
    }

    public void BulgarianChoice()
    {
        DialogueManager.SetLanguage("Български");
        languageManager.currentLanguage = 2;
    }

    public void RussianChoice()
    {
        DialogueManager.SetLanguage("Russian");
        languageManager.currentLanguage = 3;
    }

    public void GermanChoice()
    {
        DialogueManager.SetLanguage("German");
        languageManager.currentLanguage = 4;
    }

    public void SpanishChoice()
    {
        DialogueManager.SetLanguage("Spanish");
        languageManager.currentLanguage = 5;
    }

    public void FrenchChoice()
    {
        DialogueManager.SetLanguage("French");
        languageManager.currentLanguage = 6;
    }

    public void ItalianChoice()
    {
        DialogueManager.SetLanguage("Italian");
        languageManager.currentLanguage = 7;
    }

    public void NetherlandsChoice()
    {
        DialogueManager.SetLanguage("Netherlands");
        languageManager.currentLanguage = 8;
    }

    public void PolishChoice()
    {
        DialogueManager.SetLanguage("Polish");
        languageManager.currentLanguage = 9;
    }

    public void CzechChoice()
    {
        DialogueManager.SetLanguage("Czech");
        languageManager.currentLanguage = 10;
    }

    public void TurkishChoice()
    {
        DialogueManager.SetLanguage("Turkish");
        languageManager.currentLanguage = 11;
    }
}