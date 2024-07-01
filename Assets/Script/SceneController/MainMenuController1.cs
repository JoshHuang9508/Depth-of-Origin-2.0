using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Video;
using Michsky.MUIP;
using UnityEngine.EventSystems;

public class MainMenuController1 : MonoBehaviour
{
    [Header("GameLogo Settings")]
    public Image backgroundImage;
    public GameObject mainmenu, settingmenu, logopanel, mainmenuanimation, mainmenuanimation2; 
    public VideoPlayer videoplayer, MainMenuVideoPlayer, MainMenu2VideoPlayer;
    public float fadeInDuration = 0.5f, fadeOutDuration = 5.0f;

    [Header("Volume Settings")]
    public TMP_Text volumeValueText;
    public Slider volumeSlider;
    public float defaultVolume = 50f;

    [Header("Graphic Settings")]
    private bool isfullscreen;
    [Space(10)]
    public Toggle fullscreenToggle;


    [Header("Level To load")]
    public int newGameLevel;
    public int leveltoLoad;

    [Header("Resolution DropDown")]
    public CustomDropdown resolutionDropdown;
    private Resolution[] resolutions;
    public Sprite dropdownIcon;

    [Header("ModalWindows settings")]
    public ModalWindowManager NewGamemodalWindow, LoadGamemodalWindow;

    [Header("Notification settings")]
    public NotificationManager NoSavedGameNotification,keybindsavedNotification;

    [Header("KeyBind Settings")]
    public ContextMenuManager keybindsMenu;
    //initial keycode settings
    string walkforwardKey,walkrightKey,walkleftKey,walkbackKey,hotbar1Key,hotbar2Key,hotbar3Key,attackKey,backpackKey;
    public List<CustomInputField> keycodeInputField = new();
    public List<TMP_InputField>keycodeInputlist = new();

    void Start()
    {
        resolutions = Screen.resolutions;
        MainMenuVideoPlayer.Prepare();
        MainMenu2VideoPlayer.Prepare();
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
        mainmenuanimation.SetActive(false);
        mainmenu.SetActive(false);
        logopanel.SetActive(true);
        StartCoroutine(OpenMainMenu());

        //keycode inputfield settings
        KeyCode[] keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        List<string> keysName = new List<string>();
        for(int i = 0; i < keycodeInputField.Count; i++)
        {
            foreach (KeyCode _keyCode in keyCodes)
            {
                keysName.Add(_keyCode.ToString());
            }
        }
        InitialKeyCode(keysName);
        
    }

    

    IEnumerator OpenMainMenu()
    {
        yield return new WaitForSeconds(4.5f);
        logopanel.SetActive(false);
        StartCoroutine(MainMenuAnimation());
    }

    public IEnumerator MainMenuAnimation()
    {
        MainMenuVideoPlayer.Play();
        mainmenuanimation.SetActive(true);
        MainMenu2VideoPlayer.Prepare();
        yield return new WaitForSeconds(3f);
        mainmenuanimation.SetActive(false);
        mainmenu.SetActive(true);
    }

    public IEnumerator SettingsMenuAnimation()
    {
        MainMenu2VideoPlayer.Play();
        mainmenuanimation2.SetActive(true);
        MainMenuVideoPlayer.Prepare();
        yield return new WaitForSeconds(3f);
        mainmenuanimation2.SetActive(false);
        settingmenu.SetActive(true);
    }
    private void Update()
    {
        KeycodeInput();
    }
    public void InitialKeyCode(List<string> keylist)
    {
        var player = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerBehaviour>();
        if(player != null)
        {
            for(int i = 0; i < keycodeInputField.Count; i++)
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
                        foreach(var key in keylist)
                        {
                            if(player.meleeWeaponKey.ToString() == key)
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
        foreach(var key in keycodeInputlist)
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
        for(int i = 0; i < 9; i++)
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
    } */

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
                for(int i = 0; i < indexlist.Count; i++)
                {
                    keycodeInputField[i].inputText.text = indexlist[i];
                }
                break;
        }
        KeyCodeSaveBtn();
    }

    

    public void openSettingMenu()
    {
        MainMenu2VideoPlayer.Prepare();
        if (MainMenu2VideoPlayer.isPrepared)
        {
            StartCoroutine(SettingsMenuAnimation());
        }
    }
    public void openMainMenu()
    {
        MainMenuVideoPlayer.Prepare();
        if (MainMenuVideoPlayer.isPrepared)
        {
            StartCoroutine(MainMenuAnimation());
        }
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OpenPopOutDialog(string PopOutType)
    {
        
        switch(PopOutType)
        {
            case "NewGame":
                NewGamemodalWindow.UpdateUI();
                NewGamemodalWindow.Open();
                break;
            case "LoadGame":
                LoadGamemodalWindow.UpdateUI();
                LoadGamemodalWindow.Open();
                break;
            case "NoLoadGame":
                NoSavedGameNotification.UpdateUI();
                NoSavedGameNotification.Open();
                mainmenu.SetActive(true);
                break;

        }

    }

    public void ClosePopOutDialog(string PopOutType)
    {

        switch (PopOutType)
        {
            case "NewGame":
                NewGamemodalWindow.Close();
                break;
            case "LoadGame":
                LoadGamemodalWindow.Close();
                break;
            case "NoLoadGame":
                NoSavedGameNotification.Close();
                break;
        }

    }


    public void NewGameYesClicked()
    {
        PlayerPrefs.SetInt("loadscene", newGameLevel);

        SceneManager.LoadScene("Loading");
    }

    public void LoadGameYesClicked()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            leveltoLoad = PlayerPrefs.GetInt("SavedLevel");
            if(leveltoLoad != -1)
            {
                PlayerPrefs.SetInt("loadscene", leveltoLoad);
                SceneManager.LoadScene("Loading");
            }
            else
            {
                OpenPopOutDialog("NoLoadGame");
            }
        }
    }
    public void QuitBtnClicked()
    {
        Application.Quit();
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
}
