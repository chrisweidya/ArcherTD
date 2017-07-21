using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CameraFilterSwap : MonoBehaviour {

    [SerializeField] private PostProcessingBehaviour _cameraPostProcess;
    [SerializeField] private PostProcessingProfile _defaultProfile;
    [SerializeField] private PostProcessingProfile _grayscaleProfile;
    [SerializeField] private Fade _fadeScript;
    [SerializeField] private Text _respawnText;

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
        if (PlayerHandler.LocalWardenNetId == id && isDead) {
            StopAllCoroutines();
            StartCoroutine(StartDeathEffect());
        }
    }

    private IEnumerator StartDeathEffect() {
        StartCoroutine(StartCounter());
        _cameraPostProcess.profile = _grayscaleProfile;
        yield return new WaitForSeconds(PlayerHandler.RespawnTimeSecs - 3);
        _fadeScript.StartFadeOut(1, null);
        yield return new WaitForSeconds(2);
        _cameraPostProcess.profile = _defaultProfile;
        _fadeScript.StartFadeIn(1);
    }

    private IEnumerator StartCounter() {
        for(int i = (int)PlayerHandler.RespawnTimeSecs - 1; i>=0; i--) {
            _respawnText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        _respawnText.text = " ";
        yield return null;
    }
}
