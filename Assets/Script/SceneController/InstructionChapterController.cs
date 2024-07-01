using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class InstructionChapterController : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private float timer;
    [SerializeField] private string[] dialog1 = new string[] 
    {
        "... Where is here?",
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
        "(Get close to the chest and press E to open it.)"
    };
    [SerializeField] private string[] dialog3 = new string[]
    {
        "Sword... maybe I can use it to break the barrier.",
        "... But how do I hold it?",
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
        "A Bow... it's been a long time since I use a bow.",
        "Seems like some pots are in the lake.",
        "Maybe I can shoot them for practice.",
        "(Press F to open backpack, and equip the bow.)",
        "(Press 2 to take out ranged weapon.)",
        "(Move your mouse to aim, then use Mouse0 to shoot arrows.)"
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
        "(You drink a litte liquid which in the pots.)",
        "(A strong impact coming into your head.)",
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

    [Header("Object Reference")]
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
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        meleeWeaponChest.GetComponent<Interactable>().enabled = false;
        rangedWeaponChest.GetComponent<Interactable>().enabled = false;
        potionChest.GetComponent<Interactable>().enabled = false;
        trigger4.GetComponent<Interactable>().enabled = false;

        StartDialogue();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private async void StartDialogue() 
    {
        await player.SetDialog(dialog1);
        await Task.Delay(3000);

        await player.SetDialog(dialog2);

        meleeWeaponChest.GetComponent<Interactable>().enabled = true;
        await IsChestOpen(meleeWeaponChest);
        await player.SetDialog(dialog3);

        await IsTriggered(trigger1);
        await player.SetDialog(dialog4);

        rangedWeaponChest.GetComponent<Interactable>().enabled = true;
        await IsChestOpen(rangedWeaponChest);
        await player.SetDialog(dialog5);

        await IsTriggered(trigger2);
        await player.SetDialog(dialog6);

        potionChest.GetComponent<Interactable>().enabled = true;
        await IsChestOpen(potionChest);
        await player.SetDialog(dialog7);
        spawner.SpawnMobs();

        await IsTriggered(trigger3);
        await player.SetDialog(dialog8);
        trigger4.GetComponent<Interactable>().enabled = true;
    }

    private void UpdateTimer()
    {
        timer = Mathf.Max(0, timer - Time.deltaTime);
    }

    private async Task IsChestOpen(ChestController chest)
    {
        while (!chest.IsChestOpen)
        {
            await Task.Yield();
        }
    }

    private async Task IsTriggered(Collider2D trigger)
    {
        while (!trigger.IsTouching(player.GetComponent<Collider2D>()))
        {
            await Task.Yield();
        }
    }
}
