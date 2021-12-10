using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public TextMeshProUGUI action;
    public TextMeshProUGUI target;
    public TextMeshProUGUI message;
    public Canvas infoUI;
    public Canvas dialoguesUI;
    public Canvas inventoryCanvas;

    private CinemachineFreeLook _cameraFreeLook1;
    private GameObject _player;
    private bool _isDialogue;
    private bool _canStartDialogue;

    private GameObject[] _allBoars;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _cameraFreeLook1 = GameObject.FindGameObjectWithTag("CMFreeLook1").GetComponent<CinemachineFreeLook>();
        infoUI.gameObject.SetActive(true);
        dialoguesUI.gameObject.SetActive(false);
        action.text = "";
        target.text = "";
        message.text = "";
        _isDialogue = false;
        _canStartDialogue = false;

        _allBoars = GameObject.FindGameObjectsWithTag("Boar");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var boar in _allBoars)
        {
            if (boar.GetComponent<Animal>()._die)
            {
                target.text = "";
                message.text = "Задание выполнено. Получено 50 золота.";
            }
        }
        

        if (Vector3.Distance(_player.transform.position, gameObject.transform.position) <= 2 && !_isDialogue)
        {
            //infoUI.gameObject.SetActive(true);
            action.text = "Нажмите F";
            _canStartDialogue = true;
        }
        else
        {
            //infoUI.gameObject.SetActive(false);
            action.text = "";
            _canStartDialogue = false;
        }

        if (_canStartDialogue && Input.GetButtonDown("Action"))
        {
            dialoguesUI.gameObject.SetActive(true);
            //infoUI.gameObject.SetActive(false);
            _isDialogue = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _cameraFreeLook1.enabled = false;
        }
    }

    public void StartQuest()
    {
        dialoguesUI.gameObject.SetActive(false);
        //infoUI.gameObject.SetActive(false);
        inventoryCanvas.gameObject.SetActive(false);
        _isDialogue = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cameraFreeLook1.enabled = true;

        target.text = "Охотиться на кабана";
    }

    public void StartTrade()
    {
        dialoguesUI.gameObject.SetActive(false);
        //infoUI.gameObject.SetActive(false);
        inventoryCanvas.gameObject.SetActive(true);
    }

    public void ExitTrade()
    {
        dialoguesUI.gameObject.SetActive(false);
        //infoUI.gameObject.SetActive(false);
        inventoryCanvas.gameObject.SetActive(false);
        _isDialogue = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cameraFreeLook1.enabled = true;
    }
}