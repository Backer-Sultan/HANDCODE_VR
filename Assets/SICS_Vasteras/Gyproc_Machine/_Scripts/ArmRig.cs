/*********************************************
 * Project: HANDCODE                         *
 * Author: Backer Sultan                     *
 * Email:  backer.sultan@ri.se               *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class ArmRig : MonoBehaviour
    {
        /* fields & properties */

        public Identifier ID;
        public Transform mainHandle;
        public Arm arm_Left, arm_Right;
        public bool isArmsOpen { get { return arm_Left.armPos == Arm.ArmPosition.LEFT && arm_Right.armPos == Arm.ArmPosition.RIGHT; } }
        public bool isArmsUp { get { return transform.localEulerAngles.z >= 70f; } }
        public bool isArmsDown { get { return transform.localEulerAngles.z < 0f; } }

        private AudioSource audioSource;
        private HingeJoint joint;

        /* methods & coroutines */

        private void Start()
        {
            // initialization
            if (ID == Identifier.NONE)
                Debug.LogError(string.Format("{0}\nArmRig.cs: ID should be assigned!", Machine.GetPath(gameObject)));

            if (mainHandle == null)
                mainHandle = transform.Find("MainHandle");
            if (mainHandle == null)
                Debug.LogError(string.Format("{0}\nArmRig.cs: Object `MainHandle` is missing!", Machine.GetPath(gameObject)));

            if (arm_Left == null)
                arm_Left = transform.Find("MainHandle/Arm_Left").GetComponent<Arm>();
            if (arm_Left == null)
                Debug.LogError(string.Format("{0}\nArmRig.cs: Object `MainHandle/Arm_Left` is missing!", Machine.GetPath(gameObject)));

            if (arm_Right == null)
                arm_Right = transform.Find("MainHandle/Arm_Right").GetComponent<Arm>();
            if (arm_Right == null)
                Debug.LogError(string.Format("{0}\nArmRig.cs: Object `MainHandle/Arm_Right` is missing!", Machine.GetPath(gameObject)));


            audioSource = GetComponentInChildren<AudioSource>();
            if (audioSource == null)
                Debug.LogError(string.Format("{0}\nArmRig.cs: Component `AudioSource` is missing!", Machine.GetPath(gameObject)));

            joint = mainHandle.GetComponent<HingeJoint>();
            if (joint == null)
                Debug.LogError(string.Format("{0}\nArmRig.cs: Component `HingeJoint` is missing on Object `MainHandle`!", Machine.GetPath(gameObject)));
        }

        public void RotateUp()
        {
            JointMotor motor = joint.motor;
            motor.targetVelocity = 10f;
            joint.motor = motor;
            PlaySound(MachineSounds.Instance.ArmRig_Rotating);
        }

        public void RotateDown()
        {
            JointMotor motor = joint.motor;
            motor.targetVelocity = -10f;
            joint.motor = motor;
            PlaySound(MachineSounds.Instance.ArmRig_Rotating);
        }

        public void StopRotating()
        {
            JointMotor motor = joint.motor;
            motor.targetVelocity = 0f;
            joint.motor = motor;
            PlaySound(MachineSounds.Instance.ArmRig_StopRotating);
        }

        public void OpenArms()
        {
            arm_Left.MoveLeft();
            arm_Right.MoveRight();
        }

        public void CloseArms()
        {
            arm_Left.MoveRight();
            arm_Right.MoveLeft();
        }

        public void MoveArmsLeft()
        {
            arm_Left.MoveLeft();
            arm_Right.MoveLeft();
        }

        public void MoveArmsRight()
        {
            arm_Left.MoveRight();
            arm_Right.MoveRight();
        }

        public void StopArms()
        {
            arm_Left.Stop();
            arm_Right.Stop();
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

}


