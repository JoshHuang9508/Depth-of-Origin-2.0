using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private UIItemSlot itemSlot;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        itemSlot = GetComponentInChildren<UIItemSlot>();
    }

    public void SetData(Sprite sprite,int quantity)
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
