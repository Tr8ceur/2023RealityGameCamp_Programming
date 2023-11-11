using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;

    public Vector2 inputDirection;

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;

    [Header("��������")]
    public float speed;
    public float jumpForce;
    public float hurtForce;

    [Header("�������")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("״̬")]
    public bool isHurt;

    public bool isDead;

    public bool isAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        inputControl = new PlayerInputControl();

        inputControl.Gameplay.Jump.started += Jump;     //��Ծ

        inputControl.Gameplay.Attack.started += PlayerAttack;     //����
    }

    

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
        Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(Time.deltaTime * speed *inputDirection.x,rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;
        //���﷭ת
        transform.localScale = new Vector3(faceDir, 1, 1);
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("Jump");
        if(physicsCheck.isGround)
        rb.AddForce(transform.up * jumpForce,ForceMode2D.Impulse);
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayerAttack();
        isAttack = true;

    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;      //�ٶȹ���
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;  //�ж����˷���

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDaed()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}