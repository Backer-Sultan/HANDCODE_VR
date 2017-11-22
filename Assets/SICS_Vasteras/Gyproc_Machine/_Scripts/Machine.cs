/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using VRTK;

namespace HandCode
{
    public enum Identifier
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN,
    }


    public class Machine : MonoBehaviour
    {
        /* fields & properties */
        
        [HideInInspector]
        public Cradle cradle;
        [HideInInspector]
        public ArmRig armRig_Right, armRig_Left;
        [HideInInspector]
        public Spool spool_Left, spool_Right;
        [HideInInspector]
        public MainConsole mainConsole;
        [HideInInspector]
        public PaperCut paperCut;
        public AdhesiveTape adhesiveTapeHandler;
        public bool isDoubleCommandActive { get { return _isDoubleCommandActive; } }
        public MachineButton lastPushedButton;
        public bool isSpoolBreakApplied;

        private bool _isDoubleCommandActive; // Dubble kommando.



        /* methods & coroutines */

        // returns GameObject's full path in the hierarchy.
        public static string GetPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        private void Start()
        {
            cradle = GetComponentInChildren<Cradle>();
            if (cradle == false)
                Debug.LogError(string.Format("{0}\nMachine.cs: Cradle script is missing!", GetPath(gameObject)));

            ArmRig[] armRigs = GetComponentsInChildren<ArmRig>();
            foreach (ArmRig rig in armRigs)
            {
                if (rig.ID == Identifier.LEFT)
                {
                    armRig_Left = rig;
                    continue;
                }
                if (rig.ID == Identifier.RIGHT)
                {
                    armRig_Right = rig;
                    continue;
                }
            }

            Spool[] spools = GetComponentsInChildren<Spool>();
            foreach (Spool spl in spools)
            {
                if (spl.ID == Identifier.LEFT)
                {
                    spool_Left = spl;
                    continue;
                }
                if (spl.ID == Identifier.RIGHT)
                {
                    spool_Right = spl;
                    continue;
                }
            }
            if (spool_Left == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: No spool with id `Left` is found!", GetPath(gameObject)));
            if (spool_Right == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: No spool with id `Right` is found!", GetPath(gameObject)));

            mainConsole = GetComponentInChildren<MainConsole>();
            if(mainConsole == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: MainConsole script is missing!", GetPath(gameObject)));

            paperCut = FindObjectOfType<PaperCut>();
            if (paperCut == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: No PaperCut script is found!", GetPath(gameObject)));

            adhesiveTapeHandler = FindObjectOfType<AdhesiveTape>();
            if (adhesiveTapeHandler == null)
                Debug.LogError(string.Format("{0}\nMahcine.cs: No AdhesiveTapeHandler script is found!", GetPath(gameObject)));
        }

        public void OnDoubleCommandPushed()
        {
            _isDoubleCommandActive = true;
            if (lastPushedButton != null && lastPushedButton.isPushed == true)
                lastPushedButton.onPushed.Invoke();
        }

        public void OnDoubleCommandReleased()
        {
            _isDoubleCommandActive = false;
            if (lastPushedButton != null && lastPushedButton.isPushed == true && lastPushedButton.requiresDoubleCommand)
            {
                lastPushedButton.onReleased.Invoke();
            }
        }

        public void SetSpoolBreak(bool state)
        {
            isSpoolBreakApplied = state;
        }

        // used to trigger haptics on machine buttons.
        public void TriggerHaptics()
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left), 1f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                print("haptics");
                TriggerHaptics();
            }


            if (Input.GetKeyDown(KeyCode.X))
                ResetSpoolState();
        }

        /* DIRTY PATCH */
        // hardcoded reset for the spool and the arms to avoid restarting the game when damaging the spool.
        public void ResetSpoolState()
        {
            spool_Right.transform.parent = transform;
            spool_Right.transform.localPosition = Vector3.zero;
            spool_Right.transform.localEulerAngles = Vector3.zero;
            armRig_Right.arm_Left.transform.localPosition = new Vector3(0f, 0f, -0.2f);
            armRig_Right.arm_Right.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            armRig_Right.arm_Left.speed = armRig_Right.arm_Right.speed = 0.1f;
            spool_Right._isDamaged = false;
            spool_Right._isHandled = false;
            spool_Right.GetComponentInChildren<Renderer>().material.color = Color.white;
        }

        public void ResetSpoolStateAfter(float seconds)
        {
            Invoke("ResetSpoolState", seconds);
        }
    }
}
