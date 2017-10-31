using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HandCode
{
    public class GameManager : MonoBehaviour
    {

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
    } 
}
