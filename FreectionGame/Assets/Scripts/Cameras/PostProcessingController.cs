using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PostProcessProfile profile;

    [Header("Vignette")]
    Vignette v;
    [SerializeField] private float minVignetteIntensity;
    [SerializeField] private float maxVignetteIntensity;
    [SerializeField] private float vignetteCurveLog;
    [SerializeField] private AnimationCurve curveVignette;

    bool slowed = false;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color slowedColor;


    [Header("Lens")]
    LensDistortion l;
    [SerializeField] private float minLensIntensity;
    [SerializeField] private float maxLensIntensity;
    [SerializeField] private float lensCurveLog;
    [SerializeField] private AnimationCurve curveLens;


    // Start is called before the first frame update
    void Awake()
    {
        v = profile.GetSetting<Vignette>();
        l = profile.GetSetting<LensDistortion>();
        v.color.value = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVignette();
        UpdateLens();
    }

    private void UpdateVignette()
    {
        if (slowed) return;
        float t = Mathf.Log(rb.velocity.magnitude, vignetteCurveLog);
        Debug.Log("vign " + t);
        v.intensity.value = Mathf.Lerp(curveVignette.Evaluate(t), minVignetteIntensity, maxVignetteIntensity);
    }
    private void UpdateLens()
    {
        float t = Mathf.Log(rb.velocity.magnitude, lensCurveLog);
        //Debug.Log("lens " + t);
        l.intensity.value = Mathf.Lerp(curveLens.Evaluate(t), minLensIntensity, maxLensIntensity);
    }

    public void EnterSlowZone()
    {
        Debug.Log("slowed");
        slowed = true;
        v.color.value = slowedColor;
        v.intensity.value = maxVignetteIntensity;
    }
    public void ExitSlowZone()
    {
        Debug.Log("unslowed");
        slowed = false;
        v.color.value = defaultColor;

    }

}
