using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Interactable : MonoBehaviour
{
    [Header("Setting")]
    public bool interactable = true;
    public bool requireKey = false;
    [SerializeField] private string keyName;
    [SerializeField] private List<string> keyNames;
    [SerializeField] private UnityEvent interactAction, enterRangeAction, leaveRangeAction;

    [Header("Reference")]
    [SerializeField] private GameObject dialogObjectReference;

    //Runtime data
    private PlayerBehaviour player;
    private GameObject dialogObject;
    private TMP_Text dialogText;
    private bool playerInRange;

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
        //Get player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by interactable.cs)");
        }

        if(player != null)
        {
            CheckPlayerInRange();

            if (interactable)
            {
                SetHintText();
            }
        }
    }

    private void CheckPlayerInRange()
    {
        if (playerInRange && interactable)
        {
            if (Input.GetKeyDown(player.interactKey))
            {
                if (requireKey)
                {
                    if (HaveKey())
                    {
                        interactAction.Invoke();
                    }
                    else
                    {
                        player.SetDialog(new string[] { $"(You need a {keyName} to open this.)" });
                    }
                }
                else
                {
                    interactAction.Invoke();
                }
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

        if (keyName != "")
        {
            int indexOfKeyList = -1;

            foreach (var key in player.keyList)
            {
                if (key.key.Name == keyName)
                {
                    haveKey = true;
                    indexOfKeyList = player.keyList.IndexOf(key);
                }
            }
            if (haveKey)
            {
                player.keyList[indexOfKeyList].quantity--;
            }
        }

        if (keyNames.Count != 0)
        {
            List<int> indexOfKeyList = new();

            foreach (string keyName in keyNames)
            {
                foreach (var key in player.keyList)
                {
                    if (key.key.Name == keyName)
                    {
                        indexOfKeyList.Add(player.keyList.IndexOf(key));
                    }
                }
                if (indexOfKeyList.Count == keyNames.Count)
                {
                    haveKey = true;
                }
            }
            if (haveKey)
            {
                foreach (int index in indexOfKeyList)
                {
                    player.keyList[index].quantity--;
                }
            }
        }

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
