﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]

public class AdjustLight : MonoBehaviour {

    [Range(0, 2)] [SerializeField] float winLightIntensity;
    [Range(0, 2)] [SerializeField] float loseLightIntensity;
    [SerializeField] Material skybox;
    private float _originalIntensity;

    private Light _light;
    
    private void Awake() {
        _light = GetComponent<Light>();
        _originalIntensity = _light.intensity;
    }

    private void OnEnable() {
        EventManager.GameEndAction += AdjustLights;
    }

    private void OnDisable() {
        EventManager.GameEndAction -= AdjustLights;
        if(skybox)
            skybox.SetFloat("_Exposure", _originalIntensity);
    }

    private void AdjustLights(GameManager.Factions faction) {
        if(PlayerHandler.LocalFaction == faction) {
            StartCoroutine(ChangeIntensity(false));
        }
        else {
            StartCoroutine(ChangeIntensity(true));
        }
    }

    private IEnumerator ChangeIntensity(bool win) {
        float time = 2;
        float startTime = Time.time;
        float endTime = startTime + time;
        float startIntensity = _light.intensity;
        float intensityDiff;
        if (win) {
            intensityDiff = winLightIntensity - startIntensity;
        }
        else    
            intensityDiff = loseLightIntensity - startIntensity;
        while (Time.time < endTime) {
            _light.intensity = startIntensity + (Time.time - startTime) / time * intensityDiff;
            if(skybox)
                skybox.SetFloat("_Exposure", _light.intensity);
            yield return new WaitForEndOfFrame();
        }
    }
}
