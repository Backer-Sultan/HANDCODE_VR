using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressRadial : MonoBehaviour
{
    public Image image;
    public Text text;
    public float speed;

    private void Start()
    {
        image.fillAmount = 0f;
        text.text = "0%";
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Add(0.1f);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Subtract(0.1f);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(AddProgressive(0.1f));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(SubtructProgressive(0.1f));
    }


    public void Add(float num)
    {
        float total = image.fillAmount + num;
        if (total > 1f)
            total = 1f;
        image.fillAmount = total;
        text.text = Mathf.Round(image.fillAmount * 100f).ToString() + "%";
    }

    public IEnumerator AddProgressive(float num)
    {
        float total = image.fillAmount + num;
        if (total > 1f)
            total = 1f;
        while (image.fillAmount < total)
        {
            image.fillAmount = Mathf.MoveTowards(image.fillAmount, total, speed * Time.deltaTime);
            text.text = Mathf.Round(image.fillAmount * 100f).ToString() + "%";
            yield return null;
        }
    }

    public void Subtract(float num)
    {
        float total = image.fillAmount - num;
        if (total < 0f)
            total = 0f;
        image.fillAmount = total;
        text.text = Mathf.Round(image.fillAmount * 100f).ToString() + "%";
    }

    public IEnumerator SubtructProgressive(float num)
    {
        float total = image.fillAmount - num;
        if (total < 0f)
            total = 0f;
        while (image.fillAmount > total)
        {
            image.fillAmount = Mathf.MoveTowards(image.fillAmount, total, speed * Time.deltaTime);
            text.text = Mathf.Round(image.fillAmount * 100f).ToString() + "%";
            yield return null;
        }
    }
}
