/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;

namespace HandCode
{
    public class UI_Button : MonoBehaviour
    {
        public Color clickColor;
        public UnityEvent onClick;

        private Image img;
        private Color color;

        private void Start()
        {
            img = GetComponent<Image>();
            color = img.color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finger")
            {
                img.color = clickColor;
                    onClick.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Finger")
            {
                img.color = color;
            }
        }
       
    }
}