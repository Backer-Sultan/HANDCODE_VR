/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ProgressRadial : MonoBehaviour
{
    /* fields & properties */

    public Image bar;
    public Text percentage;
    public Animator greenExpander;
    public Animator checkmark;
    [Range(0.1f, 1f)]
    public float speed = 0.3f;
    public UnityEvent OnFilled;

    private IEnumerator activeRoutine;
    private float stack = 0f;
    private AudioSource audioSource;



    /* methods & coroutines */

    private void Start()
    {
        // initialization
        if (!bar)
            bar = transform.Find("Green").GetComponent<Image>();
        if (!bar)
            Debug.LogError("ProgressRadial.cs: component Image on Object 'Green' is missing!");
        else
            bar.fillAmount = 0f;

        if (!percentage)
            percentage = transform.Find("White_Solid/Text").GetComponent<Text>();
        if (!percentage)
            Debug.LogError("ProgressRadial.cs: component Text on Object 'Text' is missing!");
        else
            percentage.text = "0%";


        // Inactive GameObject can't be found using transform.Find(). You have to assign it manually in inspetor.
        if (!greenExpander)
            Debug.LogError("ProgressRadial.cs: component Animator on Object 'GreenExpander' is missing!");

        // Inactive GameObject can't be found using transform.Find(). You have to assign it manually in inspetor.
        if (!checkmark)
            Debug.LogError("ProgressRadial.cs: component Animator on Object 'CheckMark' is missing!");

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            Debug.LogError("ProgressRadial.cs: component AudioSource is missing!");
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
        Add(-num);
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
            greenExpander.gameObject.SetActive(true);
            checkmark.gameObject.SetActive(true);
            OnFilled.Invoke();
        }
        else
        {
            greenExpander.gameObject.SetActive(false);
            checkmark.gameObject.SetActive(false);
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
