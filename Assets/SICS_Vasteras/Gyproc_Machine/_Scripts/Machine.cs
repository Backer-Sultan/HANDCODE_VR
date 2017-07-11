/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class Machine : MonoBehaviour
    {
        /* fields & properties */
        public enum Identifier
        {
            NONE,
            LEFT,
            Right,
        }
        [HideInInspector]
        public Cradle cradle;
        [HideInInspector]
        public ArmRig armRig_Right, armRig_Left;
        [HideInInspector]
        public Spool spool_Left, spool_Right;



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
                if (rig.ID == Identifier.Right)
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
                if (spl.ID == Identifier.Right)
                {
                    spool_Right = spl;
                    continue;
                }
            }
            if (spool_Left == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: No spool with id `Left` is found!", GetPath(gameObject)));
            if (spool_Right == null)
                Debug.LogError(string.Format("{0}\nMachine.cs: No spool with id `Right` is found!", GetPath(gameObject)));
        }
    }
}
