using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    private bool readyForInterpretData;
    private bool readyToGameClearSceneByGameClearManager;

    CognitoAWSCredentials credentials;
    AmazonDynamoDBClient DBClient;
    DynamoDBContext context;

    private void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        credentials = new CognitoAWSCredentials("ap-northeast-2:b70964ec-0f8d-4a12-9493-04a0c2e77f18", RegionEndpoint.APNortheast2);
        DBClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(DBClient);
    }

    private void Update()
    {
        if (readyToGameClearSceneByGameClearManager == true && StageCongratulationManager.readyToGameClearSceneByStageCongratulationManager == true)
        {
            readyToGameClearSceneByGameClearManager = false;
            StageCongratulationManager.readyToGameClearSceneByStageCongratulationManager = false;
            Debug.Log("GameClearManager: " + AWSManager.UserData.DBKey);
            SceneManager.LoadScene("GameClearScene");
        }

        if (AWSManager.UserData.IsGuest == false)
        {
            if (GameStageManager.EndingFlag)
            {
                GameStageManager.EndingFlag = false;
                AWSManager.UserData.LatestScore = TimerController.time[0];
                ReadDatabase(AWSManager.UserData.DBKey);
            }

            if (readyForInterpretData)
            {
                readyForInterpretData = false;
                if (AWSManager.UserDataTmp.HighestScore != 0)
                {
                    if (AWSManager.UserDataTmp.HighestScore > AWSManager.UserData.LatestScore)
                    {
                        AWSManager.UserData.HighestScore = AWSManager.UserData.LatestScore;
                    }
                    else
                    {
                        AWSManager.UserData.HighestScore = AWSManager.UserDataTmp.HighestScore;
                    }
                }
                else
                {
                    AWSManager.UserData.HighestScore = AWSManager.UserData.LatestScore;
                }
                UpdateDatabase();
            }
        }
    }

    private void UpdateDatabase()
    {
        context.SaveAsync(AWSManager.UserData, (result) =>
        {
            if (result.Exception == null)
            {
                Debug.Log("Data Upload Success");
                readyToGameClearSceneByGameClearManager = true;
            }
            else
                Debug.LogException(result.Exception);
        });
    }

    private void ReadDatabase(string id)
    {
        Debug.Log("ReadDatabase");
        context.LoadAsync<AWSManager.Userdata>(id, (AmazonDynamoDBResult<AWSManager.Userdata> result) =>
        {
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                return;
            }
            AWSManager.UserDataTmp = result.Result;
            
            readyForInterpretData = true;
        }, null);
        return;
    }
}
