﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager> {

    public enum Factions {Legion, Hellbourne};
    public enum Scenes { MatchMaking, Lobby};

    public static bool GameWon = false;
    private static NetworkConnection ServerConnection;

    private GameObject _cameraRigGO = null;
    private GameObject _cameraPlayer = null;
    [SerializeField] private string _currentScene;

    private static GameManager.Factions localPlayerFaction;

    protected override void Awake() {
        base.Awake();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        print("Scene Loaded.");
        _cameraRigGO = GameObject.Find("Player");
        if (_cameraRigGO == null)
            print("Vive camera not found!");
        _currentScene = SceneManager.GetActiveScene().name;
    }

    private void SetCameraPos(Vector3 pos) {
        _cameraRigGO.transform.position = pos;
    }

    public void AssignCamera(GameObject player, Vector3 pos) {
        _cameraPlayer = player;
        SetCameraPos(pos);
        player.transform.parent = _cameraRigGO.transform.Find("SteamVRObjects/VRCamera");
        if(player.transform.parent == null) {
            print("VR Camera not found, Vive not connected? Fallback.");
            player.transform.parent = _cameraRigGO.transform.Find("NoSteamVRFallbackObjects/FallbackObjects");
        }
        player.transform.position = player.transform.parent.position;
        player.transform.rotation = player.transform.parent.rotation;
    }

    private void SetSceneProperties(string sceneName) {

    }

    public static void SetOnceServerConnection(NetworkConnection connection) {
        if (ServerConnection == null)
            ServerConnection = connection;
        print("here" +connection);
    }

    public static NetworkConnection GetServerConnection() {
        if (ServerConnection == null) {
            Debug.LogError("No server connection initialized.");
            return null;
        }
        else
            return ServerConnection;
    }

    public static void SetLocalPlayerFaction(GameManager.Factions faction) {
        localPlayerFaction = faction;
        GameObject.Find("TeleporterPointManager").GetComponent<TeleportPointManager>().TeleporterSetUp();
    }

    public static GameManager.Factions GetLocalPlayerFaction( ) {
       return localPlayerFaction;
    }

    public string GetCurrentSceneName() {
        return _currentScene;
    }
}
