/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.UI;
using UnityEngine.Events;

public class ProgressRadial : MonoBehaviour
{
    /* fields & properties */

    public Image bar;
    public Text percentage;
    public Animator GreenExpaner;
    public Animator CheckMark;  
    [Range(0.1f, 1f)]
    public float speed = 0.3f;
    public UnityEvent OnFilled;

    private IEnumerator activeRoutine;
    private float stack = 0f;
    



    /* methods & coroutines */

    private void Start()
    {
        // initialization
        if (!bar)
            bar = transform.FindChild("ProgressRadial/Green").GetComponent<Image>();
        if (!bar)
            Debug.LogError("ProgressRadial.cs: component Image on Object 'Green' is missing!");
        else
            bar.fillAmount = 0f;

        if (!percentage)
            percentage = transform.FindChild("White_Solid/ProgressRadial/Text").GetComponent<Text>();
        if (!percentage)
            Debug.LogError("ProgressRadial.cs: component Text on Object 'Text' is missing!");
        else
            percentage.text = "0%";
    }

    public void Add(float num)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        stack += num;
        activeRoutine = AddOrSubtractRoutine(stack);
        StartCoroutine(activeRoutine);
    }

    public void Subtract(float num)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        stack -= num;
        activeRoutine = AddOrSubtractRoutine(stack);
        StartCoroutine(activeRoutine);
    }

    private IEnumerator AddOrSubtractRoutine(float num)
    {
        float total;
        float init;
        float delta;

        total = bar.fillAmount + num;
        if (total > 1f)
            total = 1f;
        else if (total < 0f)
            total = 0f;
        while (bar.fillAmount != total)
        {
            init = bar.fillAmount;
            bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, total, speed * Time.deltaTime);
            delta = bar.fillAmount - init;
            stack -= delta;
            percentage.text = string.Format("{0}%", Mathf.Round(bar.fillAmount * 100f).ToString());
            yield return null;
        }
        activeRoutine = null;
        stack = 0f;
        if (bar.fillAmount == 1f)
        {
            GreenExpaner.gameObject.SetActive(true);
            CheckMark.gameObject.SetActive(true);
            OnFilled.Invoke();
        }
        else
        {
            GreenExpaner.gameObject.SetActive(false);
            CheckMark.gameObject.SetActive(false);
        }
    }
}
