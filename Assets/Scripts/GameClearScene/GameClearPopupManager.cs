using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameClearPopupManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerStat;

    [SerializeField]
    private TextMeshProUGUI gotoScoreboardButton;

    private void Start()
    {
        if (GameManager.IsMultiPlayer)
        {
            gotoScoreboardButton.fontSize = 16;
            gotoScoreboardButton.text = "�������� ���ư���";
        }
        else
        {
            gotoScoreboardButton.fontSize = 20;
            gotoScoreboardButton.text = "��������";
        }
        Debug.Log("GameClearPopupManagerStart: " + AWSManager.UserData.DBKey);
    }

    private void Awake()
    {
        Debug.Log("GameClearPopupManagerAwake: " + AWSManager.UserData.DBKey);
    }

    private void Update()
    {
        if (GameManager.IsMultiPlayer)
        {
            // �г���
            playerStat.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AWSManager.UserData.DBKey + "���� ���";
            // �ɸ� �ð�
            playerStat.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "�ɸ� �ð�: " + TimerControllerPUN.HighestStageTimerText;
            // �� ���� ��
            playerStat.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "�� ���� ��: " + PlayerControllerPUN.JumpCount.ToString();
            // �� ������ ��
            playerStat.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "�� ������ ��: " + PlayerControllerPUN.FallingCount.ToString();
            // ����� Ƚ��
            playerStat.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "������� �� Ƚ��: " + Stage6_DrainpipePUN.DrainpipeCount.ToString();
        }
        else
        {
            // �г���
            playerStat.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AWSManager.UserData.DBKey + "���� ���";
            // �ɸ� �ð�
            playerStat.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "�ɸ� �ð�: " + TimerController.HighestStageTimerText;
            // �� ���� ��
            playerStat.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "�� ���� ��: " + PlayerController.JumpCount.ToString();
            // �� ������ ��
            playerStat.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "�� ������ ��: " + PlayerController.FallingCount.ToString();
            // ����� Ƚ��
            playerStat.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "������� �� Ƚ��: " + Stage6_Drainpipe.DrainpipeCount.ToString();
        }

    }

    public void onClickGotoMainButton()
    {
        AWSManager.UserData = new AWSManager.Userdata
        {
            DBKey = "null",
            AuthKey = "nullAuth",
            HighestScore = 0,
            CustomizationData = -1,
            IsGuest = true
        };
        if (GameManager.IsMultiPlayer)
        {
            GameStageManagerPUN.PlayerHighestStage = 0;
            GameStageManagerPUN.PlayerStageCounter = 0;
            GameStageManagerPUN.IsCleared = false;
            PlayerControllerPUN.Stuck = false;
            PlayerControllerPUN.SlippyConstant = 0.5f;
            PlayerControllerPUN.PlayerSpeed = 7.0f;
            PlayerControllerPUN.JumpCount = 0;
            PlayerControllerPUN.FallingCount = 0;
            Stage6_DrainpipePUN.DrainpipeCount = 0;
            TimerControllerPUN.time = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            StageCongratulationManagerPUN.IsClearCongratulated = false;
            Debug.Log("Disconnect to Pun Server...");
            PhotonNetwork.Disconnect();
        }
        else
        {
            GameStageManager.PlayerHighestStage = 0;
            GameStageManager.PlayerStageCounter = 0;
            GameStageManager.IsCleared = false;
            PlayerController.Stuck = false;
            PlayerController.SlippyConstant = 0.5f;
            PlayerController.PlayerSpeed = 7.0f;
            PlayerController.JumpCount = 0;
            PlayerController.FallingCount = 0;
            Stage6_Drainpipe.DrainpipeCount = 0;
            TimerController.time = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
        SceneManager.LoadScene("TitleScene");
    }
    public void onClickGotoScoreboardButton()
    {
        if (GameManager.IsMultiPlayer)
        {
            //PhotonNetwork.JoinRoom("Room#1");
            PhotonNetwork.JoinOrCreateRoom("Room#1", new RoomOptions { MaxPlayers = 20 }, null);
        }
        else
        {
            AWSManager.UserData = new AWSManager.Userdata
            {
                DBKey = "null",
                AuthKey = "nullAuth",
                HighestScore = 0,
                CustomizationData = -1,
                IsGuest = true
            };
            GameStageManager.PlayerHighestStage = 0;
            GameStageManager.PlayerStageCounter = 0;
            GameStageManager.IsCleared = false;
            PlayerController.Stuck = false;
            PlayerController.SlippyConstant = 0.5f;
            PlayerController.PlayerSpeed = 7.0f;
            TimerController.time = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            SceneManager.LoadScene("ScoreboardScene");
        }
    }

    public override void OnJoinedRoom()
    {
        GameStageManagerPUN.IsCleared = true;
        SceneManager.LoadScene("GameScene_Multiplayer");
    }
}
