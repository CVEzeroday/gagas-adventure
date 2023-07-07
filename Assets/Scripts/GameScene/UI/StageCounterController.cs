using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageCounterController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] stageCounterText;

    private string[] subNames = { "���Ͻ�", "1��", "2��", "3��", "4��", "5��", "����", "�� ���� ������" };

    private void Start()
    {
        stageCounterText[0].text = "1��������";
        stageCounterText[1].text = "���Ͻ�";
    }

    private void Update()
    {
        if (!GameStageManager.IsCleared)
        {
            if (GameStageManager.PlayerStageCounter != 7)
            {
                stageCounterText[0].fontSize = 14;
                stageCounterText[0].text = ((GameStageManager.PlayerStageCounter + 1).ToString() + "��������");
                stageCounterText[1].fontSize = 10;
                stageCounterText[1].text = ("- " + subNames[GameStageManager.PlayerStageCounter] + " -");
            }
            else
            {
                stageCounterText[0].fontSize = 10;
                stageCounterText[0].text = ("������ ��������");
                stageCounterText[1].fontSize = 8;
                stageCounterText[1].text = ("- �� ���� ������ -");
            }
        }
        else
        {
            stageCounterText[0].fontSize = 12;
            stageCounterText[0].text = ("���� Ŭ����!");
            stageCounterText[1].fontSize = 10;
            stageCounterText[1].text = ("- �����մϴ�! -");
        }
    }
}
