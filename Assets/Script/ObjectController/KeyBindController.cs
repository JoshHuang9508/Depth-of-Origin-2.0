using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class KeyBindController : MonoBehaviour
{
    public List<TMP_InputField> inputFieldlist = new();

    public Dictionary<TMP_InputField, bool> isFirstClickedDictionary = new();

    private void Start()
    {
        foreach (TMP_InputField inputfield in inputFieldlist)
        {
            isFirstClickedDictionary[inputfield] = true;
            AddEventTrigger(inputfield, OnInputFieldClick);
        }
        CustomInputManager.OnKeyPressed += OnkeyPressed;
    }

    private void OnkeyPressed(KeyCode keyCode)
    {
        string keyString = keyCode.ToString();

        foreach (TMP_InputField inputField in inputFieldlist)
        {
            if (inputField.isFocused)
            {
                inputField.text = keyString;
                break;
            }
        }

    }

    private void OnInputFieldClick(TMP_InputField clickedInputField)
    {
        if (isFirstClickedDictionary[clickedInputField])
        {
            clickedInputField.ActivateInputField();
            isFirstClickedDictionary[clickedInputField] = false;
        }
    }

    void AddEventTrigger(TMP_InputField inputField, System.Action<TMP_InputField> callback)
    {
        EventTrigger trigger = inputField.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = inputField.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { callback.Invoke(inputField); });
        trigger.triggers.Add(entry);
    }

    private void OnDestroy()
    {
        CustomInputManager.OnKeyPressed -= OnkeyPressed;
    }

}

public static class CustomInputManager
{
    public delegate void KeyPressedHandler(KeyCode keyCode);
    public static event KeyPressedHandler OnKeyPressed;

    public static void NotifyKeyPressed(KeyCode keyCode)
    {
        OnKeyPressed?.Invoke(keyCode);
    }
}
