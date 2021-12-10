using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameService : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameRoom;
    public TextMeshProUGUI info;

    public TextMeshProUGUI username;

    public TMP_InputField login;
    public TMP_InputField password;
    public TextMeshProUGUI validationInfo;

    public Button loginButton;
    public Button logoutButton;

    private string message;

    void Start()
    {
        validationInfo.text = "";
        message = "";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.9";
        if (PlayerPrefs.GetString("username") != "")
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
            PhotonNetwork.ConnectUsingSettings();
            login.gameObject.SetActive(false);
            password.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            username.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(true);
            validationInfo.text = "";
            username.text = PlayerPrefs.GetString("username");
            loginButton.interactable = true;
        }
        else
        {
            username.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(false);
            login.gameObject.SetActive(true);
            password.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
        }
    }

    public void ConnectRoom()
    {
        if (PlayerPrefs.GetString("username") == "")
        {
            info.text = "������� ������� � ���� �������!";
            return;
        }
        if (nameRoom.text == "")
        {
            info.text = "* ������� �������� �������";
            return;
        }
        info.text = "* ���� ������� ���������� � �� � ��� ������������, ����� � �� � ���������!";

        PhotonNetwork.JoinOrCreateRoom(nameRoom.text, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, new TypedLobby(nameRoom.text, LobbyType.Default));
    }

    public void ConnectRandomRoom()
    {
        if (PlayerPrefs.GetString("username") == "")
        {
            Debug.Log("������� ������� � ���� �������!");
            return;
        }

        PhotonNetwork.JoinRandomOrCreateRoom(null, 2);
    }

    public async void Login()
    {
        if (login.text == "" || password.text == "")
        {
            info.text = "������� ������� � ���� �������!";
            return;
        }
        validationInfo.text = "����������, ���������...";
        loginButton.interactable = false;
        if (await Task.Run(() => Authentication()))
        {
            PhotonNetwork.NickName = login.text;
            PhotonNetwork.ConnectUsingSettings();
            PlayerPrefs.SetString("username", login.text);
            login.gameObject.SetActive(false);
            password.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            username.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(true);
            validationInfo.text = "";
            username.text = login.text;
            loginButton.interactable = true;
        }
        else
        {
            PlayerPrefs.SetString("username", "");
            validationInfo.text = message;
            loginButton.interactable = true;
        }
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();
        PlayerPrefs.SetString("username", "");
        username.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        login.gameObject.SetActive(true);
        password.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected from Master");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined the Room");
        PhotonNetwork.LoadLevel("Village");
    }

    private bool Authentication()
    {
        if (!Validation()) { return false; }
        return true; // ������!!!
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44389/Authentication");
            request.Method = "POST";
            string query = "username=" + login.text + "&password=" + password.text;
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(query);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            string resultQuery = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    resultQuery += reader.ReadToEnd();
                }
            }
            Debug.Log("�����: " + login.text + ". ������: " + password.text + ". ����� �� ������� ��� ����� � ������� ������: " + resultQuery);
            if (!bool.Parse(resultQuery))
            {
                message = "����� �/��� ������ �������!";
            }
            return bool.Parse(resultQuery);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            message = "��������� ������. ����������, ��������� ������� �����!";
            return false;
        }
    }

    private bool Validation()
    {
        if (Regex.IsMatch(login.text, "^[a-zA-Z�-��-�,�,�]", RegexOptions.IgnoreCase))
        {
            Regex checkLetter = new Regex(@"[a-zA-Z�-��-�,�,�]+");
            MatchCollection checkLetterMatches = checkLetter.Matches(password.text);
            Regex checkNumber = new Regex(@"\d+");
            MatchCollection checkNumberMatches = checkNumber.Matches(password.text);
            if (checkLetterMatches.Count > 0 && checkNumberMatches.Count > 0)
            {
                if (login.text.Length > 3)
                {
                    if (password.text.Length > 3)
                    {
                        return true;
                    }
                    else
                    {
                        message = "����� ������ ������ ���� ����� 3 ��������!";
                        return false;
                    }
                }
                else
                {
                    message = "����� ������ ������ ���� ����� 3 ��������!";
                    return false;
                }
            }
            else
            {
                message = "������ ������ ��������� ��� ������� ���� ����� � �����!";
                return false;
            }
        }
        else
        {
            message = "����� ������ ���������� � �����!";
            return false;
        }
    }
}