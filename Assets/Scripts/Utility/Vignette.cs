using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vignette : MonoBehaviour {

    #region Public Fields
    [Header("Effect Settings")]
    /// <summary>
    /// Screen coverage at max angular velocity.
    /// </summary>
    [Range(0f, 1f)]
    [Tooltip("Screen coverage at max angular velocity.\n(1-this) is radius of visible area at max effect (screen space).")]
    public float maxEffect = 0.5f;

    /// <summary>
    /// Feather around cut-off as fraction of screen.
    /// </summary>
    [Range(0f, 0.5f)]
    [Tooltip("Feather around cut-off as fraction of screen.")]
    public float feather = 0.1f;

    /// <summary>
    /// Smooth out radius over time. 0 for no smoothing.
    /// </summary>
    [Tooltip("Fade out radius over time.")]
    public float fadeOutTime = 3f;
    #endregion

    #region Smoothing
    private float _avSlew;
    private float _av;
    #endregion

    #region Shader property IDs
    private int _propTint;
    private int _propAV;
    private int _propFeather;
    #endregion

    #region Misc Fields
    private float av;
    private Vector3 _lastFwd;
    private Material _m;
    private float timer;
    private bool isShowing;
    #endregion

    #region Messages
    void Awake()
    {
        _m = new Material(Shader.Find("Hidden/Tunnelling"));

        _propAV = Shader.PropertyToID("_AV");
        _propFeather = Shader.PropertyToID("_Feather");
        _propTint = Shader.PropertyToID("_VignetteColor");
    }

    void Update()
    {
        if (isShowing)
        {
            timer += Time.deltaTime;

            av -= maxEffect / (fadeOutTime * 90f);

            if (av < 0f || timer > fadeOutTime)
            {
                av = 0f;
                isShowing = false;
                timer = 0f;
            }

            _m.SetFloat(_propAV, av);
            _m.SetFloat(_propFeather, feather);
        }
    }

    public void ShowGetHitVignette(float amount)
    {
        isShowing = true;
        timer = 0f;

        av = amount;
        maxEffect = amount;
        _m.SetFloat(_propAV, av);
        _m.SetFloat(_propFeather, feather);
    }

    public void ShowDieVignette()
    {
        isShowing = true;
        timer = 0f;

        av = 1f;

        _m.SetColor(_propTint, Color.black);
        _m.SetFloat(_propAV, av);
        _m.SetFloat(_propFeather, feather);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, _m);
    }

    void OnDestroy()
    {
        Destroy(_m);
    }
    #endregion
}
