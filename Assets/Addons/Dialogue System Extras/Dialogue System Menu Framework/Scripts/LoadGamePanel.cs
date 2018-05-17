using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script manages the LoadGamePanel.
    /// </summary>
    [RequireComponent(typeof(SelectablePanel))]
    public class LoadGamePanel : MonoBehaviour
    {

        [Tooltip("Game slots.")]
        public UnityEngine.UI.Button[] slots;

        [Tooltip("This button loads the game in the currently-selected slot.")]
        public UnityEngine.UI.Button loadButton;

        [Tooltip("This button deletes the game in the currently-selected slot.")]
        public UnityEngine.UI.Button deleteButton;

        [Tooltip("Shows the details of the game saved in the currently-selected slot.")]
        public UnityEngine.UI.Text details;

        [System.Serializable]
        public class StringEvent : UnityEvent<string> { }

        public StringEvent onSetDetails = new StringEvent();

        public UnityEvent onLoadGame = new UnityEvent();

        [HideInInspector]
        public int currentSlotNum = -1;

        private SaveHelper m_saveHelper = null;

        private void Awake()
        {
            if (m_saveHelper == null) m_saveHelper = FindObjectOfType<SaveHelper>();
        }

        public void SetupPanel()
        {
            details.gameObject.SetActive(false);
            loadButton.interactable = false;
            deleteButton.interactable = false;
            for (int slotNum = 0; slotNum < slots.Length; slotNum++)
            {
                var slot = slots[slotNum];
                var containsSavedGame = m_saveHelper.IsGameSavedInSlot(slotNum);
                var slotLabel = slot.GetComponentInChildren<UnityEngine.UI.Text>();
                if (slotLabel != null) slotLabel.text = m_saveHelper.GetSlotSummary(slotNum);
                slot.interactable = containsSavedGame;
            }
        }

        public void SelectSlot(int slotNum)
        {
            currentSlotNum = slotNum;
            m_saveHelper.currentSlotNum = slotNum;
            loadButton.interactable = true;
            deleteButton.interactable = true;
            var detailsText = m_saveHelper.GetSlotDetails(slotNum);
            details.text = detailsText;
            details.gameObject.SetActive(true);
            onSetDetails.Invoke(detailsText);
        }

        public void LoadCurrentSlot()
        {
            m_saveHelper.LoadGame(currentSlotNum);
            onLoadGame.Invoke();
        }

        public void DeleteCurrentSlot()
        {
            m_saveHelper.DeleteSavedGame(currentSlotNum);
            SetupPanel();
        }

    }

}