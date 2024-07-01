using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private bool skipable;
    [SerializeField] private float skipTimer;
    [SerializeField] private float timer;
    [SerializeField] private int stage;
    [SerializeField] private int dialogCount;
    [SerializeField] private string[] dialog1 = new string[] 
    {
        "...Where is here?",
        "...",
        "It's not the place I've been before.",
        "(Some voice is echoing in your head.)",
        "Damn it, it's annoying.",
        "Whatever, I should move right now.",
        "(Use W,A,S,D to control your character.)"
    };
    [SerializeField] private string[] dialog2 = new string[]
    {
        "...A chest?",
        "(Get close to the chest and press F to open it.)"
    };
    [SerializeField] private string[] dialog3 = new string[]
    {
        "Sword...maybe I can use it to break the barrier.",
        "...But how do I hold it?",
        "(Press F to open backpack, and equip the sword.)",
        "(Press 1 to take out melee weapon.)",
        "(Use Mouse0 to swing melee weapon.)"
    };
    [SerializeField] private string[] dialog4 = new string[]
    {
        "...",
        "Another chest?",
        "(Open the chest.)"
    };
    [SerializeField] private string[] dialog5 = new string[]
    {
        "A Bow...it's been a long time since I use a bow.",
        "Seems like some pots are in the lake.",
        "Maybe I can shoot them for practice.",
        "(Press F to open backpack, and equip the bow.)",
        "(Press 2 to take out ranged weapon.)",
        "(Move your mouse to aim, then use Mouse0 to shot arrows.)"
    };
    [SerializeField] private string[] dialog6 = new string[]
    {
        "Chest Again...",
        "(Open the chest.)"
    };
    [SerializeField] private string[] dialog7 = new string[]
    {
        "Potions? Looks yuck...",
        "How does it taste like?",
        "(You drink a litte liquid which in the pots, then feel a strong impact coming into your head.)",
        "Holy F***, what the hell is that feel?",
        "(You feel more energetic.)",
        "It really effect me somehow...",
        "(Use potions appropriately can help you easily defeating enemies.)",
        "I think I should take some potions.",
        "(Press F to open backpack, and equip the potions.)",
        "(You can press 3 to consume potions which are equipped or consume them in backpack.)",
        "'Errrrrr...'",
        "What is that sound?!",
        "(A monster appeared! Defeat it before go ahead.)"
    };
    [SerializeField] private string[] dialog8 = new string[]
    {
        "Oh my lord, it was close",
        "What was that creature? It was horrible!",
        "I really should move on quick before I got eaten by them.",
        "(Move to the forest.)",
        "(Instruction chapter are completed, you can find it and more detail by pressing I. GLHF!)"
    };

    [SerializeField] private ChestController meleeWeaponChest;
    [SerializeField] private ChestController rangedWeaponChest;
    [SerializeField] private ChestController potionChest;
    [SerializeField] private Collider2D trigger1;
    [SerializeField] private Collider2D trigger2;
    [SerializeField] private Collider2D trigger3;
    [SerializeField] private Collider2D trigger4;
    [SerializeField] private SpawnerController spawner;



    private void Start()
    {
        stage = 0;
        dialogCount = 0;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        meleeWeaponChest.GetComponent<Interactable>().enabled = false;
        rangedWeaponChest.GetComponent<Interactable>().enabled = false;
        potionChest.GetComponent<Interactable>().enabled = false;
        trigger4.GetComponent<Interactable>().enabled = false;
    }

    private void Update()
    {
        UpdateTimer();

        switch (stage)
        {
            case 0:
                player.SetBehaviorEnabler(false);
                player.SetDialog(dialog1[dialogCount]);
                if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if(dialogCount < dialog1.Length - 1)
                    {
                        dialogCount++;
                    }
                    else
                    {
                        stage = 1;
                        dialogCount = 0;
                        player.SetBehaviorEnabler(true);
                        player.ClearDialog();
                    }
                }
                break;
            case 1:
                if (skipable && (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0))
                {
                    timer = 5;
                    stage = 2;
                }
                break;
            case 2:
                if (timer > 0) break;
                player.SetBehaviorEnabler(false);
                player.SetDialog(dialog2[dialogCount]);
                if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (dialogCount < dialog2.Length - 1)
                    {
                        dialogCount++;
                    }
                    else
                    {
                        stage = 3;
                        dialogCount = 0;
                        player.SetBehaviorEnabler(true);
                        player.ClearDialog();
                    }
                }
                break;
            case 3:
                meleeWeaponChest.GetComponent<Interactable>().enabled = true;
                if (meleeWeaponChest.IsChestOpen)
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog3[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog3.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 4;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 4:
                if(trigger1.IsTouching(player.GetComponent<Collider2D>()))
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog4[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog4.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 5;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 5:
                rangedWeaponChest.GetComponent<Interactable>().enabled = true;
                if (rangedWeaponChest.IsChestOpen)
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog5[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog5.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 6;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 6:
                if (trigger2.IsTouching(player.GetComponent<Collider2D>()))
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog6[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog6.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 7;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 7:
                potionChest.GetComponent<Interactable>().enabled = true;
                if (potionChest.IsChestOpen)
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog7[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog7.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 8;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 8:
                spawner.SpawnMobs();
                stage = 9;
                break;
            case 9:
                if (trigger3.IsTouching(player.GetComponent<Collider2D>()))
                {
                    player.SetBehaviorEnabler(false);
                    player.SetDialog(dialog8[dialogCount]);
                    if (skipable && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (dialogCount < dialog8.Length - 1)
                        {
                            dialogCount++;
                        }
                        else
                        {
                            stage = 10;
                            dialogCount = 0;
                            player.SetBehaviorEnabler(true);
                            player.ClearDialog();
                        }
                    }
                }
                break;
            case 10:
                trigger4.GetComponent<Interactable>().enabled = true;
                break;
        }
    }

    private void UpdateTimer()
    {
        skipTimer = Mathf.Max(0, skipTimer - Time.deltaTime);
        timer = Mathf.Max(0, timer - Time.deltaTime);

        skipable = skipTimer <= 0;
    }
}
