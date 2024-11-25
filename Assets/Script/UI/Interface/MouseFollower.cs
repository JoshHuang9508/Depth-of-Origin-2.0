using UserInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private ItemSlot itemSlot;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        itemSlot = GetComponentInChildren<ItemSlot>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        itemSlot.SetData(sprite, quantity);
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out Vector2 position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
