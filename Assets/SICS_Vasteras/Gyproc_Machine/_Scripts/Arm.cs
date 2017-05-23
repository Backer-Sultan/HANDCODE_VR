/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    /* fields & properties */

    [Range(0f, 1f)]
    public float speed = 0.1f;

    private bool isMoving = false;
    private Vector3 direction = Vector3.zero;
    private enum ArmPosition { MIDDLE, LEFT, RIGHT };
    private ArmPosition armPos = ArmPosition.MIDDLE;
    private AudioSource audioSource;

    /* methods & coroutines */

    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if (!audioSource)
            Debug.LogError("Arm.cs: AudioSource is missing on one of the arms!");
    }

    public void MoveLeft()
    {
        if(armPos != ArmPosition.LEFT)
        {
            direction = Vector3.back;
            isMoving = true;
            PlaySound(MachineSounds.Instance.Arm_Moving);
        }
        
    }

    public void MoveRight()
    {
        if(armPos != ArmPosition.RIGHT)
        {
            direction = Vector3.forward;
            isMoving = true;
            PlaySound(MachineSounds.Instance.Arm_Moving);
        }
    }

    public void Stop()
    {
        isMoving = false;
        PlaySound(MachineSounds.Instance.Arm_Stopping);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Update()
    {
        if (isMoving)
            transform.Translate(direction * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ArmLimitLeft")
        {
            Stop();
            armPos = ArmPosition.LEFT;
        }
        if (other.tag == "ArmLimitRight")
        {
            Stop();
            armPos = ArmPosition.RIGHT;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        armPos = ArmPosition.MIDDLE;
    }
}
