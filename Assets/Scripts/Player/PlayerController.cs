using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    // 从外部获取碰撞检测层
    public LayerMask solidObjectLayer;
    // 遇怪长草层
    public LayerMask longGrassLayer;

    public event Action onEncountered;
    private bool isMoving;
    private Vector2 input;

    // 动画状态
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
            // 确保不能斜着移动
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
                // 开启协程,暂时不懂
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
        // 这里，感觉是如果移动的距离大于一个定值，才会移动，否则继续累积这个移动距离
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }

    // 是否能通过
    private bool IsWalkable(Vector3 targetObject)
    {
        // 检测目标位置的0.3f范围是否存在不可过去的sloid层
        if (Physics2D.OverlapCircle(targetObject, 0.1f, solidObjectLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        // 是否在草丛遇怪区
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
