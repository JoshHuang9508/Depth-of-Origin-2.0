using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private NotificationManager notification;
    [SerializeField] private Sprite complete, hint;
    [SerializeField] private SpawnerController spawner;
    [SerializeField] private ChestController meleeChestController, rangeChestController, potionChestController;
    [SerializeField] private List<GameObject> potList, crateList;
    [SerializeField] private int progressNum;
    [SerializeField] private float timer = 0;
    bool enemyAlive = true;


    void Start()
    {
        //setup notification
        notification.Close();
        notification.title = "";
        notification.description = "";
        notification.enableTimer = false;

        //setup chest controller
        meleeChestController.interactable.interactable = true;
        rangeChestController.interactable.interactable = false;
        potionChestController.interactable.interactable = false;

        progressNum = 0;
    }

    void Update()
    {
        notification.UpdateUI();
        UpdateTimer();

        if (timer > 0) return;

        switch (progressNum)
        {
            case 0:
                notification.Close();
                SetHint("Hint", "Open the chest");
                notification.Open();

                progressNum = 1;
                break;

            case 1:
                if (meleeChestController.IsChestOpen)
                {
                    notification.Close();
                    SetHint("Complete", "Chest has been opened!");
                    notification.Open();

                    timer += 3;
                    progressNum = 2;
                }
                break;

            case 2:
                notification.Close();
                SetHint("Hint", "Press 'F' to open backpack and equip the sword. \n Press 'Alpha 1' to select melee weapon then use 'Mouse 0' to swing the sword.");
                notification.Open();

                progressNum = 3;
                break;

            case 3:
                if (crateList.All(item => item == null))
                {
                    notification.Close();
                    SetHint("Complete", "The crate has been broken!");
                    notification.Open();

                    timer += 3;
                    progressNum = 4;
                }
                break;

            case 4:
                notification.Close();
                SetHint("Hint", "Open the chest");
                notification.Open();

                rangeChestController.interactable.interactable = true;
                progressNum = 5;
                break;

            case 5:
                if (rangeChestController.IsChestOpen)
                {
                    notification.Close();
                    SetHint("Complete", "Chest has been opened!");
                    notification.Open();

                    timer += 3;
                    progressNum = 6;
                }
                break;

            case 6:
                notification.Close();
                SetHint("Hint", "Press 'F' to open backpack and equip the bow \n Press 'Alpha 2' to select ranged weapon then use 'Mouse 0' to shot the pots in the water");
                notification.Open();

                progressNum = 7;
                break;

            case 7:
                if (potList.All(item => item == null))
                {
                    notification.Close();
                    SetHint("Complete", "The pot has been broken!");
                    notification.Open();

                    timer += 3;
                    progressNum = 8;
                }
                break;

            case 8:
                notification.Close();
                SetHint("Hint", "Open the chest");
                notification.Open();

                potionChestController.interactable.interactable = true;
                progressNum = 9;
                break;

            case 9:
                if (potionChestController.IsChestOpen)
                {
                    notification.Close();
                    SetHint("Complete", "Chest has been opened!");
                    notification.Open();

                    timer += 3;
                    progressNum = 10;
                }
                break;

            case 10:
                spawner.SpawnMobs();

                notification.Close();
                SetHint("Hint", "Press 'F' to open backpack and equip the potions \n Press 'Alpha 3' to consume Potion then try to kill the enemy ahead");
                notification.Open();

                progressNum = 11;
                break;

            case 11:
                GameObject enemy = GameObject.FindWithTag("Enemy");
                try
                {
                    if (enemy != null) enemyAlive = true;
                    else enemyAlive = false;
                }
                catch { }

                if (!enemyAlive)
                {
                    notification.Close();
                    SetHint("Complete", "Enemy has been defeated!");
                    notification.Open();

                    timer += 3;
                    progressNum = 12;
                }
                break;

            case 12:
                SetHint("Hint", "Walk forward to the forest");
                notification.enableTimer = false;
                notification.Open();
                break;
        }
    }

    private void SetHint(string title, string description)
    {
        notification.title = title;
        notification.description = description;
        switch (title)
        {
            case "Complete":
                notification.icon = complete;
                break;
            case "Hint":
                notification.icon = hint;
                break;
        }
    }

    private void UpdateTimer()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0);
    }

    public void enterForest()
    {
        PlayerPrefs.SetString("loadscene", "Town");
    }
}
