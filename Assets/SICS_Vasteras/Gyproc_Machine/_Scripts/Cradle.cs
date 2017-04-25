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
    }

    private IEnumerator MoveToRightRoutine()
    {
        // sound processing
        //audioSource.clip = machine.sounds.Cradle_Moving;
        //audioSource.Play();
        //
        destination = (transform.localPosition.x < middlePosX) ? 
            new Vector3(middlePosX, 0f, 0f) : new Vector3(maxRightPosX, 0f, 0f);
        while(Vector3.Distance(transform.position, destination) > Vector3.kEpsilon)
        {
            transform.position = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void MoveToRight()
    {
        IEnumerator routine = MoveToRightRoutine();
        StopAllCoroutines();
        StartCoroutine(routine);
    }

    private IEnumerator MoveToLeftRoutine()
    {
        // sound processing
        //audioSource.clip = machine.sounds.Cradle_Moving;
        //audioSource.Play();
        //
        destination = (transform.localPosition.x > middlePosX) ? 
            new Vector3(middlePosX, 0f, 0f) : new Vector3(maxLeftPosX, 0f, 0f);
        while (Vector3.Distance(transform.position, destination) > Vector3.kEpsilon)
        {
            transform.position = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void MoveToLeft()
    {
        IEnumerator routine = MoveToLeftRoutine();
        StopAllCoroutines();
        StartCoroutine(routine);
    }

    public void Stop()
    {
        // sound processing
        //audioSource.clip = machine.sounds.Cradle_Stopping;
        //audioSource.Play();
        //
        StopAllCoroutines();
    }

    public void LowerPinsher()
    {
        isPinsherLow = true;
        pinsherAnimator.SetBool("LowerPincher", true);
    }

    public void RaisePinsher()
    {
        isPinsherLow = false;
        pinsherAnimator.SetBool("LowerPincher", false);
    }

    public void ToggleBreak()
    {
        // sound processing
        //audioSource.clip = machine.sounds.Cradle_BreakToggle;
        //audioSource.Play();
        //
        isBreakApplied = !isBreakApplied;

        /******* depended on script PaperConsole.cs *****/
        //machine.paperConsole.breakButton.GetComponent<Animator>().SetBool("isBreakApplied", isBreakApplied);
    }

}
