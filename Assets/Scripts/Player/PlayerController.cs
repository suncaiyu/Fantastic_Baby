using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    // ���ⲿ��ȡ��ײ����
    public LayerMask solidObjectLayer;
    // ���ֳ��ݲ�
    public LayerMask longGrassLayer;

    public event Action onEncountered;
    private bool isMoving;
    private Vector2 input;

    // ����״̬
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //battleCamera.enabled = false;
        //mainCamera.enabled = true;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            // ȷ������б���ƶ�
            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                Vector3 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                // ����Э��,��ʱ����
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        // ����о�������ƶ��ľ������һ����ֵ���Ż��ƶ�����������ۻ�����ƶ�����
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }

    // �Ƿ���ͨ��
    private bool IsWalkable(Vector3 targetObject)
    {
        // ���Ŀ��λ�õ�0.3f��Χ�Ƿ���ڲ��ɹ�ȥ��sloid��
        if (Physics2D.OverlapCircle(targetObject, 0.1f, solidObjectLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        // �Ƿ��ڲݴ�������
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                onEncountered();
                animator.SetBool("isMoving", false);
            }
        }
    }
}
