using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

public class Dialog : MonoBehaviour, IInterface
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI dialogText;

    public bool IsActive { get; set; }

    public void Toggle()
    {
        if (IsActive) Close();
        else Open();
    }

    public void Open()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public async Task ShowDialog(string name, string[] dialog)
    {
        gameObject.SetActive(true);
        IsActive = true;
        NameText.text = name;

        // Show dialog
        foreach (var text in dialog)
        {
            dialogText.text = text;
            await Task.Delay(1000);

            // Wait for click
            while (!Input.GetKey(KeyCode.Mouse0))
            {
                await Task.Yield();
            }
        }

        gameObject.SetActive(false);
        IsActive = false;
    }
}

// This is a simple dialog class that holds the dialog name and the dialog texts
public class dialog
{
    public string dialogName;
    public string[] dialogTexts;

    public dialog(string dialogName, string[] dialogTexts)
    {
        this.dialogName = dialogName;
        this.dialogTexts = dialogTexts;
    }
}
