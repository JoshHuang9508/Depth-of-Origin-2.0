using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{
    

    [Header("Volume Settings")]
    public TMP_Text volumeValueText;
    public Slider volumeSlider;
    public float defaultVolume = 1f;

    [Header("Graphic Settings")]
    private bool isfullscreen;
    [Space(10)]
    public Toggle fullscreenToggle;

    [Header("Resolution DropDown")]
    public CustomDropdown resolutionDropdown;
    private Resolution[] resolutions;
    public Sprite dropdownIcon;

    [Header("Notification settings")]
    public NotificationManager keybindsavedNotification;

    [Header("KeyBind Settings")]
    public ContextMenuManager keybindsMenu,settingsMenu,pauseMenu;
    //initial keycode settings
    string walkforwardKey, walkrightKey, walkleftKey, walkbackKey, hotbar1Key, hotbar2Key, hotbar3Key, attackKey, backpackKey;
    public List<CustomInputField> keycodeInputField = new();
    public List<TMP_InputField> keycodeInputlist = new();

    

    private void Start()
    {
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        string option = "";
        for (int i = 0; i < resolutions.Length; i++)
        {
            option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
            resolutionDropdown.CreateNewItem(option, dropdownIcon, true);
        }
        resolutionDropdown.ChangeDropdownInfo(currentResolutionIndex);
        resolutionDropdown.RemoveItem("Item Title");


        //keycode inputfield settings
        KeyCode[] keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        List<string> keysName = new List<string>();
        for (int i = 0; i < keycodeInputField.Count; i++)
        {
            foreach (KeyCode _keyCode in keyCodes)
            {
                keysName.Add(_keyCode.ToString());
            }
        }
        InitialKeyCode(keysName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openPauseMenu();
        }
        KeycodeInput();
    }

    public void InitialKeyCode(List<string> keylist)
    {
        var player = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            for (int i = 0; i < keycodeInputField.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        keycodeInputField[0].inputText.text = "W";
                        walkforwardKey = "W";
                        break;
                    case 1:
                        keycodeInputField[1].inputText.text = "D";
                        walkrightKey = "D";
                        break;
                    case 2:
                        keycodeInputField[2].inputText.text = "A";
                        walkleftKey = "A";
                        break;
                    case 3:
                        keycodeInputField[3].inputText.text = "S";
                        walkbackKey = "S";
                        break;
                    case 4:
                        foreach (var key in keylist)
                        {
                            if (player.meleeWeaponKey.ToString() == key)
                            {
                                keycodeInputField[4].inputText.text = key;
                                hotbar1Key = key;
                            }
                        }
                        break;
                    case 5:
                        foreach (var key in keylist)
                        {
                            if (player.rangedWeaponKey.ToString() == key)
                            {
                                keycodeInputField[5].inputText.text = key;
                                hotbar2Key = key;
                            }
                        }
                        break;
                    case 6:
                        foreach (var key in keylist)
                        {
                            if (player.usePotionKey.ToString() == key)
                            {
                                keycodeInputField[6].inputText.text = key;
                                hotbar3Key = key;
                            }
                        }
                        break;
                    case 7:
                        foreach (var key in keylist)
                        {
                            if (player.useWeaponKey.ToString() == key)
                            {
                                keycodeInputField[7].inputText.text = key;
                                attackKey = key;
                            }
                        }
                        break;
                    case 8:
                        foreach (var key in keylist)
                        {
                            if (player.backpackKey.ToString() == key)
                            {
                                keycodeInputField[8].inputText.text = key;
                                backpackKey = key;
                            }
                        }
                        break;
                }
            }
        }
    }
    public void KeycodeInput()
    {
        foreach (var key in keycodeInputlist)
        {
            if (key.isFocused)
            {
                foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keycode))
                    {
                        string keyPressed = keycode.ToString();
                        key.text = keyPressed;
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
            }
        }
    }

    public void KeyCodeSaveBtn()
    {
        PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        List<string> keys = new();
        for (int i = 0; i < 9; i++)
        {
            keys.Add(keycodeInputField[i].inputText.text);
        }
        //ChangeAxisKey("Horizontal", keys[1], keys[2]);
        //ChangeAxisKey("Vertical", keys[0], keys[3]);
        player.meleeWeaponKey = StringToKeyCode(keys[4]);
        player.rangedWeaponKey = StringToKeyCode(keys[5]);
        player.usePotionKey = StringToKeyCode(keys[6]);
        player.useWeaponKey = StringToKeyCode(keys[7]);
        player.backpackKey = StringToKeyCode(keys[8]);
        keybindsavedNotification.Open();
    }

    KeyCode StringToKeyCode(string inputString)
    {
        try
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), inputString);
        }
        catch (Exception e)
        {
            Debug.LogError("Error converting string to KeyCode: " + e.Message);
            return KeyCode.None;
        }
    }

    /*private void ChangeAxisKey(string axisName, string newPositiveButton, string newNegativeButton)
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesArray = serializedObject.FindProperty("m_Axes");

        for (int i = 0; i < axesArray.arraySize; i++)
        {
            SerializedProperty axis = axesArray.GetArrayElementAtIndex(i);
            string axisPropertyName = axis.FindPropertyRelative("m_Name").stringValue;

            if (axisPropertyName == axisName)
            {
                axis.FindPropertyRelative("altPositiveButton").stringValue = newPositiveButton.ToLower();
                axis.FindPropertyRelative("altNegativeButton").stringValue = newNegativeButton.ToLower();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }*/

    public void KeyCodeResetBtn(int index)
    {
        switch (index)
        {
            case 0:
                keycodeInputField[0].inputText.text = walkforwardKey;
                break;
            case 1:
                keycodeInputField[1].inputText.text = walkrightKey;
                break;
            case 2:
                keycodeInputField[2].inputText.text = walkleftKey;
                break;
            case 3:
                keycodeInputField[3].inputText.text = walkbackKey;
                break;
            case 4:
                keycodeInputField[4].inputText.text = hotbar1Key;
                break;
            case 5:
                keycodeInputField[5].inputText.text = hotbar2Key;
                break;
            case 6:
                keycodeInputField[6].inputText.text = hotbar3Key;
                break;
            case 7:
                keycodeInputField[7].inputText.text = attackKey;
                break;
            case 8:
                keycodeInputField[8].inputText.text = backpackKey;
                break;
            case 9:
                List<string> indexlist = new() { walkforwardKey, walkrightKey, walkleftKey, walkbackKey, hotbar1Key, hotbar2Key, hotbar3Key, attackKey, backpackKey }; ;
                for (int i = 0; i < indexlist.Count; i++)
                {
                    keycodeInputField[i].inputText.text = indexlist[i];
                }
                break;
        }
        KeyCodeSaveBtn();
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ResetButton(string type)
    {
        if (type == "Graphic")
        {
            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentresolution = Screen.currentResolution;
            Screen.SetResolution(currentresolution.width, currentresolution.height, Screen.fullScreen);
            resolutionDropdown.ChangeDropdownInfo(resolutions.Length);
            GraphicApply();
        }
        if (type == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeValueText.text = defaultVolume.ToString() + "%";
            VolumeApply();
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeSlider.value = volume;
        volumeValueText.text = volume.ToString() + "%";
    }

    public void Setvolume(string _volume)
    {
        float volume = Convert.ToSingle(_volume);
        SetVolume(volume);
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void setFullScreen(bool _isfullscreen)
    {
        isfullscreen = _isfullscreen;
    }
    public void GraphicApply()
    {

        PlayerPrefs.SetInt("masterfullscreen", isfullscreen ? 1 : 0);
        Screen.fullScreen = isfullscreen;

    }
    //key bind menu open
    public void keyBindButtonOpen()
    {
        keybindsMenu.Open();
    }
    public void keyBindButtonClose()
    {
        keybindsMenu.Close();
    }
    public void openSettingsMenu()
    {
        settingsMenu.Open();
    }

    public void closeSettingsMenu()
    {
        settingsMenu.Close();
    }
    public void closePauseMenu()
    {
        pauseMenu.Close();
    }
    public void openPauseMenu()
    {
        pauseMenu.Open();
    }
    public void leavePauseMenu()
    {
        Time.timeScale = 1.0f;
    }
    public void QuitBtnClick()
    {
        PlayerPrefs.SetInt("loadscene", 0);
        SceneManager.LoadScene(1);
    }
}
