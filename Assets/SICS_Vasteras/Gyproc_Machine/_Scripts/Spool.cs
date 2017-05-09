/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spool : MonoBehaviour
{
    /* fields & properties */

    public string ID; // use 'left' for the left Spool and 'right' for the right one. 
    [Range(0f, 10f)]
    public float speed = 1f;
    [Header("Events")]
    public UnityEvent onTargetReached;
    public UnityEvent onDamaged;
    public UnityEvent onHandled;
    public bool isDamaged { get { return _isDamaged; } }
    public bool isHandled { get { return _isHandled; } }
    [HideInInspector]
    public bool isLeftSideHandled = false;
    [HideInInspector]
    public bool isRightSideHandled = false;

    private bool _isDamaged = false;
    private bool _isHandled = false;
    private bool isMoving = false;
    private bool isTargetReached = false;

   
    
    /* methods & coroutines */

    private void Start()
    {
        if (ID == string.Empty)
            Debug.LogError("Spool.cs: ID can't be empty!");
        else
        {
            ID = ID.Trim().ToLower();
            if (ID != "left" && ID != "right")
                Debug.LogError("Spool.cs: Invalid ID! ID should be `left` or `right`!");
        }
    }

    public void ApplyDamage()
    {
        print("Spool is damaged!");
        _isDamaged = true;
        onDamaged.Invoke();
    }

    public void Handle()
    {
        _isHandled = true;
        onHandled.Invoke();
    }

    public void MoveToTarget()
    {
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void Update()
    {
        if(isMoving && !_isDamaged)
        {
            isTargetReached = Vector3.Distance(transform.localPosition, Vector3.zero) <= Vector3.kEpsilon;
            if (isTargetReached)
            {
                Stop();
                onTargetReached.Invoke();
            }
            else
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed * Time.deltaTime);
        }
    }
}
