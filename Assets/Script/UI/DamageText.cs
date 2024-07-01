using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float timeToLive = 0.5f;
    [SerializeField] private float floatSpeed = 500;
    [SerializeField] private Vector3 floatingDir = new(0, 1, 0);

    [Header("Object Reference")]
    [SerializeField] private TextMeshProUGUI textmesh;
    [SerializeField] private RectTransform rtransform;

    [Header("Dynamic Data")]
    [SerializeField] private Color starting_Color;
    [SerializeField] float timeElapsed = 0.0f;



    private void Start()
    {
        starting_Color = textmesh.color;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        rtransform.position += floatSpeed * Time.deltaTime * floatingDir;
        textmesh.color = new Color(starting_Color.r, starting_Color.g, starting_Color.b, 1 - (timeElapsed / timeToLive));
        if (timeElapsed >= timeToLive)
        {
            Destroy(gameObject);
        }
    }

    public void SetContent(float value, string type)
    {
        Start();

        switch (type)
        {
            case "Damage":
                textmesh.text = Mathf.RoundToInt(value).ToString();
                textmesh.color = new Color(255, 255, 255, 255);
                textmesh.outlineWidth = 0f;
                break;
            case "DamageCrit":
                textmesh.text = Mathf.RoundToInt(value).ToString();
                textmesh.color = new Color(255, 255, 0, 255);
                textmesh.outlineColor = new Color(255, 0, 0, 255);
                textmesh.outlineWidth = 0.4f;
                break;
            case "Heal":
                textmesh.text = Mathf.RoundToInt(value).ToString();
                textmesh.color = new Color(0, 150, 0, 255);
                textmesh.outlineWidth = 0f;
                break;
            case "PlayerHit":
                textmesh.text = Mathf.RoundToInt(value).ToString();
                textmesh.color = new Color(150, 0, 0, 255);
                textmesh.outlineWidth = 0f;
                break;
        }
    }

    public static void InstantiateDamageText(GameObject damageText, Vector3 position, float value, string type)
    {
        var damageTextInstantiated = Instantiate(
            damageText,
            Camera.main.WorldToScreenPoint(position),
            Quaternion.identity,
            GameObject.Find("ScreenUI").transform
            ).GetComponent<DamageText>();

        damageTextInstantiated.SetContent(value, type);
    }
}
