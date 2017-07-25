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
    [RequireComponent(typeof(Image))]
    public class UI_Button : MonoBehaviour
    {
        /* fields & properties */

        public Color clickColor;
        public UnityEvent onClick;

        private Image img;
        private Color color;



        /* methods & coroutines */

        private void Start()
        {
            img = GetComponent<Image>();
            color = img.color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finger")
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right), 1f);
                img.color = clickColor;
                onClick.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Finger")
            {
                img.color = color;
            }
        }

    }
}