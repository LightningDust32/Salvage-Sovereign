using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume sceneVolume;
    [SerializeField] AnimationCurve effectCurve;

    public static ProcessingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Bloom bloom;
    private MotionBlur motionBlur;
    private ShadowsMidtonesHighlights shadowsMidtonesHighlights;
    private LiftGammaGain liftGammaGain;
    private Vignette vignette;

    // Default values
    private float defaultBloomIntensity;
    private float defaultMotionBlurIntensity;
    private float defaultVignetteIntensity;

    private Vector4 defaultShadows;
    private Vector4 defaultMidtones;
    private Vector4 defaultHighlights;

    private Vector4 defaultLift;
    private Vector4 defaultGamma;
    private Vector4 defaultGain;


    private void Start()
    {
        if (sceneVolume == null || sceneVolume.profile == null)
        {
            Debug.LogError("ProcessingManager missing volume profile");
            return;
        }

        sceneVolume.profile.TryGet(out bloom);
        sceneVolume.profile.TryGet(out motionBlur);
        sceneVolume.profile.TryGet(out shadowsMidtonesHighlights);
        sceneVolume.profile.TryGet(out liftGammaGain);
        sceneVolume.profile.TryGet(out vignette);

        CacheDefaults();
    }

    private void CacheDefaults()
    {
        if (bloom != null)
            defaultBloomIntensity = bloom.intensity.value;

        if (motionBlur != null)
            defaultMotionBlurIntensity = motionBlur.intensity.value;

        if (vignette != null)
            defaultVignetteIntensity = vignette.intensity.value;

        if (shadowsMidtonesHighlights != null)
        {
            defaultShadows = shadowsMidtonesHighlights.shadows.value;
            defaultMidtones = shadowsMidtonesHighlights.midtones.value;
            defaultHighlights = shadowsMidtonesHighlights.highlights.value;
        }

        if (liftGammaGain != null)
        {
            defaultLift = liftGammaGain.lift.value;
            defaultGamma = liftGammaGain.gamma.value;
            defaultGain = liftGammaGain.gain.value;
        }
    }

    public void PulseBloom(float targetIntensity, float duration)
    {
        if (bloom == null) return;

        StopCoroutine(nameof(BloomRoutine));
        StartCoroutine(BloomRoutine(targetIntensity, duration));
    }

    public void PulseVignette(float targetIntensity, float duration)
    {
        if (vignette == null) return;

        StopCoroutine(nameof(VignetteRoutine));
        StartCoroutine(VignetteRoutine(targetIntensity, duration));
    }

    private IEnumerator BloomRoutine(float target, float duration)
    {
        float start = bloom.intensity.value;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            float curved = effectCurve.Evaluate(t);

            bloom.intensity.value = Mathf.Lerp(start, target, curved);

            yield return null;
        }

        timer = 0f;
        start = bloom.intensity.value;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            float curved = effectCurve.Evaluate(t);

            bloom.intensity.value = Mathf.Lerp(start, defaultBloomIntensity, curved);

            yield return null;
        }

        bloom.intensity.value = defaultBloomIntensity;
    }
    
    private IEnumerator VignetteRoutine(float target, float duration)
    {
        float start = vignette.intensity.value;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            float curved = effectCurve.Evaluate(t);

            vignette.intensity.value = Mathf.Lerp(start, target, curved);

            yield return null;
        }

        timer = 0f;
        start = vignette.intensity.value;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            float curved = effectCurve.Evaluate(t);

            vignette.intensity.value = Mathf.Lerp(start, defaultVignetteIntensity, curved);

            yield return null;
        }

        vignette.intensity.value = defaultVignetteIntensity;
    }
}
