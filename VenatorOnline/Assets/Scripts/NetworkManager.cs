using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPoses;
    private CinemachineFreeLook _cameraFreeLook1;

    void Awake()
    {
        Vector3 randomPos = spawnPoses[Random.Range(0, spawnPoses.Length)].position;
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);

        player.tag = "Player";
        _cameraFreeLook1 = GameObject.FindGameObjectWithTag("CMFreeLook1").GetComponent<CinemachineFreeLook>();
        _cameraFreeLook1.Follow = player.transform;
        _cameraFreeLook1.LookAt = player.transform;
        _cameraFreeLook1.GetRig(0).LookAt = player.GetComponentsInChildren<Transform>()[1];
        _cameraFreeLook1.GetRig(1).LookAt = player.GetComponentsInChildren<Transform>()[2];
        _cameraFreeLook1.GetRig(2).LookAt = player.GetComponentsInChildren<Transform>()[2];
    }

    void Update()
    {

    }

    public override void OnLeftRoom() // текущий игрок (мы) покидает комнату
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + " зашёл в комнату.");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " покинул комнату.");
    }
}