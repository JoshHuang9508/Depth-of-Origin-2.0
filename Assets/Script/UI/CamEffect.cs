using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CamEffect : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private Animator healAnimator;
    [SerializeField] private Animator crossfadeAnimator;
    [SerializeField] private PlayerBehaviour player;

    public enum CamEffectType
    {
        Heal, Hit, CrossfadeIn, CrossfadeOut
    }

    public void PlayCamEffect(CamEffectType type)
    {
        switch (type)
        {
            case CamEffectType.Heal:
                healAnimator.SetTrigger("Heal");
                break;
            case CamEffectType.Hit:
                hitAnimator.SetTrigger("Hit");
                break;
            case CamEffectType.CrossfadeIn:
                crossfadeAnimator.SetTrigger("Start");
                break;
            case CamEffectType.CrossfadeOut:
                crossfadeAnimator.SetTrigger("End");
                break;
        }
    }
}
