using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_controller : MonoBehaviour {

    private const int MAX_SCENE_NR = 8;

    private int curSceneNumber = -1;
    // names of the game objects to use in the test, add more as needed, the order doesn't matter
    private string[] testObjectNames = { "tagged_box1", "MSH_Hammer", "MSH_Screwdriver", "scanner", "button1", "hela_hurtsen", "door", "Nyckel_vg", "Crank", "Lever", "tagged_box3" };
    private GameObject[] testGameObjects;
    private GameObject debugText;
    public Material highlightMat;
    private int clicksToNextScene = 1;

    // Use this for initialization
    void Start()
    {
       // Debug.Log("test_controller - here we go!!");
        StartTest();
    }

	void StartTest()
    {
       // Debug.Log("test_controller - starting test!");
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
               // Debug.Log("test_controller.StartTest - found object with name " + testObjectNames[i]);
            }
        }
		HideAllTestObjects ();
        displayTextOnHud("Press right arrow to start test");
	}

    void displayTextOnHud(string text)
    {
        debugText = GameObject.Find("TestSceneText");
        if (debugText)
        {
            debugText.GetComponent<Text>().text = text;
        }
    }

    void UnhighlightTestObject(string name)
    {
        int index = getTestObjectIndex(name);
        if (index < 0)
        {
            Debug.Log("test_controller.UnhighlightTestObject - can't find index of object " + name);
        }
        else
        {
           // Debug.Log("test_controller.UnhighlightTestObject - index " + index);
            GameObject obj = testGameObjects[index];
            if (obj)
            {
                // do something
            }
            else
            {
                Debug.Log("test_controller.UnhighlightTestObject - can't find object with index " + index);
            }
        }
    }

    void UnhighlightAllTestObjects()
    {
        for (int i = 0; i < testObjectNames.Length; i++)
        {
            UnhighlightTestObject(testObjectNames[i]);
        }
    }

    void HighlightTestObject(string name)
    {
        int index = getTestObjectIndex(name);
        if (index < 0)
        {
            Debug.Log("test_controller.HighlightTestObject - can't find index of object " + name);
        }
        else
        {
            // Debug.Log("test_controller.HighlightTestObject - index " + index);
            GameObject obj = testGameObjects[index];
            if (obj)
            {
                obj.GetComponent<Renderer>().material = highlightMat;
                // do something
            }
            else
            {
                Debug.Log("test_controller.HighlightTestObject - can't find object with index " + index);
            }
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

    void HideAllTestObjects()
    {
        for (int i = 0; i < testObjectNames.Length; i++)
        {
            HideTestObject(testObjectNames[i]);
        }
    }

    void HideTestObject(string name)
    {
        int index = getTestObjectIndex(name);
        if (index < 0)
        {
            Debug.Log("test_controller.HideTestObject - can't find index of object " + name);
        }
        else
        {
           // Debug.Log("test_controller.HideTestObject - index " + index);
            GameObject obj = testGameObjects[index];
            if (obj)
            {
                obj.SetActive(false);
            }
            else
            {
                Debug.Log("test_controller.HideTestObject - can't find object with index " + index);
            }
        }
    }

	void ShowTestObject(string name)
    {
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
                obj.SetActive(true);
            }
            else
            {
                Debug.Log("test_controller.ShowTestObject - can't find object with index " + index);
            }
        }
    }

    void ShowScene(int sceneNumber)
    {
        // Debug.Log("test_controller.ShowNextScene - enter " + sceneNumber);
        
        HideAllTestObjects();
        UnhighlightAllTestObjects();
        if (clicksToNextScene == 1)
        {
            displayTextOnHud("Click right to reach scene " + (sceneNumber + 1) + ", click left to reach scene " + sceneNumber);
        }
        else
        {
            displayTextOnHud("Current scene is " + sceneNumber);
            if (sceneNumber == 0)
            {
                ShowTestObject("tagged_box3");
            }
            else if (sceneNumber == 1)
            {
                // scene should be empty
            }
            else if (sceneNumber == 2)
            {
                ShowTestObject("MSH_Hammer");
            }
            else if (sceneNumber == 3)
            {
                ShowTestObject("hela_hurtsen");
                ShowTestObject("Nyckel_vg");
                HighlightTestObject("Nyckel_vg");
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
                HighlightTestObject("Handtag_1");
            }
            else if (sceneNumber == 9)
            {
                ShowTestObject("Crank");
            }
            else if (sceneNumber == 10)
            {
                ShowTestObject("Lever");
            }
            else
            {
                Debug.Log("test_controller.ShowNextScene - invalid scene number " + sceneNumber);
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (curSceneNumber < MAX_SCENE_NR)
            {
                clicksToNextScene--;
                if (clicksToNextScene == 0)
                {
                    curSceneNumber++;
                    clicksToNextScene = 2;
                }
                ShowScene(curSceneNumber);
            }
		}
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (curSceneNumber > 0)
            {
                clicksToNextScene--;
                if (clicksToNextScene == 0)
                {
                    clicksToNextScene = 2;
                }
                else
                {
                    curSceneNumber--;
                }
                ShowScene(curSceneNumber);
            }
           
        }
    }
}
