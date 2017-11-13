using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_controller : MonoBehaviour {

    private int sceneNumber = 0;
    // names of the game objects to use in the test, add more as needed
    private string[] testObjectNames = { "tagged_box1", "tagged_box3", "MSH_Hammer", "MSH_Screwdriver", "scanner", "button1", "hela_hurtsen" };
    private GameObject[] testGameObjects;

    // Use this for initialization
    void Start() {
        Debug.Log("test_controller - here we go!!");
    }

	void StartTest() {
        Debug.Log("test_controller - starting test!");
        testGameObjects = new GameObject[testObjectNames.Length];
        for (int i=0; i<testObjectNames.Length; i++)
        {
            testGameObjects[i] = GameObject.Find(testObjectNames[i]);
            if (!testGameObjects[i])
            {
                Debug.Log("test_controller.StartTest - can't find object with name " + testObjectNames[i]);
            }
            else
            {
                Debug.Log("test_controller.StartTest - fount object with name " + testObjectNames[i]);
            }
        }
        sceneNumber = 0;
		HideAllTestObjects ();
		ShowNextScene ();
	}

	void HideAllTestObjects() {
		for (int i=0; i< testObjectNames.Length; i++)
		{
			HideTestObject (testObjectNames[i]);
		}
	}

    int getTestObjectIndex(string name)
    {
        for (int i = 0; i < testObjectNames.Length; i++)
        {
            if (testObjectNames[i] == name)
            {
                return i;
            }
        }

        return -1;
    }

	void HideTestObject(string name) {
        int index = getTestObjectIndex(name);
        if (index < 0)
        {
            Debug.Log("test_controller.HideTestObject - can't find index of object " + name);
        }
        else
        {
            Debug.Log("test_controller.ShowTestObject - index " + index);
            GameObject obj = testGameObjects[index];
            if (obj)
            {
                Debug.Log("test_controller.ShowTestObject - hiding object with index " + index);
                obj.SetActive(false);
            }
            else
            {
                Debug.Log("test_controller.HideTestObject - can't find object with index " + index);
            }
        }
    }

	void ShowTestObject(string name) {
        int index = getTestObjectIndex(name);
        if (index < 0)
        {
            Debug.Log("test_controller.ShowTestObject - can't find index of object " + name);
        }
        else
        { 
            GameObject obj = testGameObjects[index];
            if (obj)
            {
                Debug.Log("test_controller.ShowTestObject - showing object with index " + index);
                obj.SetActive(true);
            }
            else
            {
                Debug.Log("test_controller.ShowTestObject - can't find object with index " + index);
            }
        }
    }

    void ShowNextScene() {
        Debug.Log("test_controller.ShowNextScene - enter " + sceneNumber);
        HideAllTestObjects();
        sceneNumber++;
        if (sceneNumber == 1)
        {
        }
        else if (sceneNumber == 2)
        {
            ShowTestObject("MSH_Hammer");
        }
        else if (sceneNumber == 3)
        {
            ShowTestObject("hela_hurtsen");
        }
        else if (sceneNumber == 4)
        {
            ShowTestObject("tagged_box1");
            ShowTestObject("scanner");
        }
        else if (sceneNumber == 5)
        {
            ShowTestObject("door");
        }
        else if (sceneNumber == 6)
        {
            ShowTestObject("MSH_Screwdriver");
        }
        else if (sceneNumber == 7)
        {
            ShowTestObject("button1");
        }
        else if (sceneNumber == 8)
        {
            ShowTestObject("hela_hurtsen");
        }
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
			if (sceneNumber==0)
            {
				StartTest ();
			}
            else
            {
				ShowNextScene ();
			}
		}
	}
}
