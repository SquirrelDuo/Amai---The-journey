using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    public class EventOnStart : MonoBehaviour
    {

        public int musicIndex = -1;

        public UnityEvent onStart = new UnityEvent();

        void Start()
        {
            if (musicIndex != -1)
            {
                var musicManager = FindObjectOfType<MusicManager>();
                if (musicManager != null) musicManager.PlayGameplayMusic(musicIndex);
            }
            onStart.Invoke();
        }

    }
}