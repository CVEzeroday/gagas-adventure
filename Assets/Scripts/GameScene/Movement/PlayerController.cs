using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviour
{
    public static float PlayerSpeed = 7.0f;
    public static float JumpSpeed = 14f; //�� 4.87 ����
    public static Vector2 PlayerPosition;
    public static float SlippyConstant = 0.5f;
    public static bool IsJumping = false;
    public static bool Stuck = false;

    public static int JumpCount = 0;
    public static int FallingCount = 0;
    private bool isFalling = false;

    public static bool ForceRight = false;
    public static bool ForceLeft = false;
    public static bool ForceJump = false;

    private float deltaVelocity = 0;
    private float inputHor = 0;
    private float inputVert = 0;

    private bool jumpKeyOn = false;
    private bool leftKeyOn = false;
    private bool rightKeyOn = false;

    private Rigidbody2D rigid2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField]
    private GameObject[] hats;
    private SpriteRenderer[] hatsSpriteRenderers;

    private Vector2[] initialHatsPosition;
    private Vector3[] initialHatsRotation;

    void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        rigid2D.freezeRotation = true;
        rigid2D.gravityScale = 2;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        hatsSpriteRenderers = new SpriteRenderer[6];
        initialHatsPosition = new Vector2[6];
        initialHatsRotation = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            hatsSpriteRenderers[i] = hats[i].GetComponent<SpriteRenderer>();
            initialHatsPosition[i] = hats[i].transform.localPosition;
            initialHatsRotation[i] = hats[i].transform.localRotation.eulerAngles;
            Debug.Log(initialHatsPosition[i].x);
            Debug.Log(initialHatsRotation[i].z);
        }
    }

    void Update()
    {
        //Debug.Log("JumpCount: " + JumpCount);
        //Debug.Log("FallingCount: " + FallingCount);
        if (Input.GetKey(KeyCode.UpArrow) && !Stuck)
        {
            jumpKeyOn = true;
        }
        else
        {
            jumpKeyOn = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && !Stuck)
        {
            leftKeyOn = true;
        }
        else
        {
            leftKeyOn = false;
        }
        if (Input.GetKey(KeyCode.RightArrow) && !Stuck)
        {
            rightKeyOn = true;
        }
        else
        {
            rightKeyOn = false;
        }

        if (IsJumping == false)
        {
            //�ӵ� ����
            if (!(leftKeyOn || rightKeyOn || ForceLeft || ForceRight))
            {
                rigid2D.velocity = new Vector2(rigid2D.velocity.x * SlippyConstant, rigid2D.velocity.y);
            }
            if (Mathf.Abs(rigid2D.velocity.x) < 0.01f)
            {
                rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
            }
            //Debug.Log(rigid2D.velocity.x);
        }
        PlayerPosition = transform.position;

        //�ִϸ��̼�
        if ((rigid2D.velocity.x != 0) && (IsJumping == false))
        {
            animator.SetBool("IsWalking", true);
        }
        else animator.SetBool("IsWalking", false);

        //Ŀ���͸���¡
        for (int i = 0; i < 6; i++)
        {
            if (AWSManager.UserData.CustomizationData == i)
            {
                hats[i].SetActive(true);
            }
            else
            {
                hats[i].SetActive(false);
            }
        }
    }

    private void flip(bool direction)
    {
        spriteRenderer.flipX = direction;
        for (int i = 0; i < 6; i++)
        {
            hatsSpriteRenderers[i].flipX = direction;
            if (i == 2)
            {
                hatsSpriteRenderers[i].flipX = !direction;
            }

            if (!direction)
            {
                hats[i].transform.localPosition = new Vector2(initialHatsPosition[i].x, initialHatsPosition[i].y);
                hats[i].transform.localRotation = Quaternion.Euler(initialHatsRotation[i].x, initialHatsRotation[i].y, initialHatsRotation[i].z);
            }
            else
            {
                hats[i].transform.localPosition = new Vector2(-1 * initialHatsPosition[i].x, initialHatsPosition[i].y);
                hats[i].transform.localRotation = Quaternion.Euler(initialHatsRotation[i].x, initialHatsRotation[i].y, -1 * initialHatsRotation[i].z);
            }
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("PlayerVelocityY: " + rigid2D.velocity.y);

        inputHor = 0;
        inputVert = 0;

        deltaVelocity -= rigid2D.velocity.y;

        if(deltaVelocity == 0)
        {
            IsJumping = false;
        }
        else
        {
            IsJumping = true;
        }

        if (rigid2D.velocity.y < -15f && !isFalling)
        {
            FallingCount++;
            isFalling = true;
        }
        if (rigid2D.velocity.y > -15f)
        {
            isFalling = false;
        }

        if (IsJumping == false)
        {
            if (leftKeyOn || ForceLeft)
            {
                inputHor = -1f;
                flip(true);
            }
            if (rightKeyOn || ForceRight)
            {
                inputHor = 1f;
                flip(false);
            }
            if (jumpKeyOn || ForceJump)
            {
                JumpCount++;
                IsJumping = true;
                inputVert = 1f;
            }
        }

        if (Stuck)
        {
            if (ForceLeft)
            {
                inputHor = -1f;
                flip(true);
            }
            if (ForceRight)
            {
                inputHor = 1f;
                flip(false);
            }
            if (ForceJump)
            {
                JumpCount++;
                IsJumping = true;
                inputVert = 1f;
            }
        }

        //Debug.Log(inputHor);

        rigid2D.AddForce(Vector2.right * inputHor + Vector2.up * inputVert * JumpSpeed, ForceMode2D.Impulse);

        //Max speed Right
        if (rigid2D.velocity.x > PlayerSpeed)
        {//���������� �̵� (+) , �ִ� �ӷ��� ������ 
            rigid2D.velocity = new Vector2(PlayerSpeed, rigid2D.velocity.y); //�ش� ������Ʈ�� �ӷ��� PlayerSpeed 
        }

        //Max speed left
        else if (rigid2D.velocity.x < PlayerSpeed * (-1))
        {// �������� �̵� (-) 
            rigid2D.velocity = new Vector2(PlayerSpeed * (-1), rigid2D.velocity.y); //y���� ������ �����̹Ƿ� 0���� ������ �θ� �ȵ� 
        }
        //Debug.Log("PlayerController: velocity: " + rigid2D.velocity);
        //Debug.Log("PlayerController: SlippyConstant: " + SlippyConstant);
        //Debug.Log("Player Y: " + rigid2D.position.y);
        deltaVelocity = rigid2D.velocity.y;
    }
}