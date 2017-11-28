namespace HandCode
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Baloon_UI : MonoBehaviour
    {
        public GameObject line;
        private void Start()
        {
            line.SetActive(true);
            GetComponentInChildren<LineRenderer>().enabled = true;
        }
    }

}