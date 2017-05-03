/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * Created: 25-04-2017                       *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cradle : MonoBehaviour
{
    /* fields & properties */

    [Range(0f, 1f)]
    public float speed = 0.5f;
    [Tooltip("An object with name `PinsherRotator` will be searched and assigned by default, if it doesn't exist you need to assign it manually.")]
    public Transform pinsherRotator;
    [HideInInspector]
    public bool isBreakApplied = true;
    [HideInInspector]
    public bool isPinsherLow = false;
    [HideInInspector]
    public enum CradlePosition { MIDDLE, LEFT, RIGHT };
    public CradlePosition cradlePos = CradlePosition.MIDDLE;
    [Header("Events")]
    public UnityEvent onTargetReached;
    public UnityEvent onTargetLeft;
    public UnityEvent onPinsherLowered;
    public UnityEvent onPinsherRaised;
    public UnityEvent onBreakToggled;

    private AudioSource audioSource;
    private Animator pinsherAnimator;
    private bool isMoving = false;
    private Vector3 direction = Vector3.zero;



    /* methods & coroutines */

    private void Start()
    {
        // initialization
        if (!pinsherRotator)
            pinsherRotator = transform.Find("PinsherRotator");
        if (!pinsherRotator)
            Debug.LogError("Cradle.cs: Object PinsherRotator is missing!");

        pinsherAnimator = pinsherRotator.GetComponent<Animator>();
        if (!pinsherAnimator)
            Debug.LogError("Crale.cs: Animator Component is missing on object PinsherRotator!");

        audioSource = GetComponentInChildren<AudioSource>();
        if (!audioSource)
            Debug.LogError("Cradle.cs: AudioSource component is missing!");

    }

    public void MoveToLeft()
    {
        if(cradlePos != CradlePosition.LEFT)
        {
            PlaySound(MachineSounds.Instance.Cradle_Moving);
            direction = Vector3.left;
            isMoving = true;
        }
    }

    public void MoveToRight()
    {
       if(cradlePos!= CradlePosition.RIGHT)
        {
            PlaySound(MachineSounds.Instance.Cradle_Moving);
            direction = Vector3.right;
            isMoving = true;
        }
    }

    public void Stop()
    {
        PlaySound(MachineSounds.Instance.Cradle_Stopping);
        isMoving = false;
    }

    public void LowerPinsher()
    {
        isPinsherLow = true;
        pinsherAnimator.SetBool("LowerPinsher", isPinsherLow);
        onPinsherLowered.Invoke();
    }

    public void RaisePinsher()
    {
        isPinsherLow = false;
        pinsherAnimator.SetBool("LowerPinsher", isPinsherLow);
        onPinsherRaised.Invoke();
    }

    public void ToggleBreak()
    {
        PlaySound(MachineSounds.Instance.Cradle_Stopping);
        isBreakApplied = !isBreakApplied;
        onBreakToggled.Invoke();

        /******* depended on script PaperConsole.cs *****/
        //machine.paperConsole.breakButton.GetComponent<Animator>().SetBool("isBreakApplied", isBreakApplied);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.Self);
            // stoping when reaching the middle position (aproximately (0,0,0)).
            if (Vector3.Distance(transform.localPosition, Vector3.zero) <= Vector3.kEpsilon)
                Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CradleLimitRight")
        {
            Stop();
            cradlePos = CradlePosition.RIGHT;
            onTargetReached.Invoke();
            return;
        }
        if (other.tag == "CradleLimitLeft")
        {
            Stop();
            cradlePos = CradlePosition.LEFT;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cradlePos == CradlePosition.RIGHT)
            onTargetLeft.Invoke();
        cradlePos = CradlePosition.MIDDLE;
    }
}