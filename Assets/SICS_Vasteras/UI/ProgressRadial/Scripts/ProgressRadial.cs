/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace HandCode
{
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
            if (bar == null)
                bar = transform.Find("Green").GetComponent<Image>();
            if (bar == null)
                Debug.LogError(string.Format("{0}\nProgressRadial.cs: Component `Image` on Object `Green` is missing!", Machine.GetPath(gameObject)));
            else
                bar.fillAmount = 0f;

            if (percentage == null)
                percentage = transform.Find("White_Solid/Text").GetComponent<Text>();
            if (percentage == null)
                Debug.LogError(string.Format("{0}/nProgressRadial.cs: Component `Text` on Object `Text` is missing!", Machine.GetPath(gameObject)));
            else
                percentage.text = "0%";


            // Inactive GameObject can't be found using transform.Find(). You have to assign it manually in the inspetor.
            if (greenExpander == null)
                Debug.LogError(string.Format("{0}\nProgressRadial.cs: Component `Animator` on Object `GreenExpander` is missing!", Machine.GetPath(gameObject)));

            // Inactive GameObject can't be found using transform.Find(). You have to assign it manually in inspetor.
            if (checkmark == null)
                Debug.LogError(string.Format("{0}\nProgressRadial.cs: Component `Animator` on Object `CheckMark` is missing!", Machine.GetPath(gameObject)));

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                Debug.LogError(string.Format("{0}\nProgressRadial.cs: Component `AudioSource` is missing!", Machine.GetPath(gameObject)));
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

        public void Assign(float num)
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                stack = 0f;
            }
            float difference = Mathf.Abs(num - bar.fillAmount);
            if (num > bar.fillAmount)
                Add(difference);
            else
                Subtract(difference);
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
                percentage.text = string.Format("{0}%", Mathf.FloorToInt(bar.fillAmount * 100f).ToString());
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

        public void GameFlowSystemListener()
        {
            GameFlowManager gfm = GameObject.FindObjectOfType<GameFlowManager>();
            if(gfm == null)
            {
                Debug.LogError(string.Format("{0}\nProgressRadial.cs: Couldn't find any `GameFlowManager.cs`", Machine.GetPath(gameObject)));
                return;
            }
            Assign(gfm.completionPercentage);
        }
    } 
}
