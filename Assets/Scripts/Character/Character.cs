using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    CharacterAnimator animator;
    public bool IsMoving { get; private set; }
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);
        //animator.SetFloat("moveX", input.x);
        //animator.SetFloat("moveY", input.y);
        Vector3 targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!IsPathClear(targetPos))
        {
            yield break;
        }

        IsMoving = true;
        // ����о�������ƶ��ľ������һ����ֵ���Ż��ƶ�����������ۻ�����ƶ�����
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;
        OnMoveOver?.Invoke();
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.1f, 0.1f), 0f, dir, diff.magnitude - 1,
            GameLayers.i.SolidObjectLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer))
        {
            return false;
        }
        return true;
    }

    private bool IsWalkable(Vector3 targetObject)
    {
        // ���Ŀ��λ�õ�0.3f��Χ�Ƿ���ڲ��ɹ�ȥ��sloid��
        if (Physics2D.OverlapCircle(targetObject, 0.1f, GameLayers.i.SolidObjectLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        } else
        {
            Debug.Log("Error in Look Toeards: �㲻�ܿ���Խ���");
        }
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }
    public CharacterAnimator Animator
    {
        get => animator;
    }
}
