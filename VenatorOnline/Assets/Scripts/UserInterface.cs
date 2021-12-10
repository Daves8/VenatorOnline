using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public Canvas canvas;
    public CinemachineFreeLook cameraFreeLook1;
    private Movement _playerMovement;
    private PhotonView _photonView;

    public bool pause;

    private void Start()
    {
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
        _photonView = _playerMovement.gameObject.GetComponent<PhotonView>();
        canvas.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pause = false;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        canvas.gameObject.SetActive(!canvas.gameObject.activeInHierarchy);
        if (canvas.gameObject.activeInHierarchy) // в меню
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraFreeLook1.enabled = false;
            _playerMovement.IsReadyToMove = false;
            _playerMovement.IsReadyToRotate = false;
            _playerMovement.IsReadyToRun = false;
            pause = true;
            //cameraFreeLook1.m_XAxis.m_InputAxisName = "";
            //cameraFreeLook1.m_YAxis.m_InputAxisName = "";
        }
        else // в игре
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraFreeLook1.enabled = true;
            _playerMovement.IsReadyToMove = true;
            _playerMovement.IsReadyToRotate = true;
            _playerMovement.IsReadyToRun = true;
            pause = false;
            //cameraFreeLook1.m_XAxis.m_InputAxisName = "Mouse X";
            //cameraFreeLook1.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Exit()
    {
        Application.Quit();
    }
}