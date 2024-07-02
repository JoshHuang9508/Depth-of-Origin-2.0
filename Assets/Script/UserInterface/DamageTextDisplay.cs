using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextDisplay : MonoBehaviour
{
    [Header("Setting")]
    public float existTime = 0.5f;
    public float floatSpeed = 500;
    public Vector3 floatingDir = new(0, 1, 0);

    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI textmesh;
    [SerializeField] private RectTransform rtransform;

    //Runtime Data
    private float timeElapsed = 0.0f;

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        rtransform.position += floatSpeed * Time.deltaTime * floatingDir;
        textmesh.color = new Color(textmesh.color.r, textmesh.color.g, textmesh.color.b, 1 - (timeElapsed / existTime));
        if (timeElapsed >= existTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetDisplay(float value, string type)
    {
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
}
