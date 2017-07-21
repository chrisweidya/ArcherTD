using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Networking;

public class CameraFilterSwap : MonoBehaviour {

    [SerializeField] private PostProcessingBehaviour _cameraPostProcess;
    [SerializeField] private PostProcessingProfile _defaultProfile;
    [SerializeField] private PostProcessingProfile _grayscaleProfile;
    [SerializeField] private Fade _fadeScript;

    private void OnEnable() {
        EventManager.PlayerDeathAction += Die;
    }

    private void OnDisable() {
        EventManager.PlayerDeathAction -= Die;
    }

    private void Awake() {
        if (_cameraPostProcess == null || _fadeScript == null)
            Destroy(this);
    }
    
    private void Die(NetworkInstanceId id, bool isDead) {
        if(PlayerHandler.LocalWardenNetId == id && isDead)
            StartCoroutine(StartDeathEffect());
    }

    private IEnumerator StartDeathEffect() {
        _cameraPostProcess.profile = _grayscaleProfile;
        yield return new WaitForSeconds(PlayerHandler.RespawnTimeSecs - 3);
        _fadeScript.StartFadeOut(1, null);
        yield return new WaitForSeconds(2);
        _cameraPostProcess.profile = _defaultProfile;
        _fadeScript.StartFadeIn(1);
    }
}
