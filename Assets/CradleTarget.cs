using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HandCode
{
    public class CradleTarget : MonoBehaviour
    {
        public Machine machine;

        private void Start()
        {
            machine = FindObjectOfType<Machine>();
            if (machine == null)
                Debug.Log(string.Format("{0}\nCradleTarget.cs: Machine.cs is not found in the scene!", Machine.GetPath(gameObject)));
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Cradle")
                machine.cradle.MarkReachedTarget();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Cradle")
                machine.cradle.MarkLeftTarget();
        }
    }
}
