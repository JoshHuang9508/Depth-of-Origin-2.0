using System.Collections;
using UnityEngine;
using TMPro;

public class DamageTextDisplay : MonoBehaviour
{
    [Header("Attributes")]
    public float existTime = 0.5f;
    public float floatSpeed = 500;
    public Vector3 floatingDir = new(0, 1, 0);

    [Header("Reference")]
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private RectTransform rectTransform;

    // Enum
    public enum DamageTextType
    {
        Damage, DamageCrit, Heal, PlayerHit, Dodge
    }

    private void Start()
    {
        StartCoroutine(PlayAnim());
    }
    public void Initialize(float value, DamageTextType type)
    {
        switch (type)
        {
            case DamageTextType.Damage:
                textMesh.text = Mathf.RoundToInt(value).ToString();
                textMesh.color = new Color(255, 255, 255, 255);
                textMesh.outlineWidth = 0f;
                break;
            case DamageTextType.DamageCrit:
                textMesh.text = Mathf.RoundToInt(value).ToString();
                textMesh.color = new Color(255, 255, 0, 255);
                textMesh.outlineColor = new Color(255, 0, 0, 255);
                textMesh.outlineWidth = 0.4f;
                break;
            case DamageTextType.Heal:
                textMesh.text = Mathf.RoundToInt(value).ToString();
                textMesh.color = new Color(0, 150, 0, 255);
                textMesh.outlineWidth = 0f;
                break;
            case DamageTextType.PlayerHit:
                textMesh.text = Mathf.RoundToInt(value).ToString();
                textMesh.color = new Color(150, 0, 0, 255);
                textMesh.outlineWidth = 0f;
                break;
            case DamageTextType.Dodge:
                textMesh.text = "Dodged";
                textMesh.color = new Color(255, 255, 255, 100);
                textMesh.outlineWidth = 0f;
                break;
        }
    }
    public IEnumerator PlayAnim()
    {
        float timeElapsed = 0f;
        while (timeElapsed < existTime)
        {
            timeElapsed += Time.deltaTime;
            rectTransform.position += floatSpeed * Time.deltaTime * floatingDir;
            textMesh.color = new Color(
                textMesh.color.r,
                textMesh.color.g,
                textMesh.color.b,
                1 - (timeElapsed / existTime)
            );

            yield return null;
        }
        Destroy(gameObject);
    }
}
