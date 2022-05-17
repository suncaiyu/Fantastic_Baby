using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ѫ����
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    // ����Ѫ��
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    // ƽ������Ѫ��
    public IEnumerator SetHPSmooth(float hp)
    {
        float currentHP = health.transform.localScale.x;
        float changeAmt = currentHP - hp;
        while(currentHP - hp > Mathf.Epsilon)
        {
            currentHP -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(hp, 1f);
    }
}
