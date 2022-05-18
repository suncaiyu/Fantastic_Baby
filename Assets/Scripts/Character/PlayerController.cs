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
            // ȷ������б���ƶ�
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
    //    // ����о�������ƶ��ľ������һ����ֵ���Ż��ƶ�����������ۻ�����ƶ�����
    //    while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    transform.position = targetPos;
    //    isMoving = false;
    //    CheckForEncounters();
    //}

    //// �Ƿ���ͨ��
    //private bool IsWalkable(Vector3 targetObject)
    //{
    //    // ���Ŀ��λ�õ�0.3f��Χ�Ƿ���ڲ��ɹ�ȥ��sloid��
    //    if (Physics2D.OverlapCircle(targetObject, 0.1f, solidObjectLayer | interactableLayer) != null)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    private void CheckForEncounters()
    {
        // �Ƿ��ڲݴ�������
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
