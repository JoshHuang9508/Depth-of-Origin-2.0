using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Interactable : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] public bool interactable;
    [SerializeField] private UnityEvent interactAction, enterRangeAction, leaveRangeAction;

    [Header("Object Reference")]
    [SerializeField] private GameObject dialogObjectReference;
    [SerializeField] private PlayerBehaviour player;

    [Header("Dynamic Data")]
    [SerializeField] private GameObject dialogObject;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private bool isInRange;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        if (interactable)
        {
            dialogObject = Instantiate(
            dialogObjectReference,
            transform.position + new Vector3(0, 1.5f, 0),
            Quaternion.identity,
            transform
            );

            dialogObject.SetActive(false);
            dialogText = dialogObject.GetComponentInChildren<TMP_Text>();
        }
    }

    

    

    void Update()
    {
        if (isInRange && this.enabled && interactable && Input.GetKeyDown(player.interactKey))
        {
            if (gameObject.GetComponent<KeyRequired>())
            {
                player.GetKeyList = GetComponent<KeyRequired>().DetectKey(player.GetKeyList);

                if (GetComponent<KeyRequired>().HaveKey)
                {
                    interactAction.Invoke();
                }
                else
                {
                    //warning text
                }
            }
            else
            {
                interactAction.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            enterRangeAction.Invoke();

            if (enabled && interactable)
            {
                dialogObject.SetActive(true);
                dialogText.text = $"Press {player.interactKey} to interact";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            leaveRangeAction.Invoke();

            if (interactable) dialogObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (interactable) dialogObject.SetActive(false);
    }
}
