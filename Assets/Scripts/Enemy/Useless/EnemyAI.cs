using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private Vector3 initialPosition;

    /// <summary>
    /// ��Ϊ�ز������ܵĶ�����ֻ���ߵģ��õĻ��ǹ������Ҳ��������������µ�walk��run��Ӧ���������������ٶȣ���Ŀǰֻ��һ��������walk��chase����һ���������ٶ�
    /// </summary>

    [Header("���ַ�Χ")]
    public float wanderRadius;          //���߰뾶���ƶ�״̬�£�����������߰뾶�᷵�س���λ��
    public float chaseRadius;            //׷���뾶������ҳ���׷���뾶������׷��������׷����ʼλ��
    public float defendRadius;          //�����뾶����ҽ��������׷����ң�������<����������ᷢ������

    [Header("���ֲ���")]
    public float attackRange;            //��������
    public float walkSpeed;              //�ƶ��ٶ�
    public float turnSpeed;              //ת���ٶ�
    public float flipThreshold = 0.01f;

    private enum State
    {
        IDLE,      //ԭ�غ���
        WALK,       //�ƶ�
        CHASE,      //׷�����
        RETURN      //����׷����Χ�󷵻�
    }
    private State currentState = State.IDLE;          //Ĭ��״̬Ϊԭ�غ���

    private float diatanceToPlayer;             //��������ҵľ���
    private float diatanceToInitial;            //�������ʼλ�õľ���
    private Quaternion targetRotation;          //�����Ŀ�곯��

    private bool isRun = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        //�����ʼλ����Ϣ
        initialPosition = gameObject.GetComponent<Transform>().position;

    }
    void Update()
    {

        switch (currentState)
        {
            //���ߣ�����״̬���ʱ���ɵ�Ŀ��λ���޸ĳ��򣬲���ǰ�ƶ�
            case State.WALK:                        
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                //��״̬�µļ��ָ��
                WanderRadiusCheck();
                break;

            //׷��״̬�����������ȥ(��ȥ)
            case State.CHASE:                       
                if (!isRun)
                {
                    anim.SetTrigger("Walk");
                    isRun = true;
                }
                transform.Translate(Vector2.up * Time.deltaTime * walkSpeed);

                Vector3 direction = (player.transform.position   - transform.position).normalized;

                // �ȽϷ���������Xֵ����ֵ
                if (direction.x > flipThreshold)
                {
                    // �����Ҳ�
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if (direction.x < -flipThreshold)
                {
                    // �������
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }

                /*//�������λ��
                targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector2.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);*/

                //��״̬�µļ��ָ��
                ChaseRadiusCheck();
                break;

            //����״̬������׷����Χ�󷵻س���λ��
            case State.RETURN:
                //�����ʼλ���ƶ�
                targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector2.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                //��״̬�µļ��ָ��
                ReturnCheck();
                break;



        }
    }

    //ԭ�غ���״̬�ļ��
    void DistanceCheck()
    {
        diatanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (diatanceToPlayer < attackRange)
        {
            anim.SetTrigger("Attack");
        }
        else if (diatanceToPlayer < defendRadius)
        {
            currentState = State.CHASE;
        }

    }


    //����״̬��⣬�����˾��뼰�����Ƿ�Խ��
    void WanderRadiusCheck()
    {
        diatanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        diatanceToInitial = Vector2.Distance(transform.position, initialPosition);

        if (diatanceToPlayer < attackRange)
        {
            anim.SetTrigger("Attack");
        }
        else if (diatanceToPlayer < defendRadius)
        {
            currentState = State.CHASE;
        }

        if (diatanceToInitial > wanderRadius)
        {
            //�������Ϊ��ʼ����
            targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector2.up);
        }
    }

    // ׷��״̬��⣬�������Ƿ���빥����Χ�Լ��Ƿ��뿪���䷶Χ
    void ChaseRadiusCheck()
    {
        diatanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        diatanceToInitial = Vector2.Distance(transform.position, initialPosition);

        if (diatanceToPlayer < attackRange)
        {
            anim.SetTrigger("Attack");
        }
        //�������׷����Χ���ߵ��˵ľ��볬���������ͷ���
        if (diatanceToInitial > chaseRadius )
        {
            currentState = State.RETURN;
        }
    }

    //����׷���뾶������״̬�ļ�⣬���ټ����˾���
    void ReturnCheck()
    {
        diatanceToInitial = Vector2.Distance(transform.position, initialPosition);
        //����Ѿ��ӽ���ʼλ�ã������һ������״̬
        if (diatanceToInitial < 0.5f)
        {
            isRun = false;
        }
    }


}
