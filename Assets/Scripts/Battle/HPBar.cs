using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 血槽类
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    // 设置血量
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    // 平滑设置血量
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
