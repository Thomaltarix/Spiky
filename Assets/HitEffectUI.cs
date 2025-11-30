using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitEffectUI : MonoBehaviour
{
    [Header("UI Image pour l'effet")]
    public Image hitImage;

    [Header("Réglages")]
    public float maxAlpha = 0.5f;
    public float fadeInDuration = 0.1f;
    public float fadeOutDuration = 0.4f;

    private Coroutine flashRoutine;

    public void PlayHitEffect()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        Color c = hitImage.color;

        // ------------ FADE IN ------------
        float t = 0;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0, maxAlpha, t / fadeInDuration);
            hitImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        // ------------ FADE OUT ------------
        t = 0;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(maxAlpha, 0, t / fadeOutDuration);
            hitImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        // Reset final
        hitImage.color = new Color(c.r, c.g, c.b, 0);
    }
}
