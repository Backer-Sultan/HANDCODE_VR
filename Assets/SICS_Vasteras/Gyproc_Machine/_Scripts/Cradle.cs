/*********************************************
 * Author: Backer Sultan                     *
 * Email:  backer.sultan@ri.se               *
 * Created: 25-04-2017                       *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cradle : MonoBehaviour
{
    /* fields & properties */

    public float maxLeftPosX = -2.355f, maxRightPosX = 2.355f, middlePosX = 0f;
    public float speed = 0.5f;
    public bool isCradleInMaxRight { get { return (transform.localPosition.x >= maxRightPosX) ? true : false; } }
    [Tooltip("An object with name `PinsherRotator` will be searched and assigned by default, if it doesn't exist you need to assign it manually.")]
    public Transform pinsherRotator;

    [HideInInspector]
    public bool isCradleMoving = false;
    [HideInInspector]
    public bool isBreakApplied = true;
    [HideInInspector]
    public bool isPinsherLow = false;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public Machine machine;

    private Vector3 destination;
    private Animator pinsherAnimator;



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

        machine = transform.GetComponentInParent<Machine>();
        if (!machine)
            Debug.LogError("Cradle.cs: Machine script is not fount in the parent object!");

        audioSource = GetComponentInChildren<AudioSource>();
        if (!audioSource)
            Debug.LogError("Cradle.cs: AudioSource component is missing!");

    }

    private IEnumerator MoveToRightRoutine()
    {
        PlaySound(machine.sounds.Cradle_Moving);
        isCradleMoving = true;
        destination = (transform.localPosition.x < middlePosX) ?
            new Vector3(middlePosX, 0f, 0f) : new Vector3(maxRightPosX, 0f, 0f);
        while(Vector3.Distance(transform.localPosition, destination) > Vector3.kEpsilon)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
        audioSource.clip = machine.sounds.Cradle_Stopping;
        audioSource.Play();
        isCradleMoving = false;
    }

    public void MoveToRight()
    {
        IEnumerator routine = MoveToRightRoutine();
        StopAllCoroutines();
        StartCoroutine(routine);
    }

    private IEnumerator MoveToLeftRoutine()
    {
        PlaySound(machine.sounds.Cradle_Moving);
        isCradleMoving = true;
        destination = (transform.localPosition.x > middlePosX) ? 
            new Vector3(middlePosX, 0f, 0f) : new Vector3(maxLeftPosX, 0f, 0f);
        while (Vector3.Distance(transform.localPosition, destination) > Vector3.kEpsilon)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
        audioSource.clip = machine.sounds.Cradle_Stopping;
        audioSource.Play();
        isCradleMoving = false;
    }

    public void MoveToLeft()
    {
        IEnumerator routine = MoveToLeftRoutine();
        StopAllCoroutines();
        StartCoroutine(routine);
    }

    public void Stop()
    {
        PlaySound(machine.sounds.Cradle_Stopping);
        isCradleMoving = false;
        StopAllCoroutines();
    }

    public void LowerPinsher()
    {
        isPinsherLow = true;
        pinsherAnimator.SetBool("LowerPinsher", isPinsherLow);
    }

    public void RaisePinsher()
    {
        isPinsherLow = false;
        pinsherAnimator.SetBool("LowerPinsher", isPinsherLow);
    }

    public void ToggleBreak()
    {
        PlaySound(machine.sounds.Cradle_Stopping);
        isBreakApplied = !isBreakApplied;

        /******* depended on script PaperConsole.cs *****/
        //machine.paperConsole.breakButton.GetComponent<Animator>().SetBool("isBreakApplied", isBreakApplied);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
