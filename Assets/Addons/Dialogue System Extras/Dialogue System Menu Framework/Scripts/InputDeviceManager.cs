using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    public enum InputDevice { Joystick, Keyboard, Mouse, Touch }

    /// <summary>
    /// This script checks for joystick and keyboard input. If the player uses a joystick,
    /// it enables autofocus. If the player uses the mouse or keyboard, it disables autofocus.
    /// </summary>
    public class InputDeviceManager : MonoBehaviour
    {

        [Tooltip("Current input mode.")]
        public InputDevice inputDevice = InputDevice.Joystick;

        [Tooltip("If any of these keycodes are pressed, current device is joystick.")]
        public KeyCode[] joystickKeyCodesToCheck = new KeyCode[] { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton7 };

        [Tooltip("If any of these buttons are pressed, current device is joystick. Must be defined in Input Manager.")]
        public string[] joystickButtonsToCheck = new string[0];

        [Tooltip("If any of these axes are greater than Joystick Axis Threshold, current device is joystick. Must be defined in Input Manager.")]
        public string[] joystickAxesToCheck = new string[] { "JoystickAxis1", "JoystickAxis2", "JoystickAxis3", "JoystickAxis4", "JoystickAxis6", "JoystickAxis7" };

        [Tooltip("Joystick axis values must be above this threshold to switch to joystick mode.")]
        public float joystickAxisThreshold = 0.5f;

        [Tooltip("If any of these buttons are pressed, current device is keyboard (unless device is currently mouse).")]
        public string[] keyButtonsToCheck = new string[0];

        [Tooltip("If any of these keys are pressed, current device is keyboard (unless device is currently mouse).")]
        public KeyCode[] keyCodesToCheck = new KeyCode[] { KeyCode.Escape };

        [Tooltip("If mouse moves more than this, current device is mouse.")]
        public float mouseMoveThreshold = 2f;

        [Tooltip("When paused and device is mouse, make sure cursor is visible.")]
        public bool enforceCursorOnPause = false;

        [Tooltip("If any of these keycodes are pressed, go back to the previous menu.")]
        public KeyCode[] backKeyCodes = new KeyCode[] { KeyCode.JoystickButton1 };

        [Tooltip("If any of these buttons are pressed, go back to the previous menu.")]
        public string[] backButtons = new string[] { "Cancel", "JoystickButton1" };

        public UnityEvent onUseKeyboard = new UnityEvent();
        public UnityEvent onUseJoystick = new UnityEvent();
        public UnityEvent onUseMouse = new UnityEvent();
        public UnityEvent onUseTouch = new UnityEvent();

        public delegate bool GetButtonDownDelegate(string buttonName);

        public GetButtonDownDelegate GetButtonDown = null;

        private Vector3 m_lastMousePosition;
        private bool m_ignoreMouse = false;
        private UnityEngine.UI.GraphicRaycaster m_graphicRaycaster;

        private static InputDeviceManager m_instance = null;
        private static bool m_isApplicationQuitting = false;

        public static InputDevice currentInputDevice
        {
            get
            {
                if (m_instance == null && !m_isApplicationQuitting) Debug.LogError("Dialogue System Menus: Internal error - InputDeviceManager.m_instance is null.");
                return (m_instance != null) ? m_instance.inputDevice : InputDevice.Joystick;
            }
        }

        public static bool deviceUsesCursor
        {
            get { return currentInputDevice == InputDevice.Mouse; }
        }

        public static bool autoFocus
        {
            get { return currentInputDevice == InputDevice.Joystick || currentInputDevice == InputDevice.Keyboard; }
        }

        public static bool isBackButtonDown
        {
            get { return (m_instance != null) ? m_instance.IsBackButtonDown() : false; }
        }

        public static bool IsButtonDown(string buttonName)
        {
            return (m_instance != null) ? m_instance.GetButtonDown(buttonName) : false;
        }

        public void Awake()
        {
            if (m_instance == null) m_instance = this;
            m_graphicRaycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
            GetButtonDown = DefaultGetButtonDown;
        }

        public void Start()
        {
            m_lastMousePosition = Input.mousePosition;
            SetInputDevice(inputDevice);
            BrieflyIgnoreMouseMovement();
        }

        public void SetInputDevice(InputDevice newDevice)
        {
            inputDevice = newDevice;
            Tools.ShowCursor(deviceUsesCursor);
            m_graphicRaycaster.enabled = deviceUsesCursor;
            switch (inputDevice)
            {
                case InputDevice.Joystick:
                    onUseJoystick.Invoke();
                    break;
                case InputDevice.Keyboard:
                    onUseKeyboard.Invoke();
                    break;
                case InputDevice.Mouse:
                    var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                    var currentSelectable = (eventSystem != null && eventSystem.currentSelectedGameObject != null) ? eventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.Selectable>() : null;
                    if (currentSelectable != null) currentSelectable.OnDeselect(null);
                    onUseMouse.Invoke();
                    break;
                case InputDevice.Touch:
                    onUseTouch.Invoke();
                    break;
            }
            SetDialogueSystemUIAutoFocus();
        }

        public void Update()
        {
            switch (inputDevice)
            {
                case InputDevice.Joystick:
                    if (IsUsingMouse()) SetInputDevice(InputDevice.Mouse);
                    else if (IsUsingKeyboard()) SetInputDevice(InputDevice.Mouse);
                    break;
                case InputDevice.Keyboard:
                    if (IsUsingMouse()) SetInputDevice(InputDevice.Mouse);
                    else if (IsUsingJoystick()) SetInputDevice(InputDevice.Joystick);
                    break;
                case InputDevice.Mouse:
                    if (IsUsingJoystick()) SetInputDevice(InputDevice.Joystick);
                    break;
                case InputDevice.Touch:
                    if (IsUsingMouse()) SetInputDevice(InputDevice.Mouse);
                    else if (IsUsingKeyboard()) SetInputDevice(InputDevice.Mouse);
                    break;
            }
        }

        public bool IsUsingJoystick()
        {
            try
            {
                for (int i = 0; i < joystickKeyCodesToCheck.Length; i++)
                {
                    if (Input.GetKeyDown(joystickKeyCodesToCheck[i]))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < joystickButtonsToCheck.Length; i++)
                {
                    if (GetButtonDown(joystickButtonsToCheck[i]))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < joystickAxesToCheck.Length; i++)
                {
                    if (Mathf.Abs(Input.GetAxisRaw(joystickAxesToCheck[i])) > joystickAxisThreshold)
                    {
                        return true;
                    }
                }
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("Dialogue System Menus: Some input settings listed on the Input Device Manager component are missing from Unity's Input Manager. To automatically add them, inspect the UI's Input Device Manager component and click the 'Add Input Definitions' button at the bottom.\n" + e.Message, this);
            }
            return false;
        }

        public bool IsUsingMouse()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)) return true;
            var didMouseMove = !m_ignoreMouse && (Mathf.Abs(Input.mousePosition.x - m_lastMousePosition.x) > mouseMoveThreshold || Mathf.Abs(Input.mousePosition.y - m_lastMousePosition.y) > mouseMoveThreshold);
            m_lastMousePosition = Input.mousePosition;
            return didMouseMove;
        }

        public void BrieflyIgnoreMouseMovement()
        {
            StartCoroutine(BrieflyIgnoreMouseMovementCoroutine());
        }

        IEnumerator BrieflyIgnoreMouseMovementCoroutine()
        {
            m_ignoreMouse = true;
            yield return new WaitForSeconds(0.5f);
            m_ignoreMouse = false;
            m_lastMousePosition = Input.mousePosition;
            if (deviceUsesCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public bool IsUsingKeyboard()
        {
            try
            {
                for (int i = 0; i < keyCodesToCheck.Length; i++)
                {
                    if (Input.GetKeyDown(keyCodesToCheck[i]))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < keyButtonsToCheck.Length; i++)
                {
                    if (GetButtonDown(keyButtonsToCheck[i]))
                    {
                        return true;
                    }
                }
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("Dialogue System Menus: Some input settings listed on the Input Device Manager component are missing from Unity's Input Manager. To automatically add them, inspect the UI's Input Device Manager component and click the 'Add Input Definitions' button at the bottom.\n" + e.Message, this);
            }
            return false;
        }

        public bool IsBackButtonDown()
        {
            try
            {
                for (int i = 0; i < backKeyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(backKeyCodes[i]))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < backButtons.Length; i++)
                {
                    if (GetButtonDown(backButtons[i]))
                    {
                        return true;
                    }
                }
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("Dialogue System Menus: Some input settings listed on the Input Device Manager component are missing from Unity's Input Manager. To automatically add them, inspect the UI's Input Device Manager component and click the 'Add Input Definitions' button at the bottom.\n" + e.Message, this);
            }
            return false;
        }

        public bool DefaultGetButtonDown(string buttonName)
        {
            try
            {
                return string.IsNullOrEmpty(buttonName) ? false : Input.GetButtonDown(buttonName);
            }
            catch (System.ArgumentException) // Input button not in setup.
            {
                return false;
            }
        }

        public void OnConversationStart(Transform actor)
        {
            var unityUIDialogueUI = DialogueManager.DialogueUI as UnityUIDialogueUI;
            if (unityUIDialogueUI != null) unityUIDialogueUI.autoFocus = autoFocus;

        }

        public void SetDialogueSystemUIAutoFocus()
        {
            var questLogWindow = FindObjectOfType<QuestLogWindow>() as UnityUIQuestLogWindow;
            if (questLogWindow != null) questLogWindow.autoFocus = autoFocus;
        }

        public void OnApplicationQuit()
        {
            m_isApplicationQuitting = true;
        }

    }
}