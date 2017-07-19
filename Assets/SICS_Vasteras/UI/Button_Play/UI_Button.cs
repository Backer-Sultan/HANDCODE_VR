/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.UI;

namespace HandCode
{
    public class UI_Button : MonoBehaviour
    {
        Button btn;

        private void Start()
        {
            btn = GetComponent<Button>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finger")
                btn.onClick.Invoke();
        }
    }
}