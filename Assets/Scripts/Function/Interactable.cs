using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Interactable : MonoBehaviour
{
    [Header("Attributes")]
    public bool interactable = true;
    public bool requireKey = false;
    public List<ItemSO> allowedKeyList;

    [Header("Actions")]
    public UnityEvent interactAction, enterRangeAction, leaveRangeAction;

    [Header("Reference")]
    [SerializeField] private GameObject dialogObjectReference;
    [SerializeField] private PlayerBehaviour player;

    // Flags
    private bool playerInRange;

    // TODO: Implement dialog system

    // Remove this
    private GameObject dialogObject;
    private TMP_Text dialogText;

    private void Start()
    {
        if (interactable)
        {
            dialogObject = Instantiate(
            dialogObjectReference,
            transform.position + new Vector3(0, 1.5f, 0),
            Quaternion.identity,
            transform);
            dialogText = dialogObject.GetComponentInChildren<TMP_Text>();
            dialogObject.SetActive(false);
        }
    }
    private void Update()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerBehaviour>() ?? null;

        if (player != null)
        {
            CheckPlayerInRange();

            if (interactable)
            {
                SetHintText();
            }
        }
    }
    private async void CheckPlayerInRange()
    {
        if (playerInRange && interactable && Input.GetKeyDown(player.interactKey))
        {
            if (requireKey)
            {
                if (HaveKey()) interactAction.Invoke();
                else await player.SetDialog(new string[] { "(You need a key to open this.)" });
            }
            else
            {
                interactAction.Invoke();
            }
        }
    }
    private void SetHintText()
    {
        dialogObject.SetActive(playerInRange);
        dialogText.text = $"Press {player.interactKey} to interact";
    }
    private bool HaveKey()
    {
        bool haveKey = false;

        // if (allowedKeyNames.Count != 0)
        // {
        //     List<int> indexOfKeyList = new();

        //     foreach (string keyName in allowedKeyNames)
        //     {
        //         foreach (var key in player.keyList)
        //         {
        //             if (key.key.Name == keyName)
        //             {
        //                 indexOfKeyList.Add(player.keyList.IndexOf(key));
        //             }
        //         }
        //         if (indexOfKeyList.Count == allowedKeyNames.Count)
        //         {
        //             haveKey = true;
        //         }
        //     }
        //     if (haveKey)
        //     {
        //         foreach (int index in indexOfKeyList)
        //         {
        //             player.keyList[index].quantity--;
        //         }
        //     }
        // }

        return haveKey;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            enterRangeAction.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            leaveRangeAction.Invoke();
        }
    }
}
