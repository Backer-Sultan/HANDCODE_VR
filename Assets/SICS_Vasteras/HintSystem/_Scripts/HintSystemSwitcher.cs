//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using VRTK;

//namespace HandCode
//{
//    public class HintSystemSwitcher : MonoBehaviour
//    {

//        public HintSystem hs;
//        public bool showState = true;


//        private void Start()
//        {
//            hs = GameObject.FindObjectOfType<HintSystem>();
//            GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += new ControllerInteractionEventHandler(SwitchState);

//        }

//        private void SwitchState(object sender, ControllerInteractionEventArgs e)
//        {
//            print("Pressed!");
//            showState = !showState;
//            hs.gameObject.SetActive(showState);

//        }
        
//    } 
//}
