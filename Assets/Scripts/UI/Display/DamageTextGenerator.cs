using System.Collections;
using UnityEngine;
using TMPro;

public class DamageTextGenerator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject damageTextModel;

    // Static
    private static DamageTextGenerator GDamageTextGenerator;

    private void Start()
    {
        GDamageTextGenerator = this;
    }
    public static void SetDamageText(Vector3 position, float value, DamageTextDisplay.DamageTextType type)
    {
        var damageText = Instantiate(
            GDamageTextGenerator.damageTextModel,
            position,
            Quaternion.identity,
            GameObject.FindWithTag("Item").transform
            );

        damageText.GetComponent<DamageTextDisplay>().Initialize(value, type);
    }
}
