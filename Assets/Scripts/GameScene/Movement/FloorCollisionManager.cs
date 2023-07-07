using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollisionManager : MonoBehaviour
{
    public static string FloorType;
    public static bool IsOnCollision = false; // RayCast�� �ƴ� Collision ó�� (�� �΋H���� ��츦 ����)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FloorType = gameObject.tag;
            //Debug.Log(gameObject.tag);
            IsOnCollision = true;
        }
    }

    /*private void OnCollisionExit2D(Collision2D collision)
    {
        FloorType = "";
        IsOnCollision = false;
    }*/
}
