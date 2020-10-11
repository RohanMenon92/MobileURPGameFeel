using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class IgnitionScript : MonoBehaviour
{
    VisualEffect visualEffect;
    Sequence effectSequence;
    // Start is called before the first frame update

    void Start()
    {
        visualEffect = this.GetComponent<VisualEffect>();
    }

    public void SetColor(Color color)
    {
        visualEffect.SetVector4("Color", new Vector4(color.r, color.g, color.b, 1.0f));
    }

    public void FadeIn()
    {
        if (visualEffect == null)
        {
            visualEffect = this.GetComponent<VisualEffect>();
        }

        visualEffect.Stop();
        visualEffect.SetFloat("Rate", 0f);
        effectSequence = DOTween.Sequence();
        effectSequence.Insert(0f, DOTween.To(() => visualEffect.GetFloat("Rate"), x => visualEffect.SetFloat("Rate", x), 1.0f, GameConstants.effectsTime * 0.2f));
        effectSequence.Append(DOTween.To(() => visualEffect.GetFloat("Rate"), x => visualEffect.SetFloat("Rate", x), 0f, GameConstants.effectsTime * 0.2f).SetDelay(GameConstants.effectsTime * 0.6f));
        //effectSequence.AppendInterval(2.0f);
        visualEffect.Play();
        effectSequence.Play();
    }

    public void FadeAway()
    {
        FindObjectOfType<GameManager>().ReturnEffectToPool(gameObject, GameConstants.EffectTypes.Ignition);
    }
}
