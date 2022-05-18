using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action onEncountered;

    private Vector2 input;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
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
        if (!character.IsMoving)
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
                StartCoroutine(character.Move(input, CheckForEncounters));
            }
        }
        //animator.SetBool("isMoving", isMoving);
        character.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    //IEnumerator Move(Vector3 targetPos)
    //{
    //    isMoving = true;
    //    // 这里，感觉是如果移动的距离大于一个定值，才会移动，否则继续累积这个移动距离
    //    while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    transform.position = targetPos;
    //    isMoving = false;
    //    CheckForEncounters();
    //}

    //// 是否能通过
    //private bool IsWalkable(Vector3 targetObject)
    //{
    //    // 检测目标位置的0.3f范围是否存在不可过去的sloid层
    //    if (Physics2D.OverlapCircle(targetObject, 0.1f, solidObjectLayer | interactableLayer) != null)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    private void CheckForEncounters()
    {
        // 是否在草丛遇怪区
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.LongGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                onEncountered();
                //animator.SetBool("isMoving", false);
                character.Animator.IsMoving = false;
            }
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos =  transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider =  Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }
}
