﻿using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    [CustomEditor(typeof(InputDeviceManager))]
    public class InputDeviceManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button(new GUIContent("Add Input Definitions", "If any of the buttons or axes listed above aren't in Unity's Input Manager, add them.")))
            {
                AddInputDefinitions(target as InputDeviceManager);
            }
        }

        public static void AddInputDefinitions(InputDeviceManager inputDeviceManager)
        {
            if (inputDeviceManager == null) return;
            foreach (var button in inputDeviceManager.joystickButtonsToCheck)
            {
                AddInputDefinition(button);
            }
            foreach (var axis in inputDeviceManager.joystickAxesToCheck)
            {
                AddInputDefinition(axis);
            }
            foreach (var button in inputDeviceManager.keyButtonsToCheck)
            {
                AddInputDefinition(button);
            }
            foreach (var button in inputDeviceManager.backButtons)
            {
                AddInputDefinition(button);
            }
            Debug.Log("All input definitions are in Unity's Input Manager.");
        }

        public static void AddInputDefinition(string axisName)
        {
            if (string.IsNullOrEmpty(axisName)) return;
            switch (axisName)
            {
                case "JoystickButton0":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 0", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton1":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 1", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton2":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 2", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton3":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 3", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton4":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 4", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton5":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 5", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton6":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 6", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickButton7":
                    AddAxis(new InputAxis() { name = axisName, positiveButton = "joystick button 7", gravity = 1000, dead = 0.0001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
                    break;
                case "JoystickAxis1":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 1, joyNum = 0, });
                    break;
                case "JoystickAxis2":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 2, joyNum = 0, });
                    break;
                case "JoystickAxis3":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 3, joyNum = 0, });
                    break;
                case "JoystickAxis4":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 4, joyNum = 0, });
                    break;
                case "JoystickAxis5":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 5, joyNum = 0, });
                    break;
                case "JoystickAxis6":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 6, joyNum = 0, });
                    break;
                case "JoystickAxis7":
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 7, joyNum = 0, });
                    break;
                default:
                    AddAxis(new InputAxis() { name = axisName, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 7, joyNum = 0, });
                    return;
            }
        }

        // From: https://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        private static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return;

            Debug.Log("Added to Input Manager: " + axis.name);

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }

        private static void AddAxisUndefined(string axisName)
        {
            if (AxisDefined(axisName)) return;
            Debug.LogWarning("Will add to Input Manager: " + axisName + " but you must set its values (Edit > Project Settings > Input).");
            AddAxis(new InputAxis() { name = axisName });
        }
    }
}