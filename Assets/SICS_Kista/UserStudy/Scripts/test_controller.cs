using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct _Tuple
{
    GameObject obj;
    bool hightlight;
}

[System.Serializable]
public class MultiDimensionalInt
{
    public Object[] objects;
}

public class test_controller : MonoBehaviour {

    private const int MAX_SCENE_NR = 15;

    private int curSceneNumber = -1;
    public MultiDimensionalInt[] tasks;
    // names of the game objects to use in the test, add more as needed, the order doesn't matter
    private string[] testObjectNames = {
        "tagged_box1",
        "MSH_Hammer",
        "MSH_Screwdriver",
        "scanner",
        "button1",
        "hela_hurtsen",
        "door",
        "Nyckel_vg",
        "Crank",
        "tagged_box3",
        "wrench_with_ratchet",
        "scotch_tape",
        "saw",
        "line_level",
        "monitor",
        "pliers",
        "nut1",
        "MSH_Nail",
        "Lightbulb",
        "Battery",
        "scissors",
        "brush",
        "dril",
        "chisel",
        "SafetyGlasses",
        "Folder",
        "linjal",
        "Worker_Helm",
        "earmuffs",
        "Spray_can",
        "keyboard",
        "armatur"
    };
    private GameObject sideTable;
    private GameObject testTable;
    private GameObject[] testGameObjects;
    private GameObject debugText;
    public Material highlightMat;
    private int clicksToNextScene = 1;

    // Use this for initialization
    void Start()
    {
        // Debug.Log("test_controller - here we go!!");
        //StartTest();
        testGameObjects = new GameObject[testObjectNames.Length];
        for (int i = 0; i < testObjectNames.Length; i++)
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
        testTable = GameObject.Find("testTable");
        sideTable = GameObject.Find("sideTable");
        displayTextOnHud("Press right arrow to start test");
    }

	void StartTest()
    {
       // Debug.Log("test_controller - starting test!");
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

    void ShowAndPositionTestObject(string name)
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
                obj.transform.position = testTable.transform.position;
                //Debug.Log("test_controller.ShowTestObject - test object pos " + obj.transform.position);
                Vector3 up = new Vector3(0, 0.8f, 0); // XXX 
                obj.transform.Translate(up, Space.World);
                //Debug.Log("test_controller.ShowTestObject - test object pos after " + obj.transform.position);
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
        sideTable.SetActive(false);
        UnhighlightAllTestObjects();

        if (clicksToNextScene == 1)
        {
            displayTextOnHud("<-- scene " + sceneNumber + "       scene " + (sceneNumber + 1) + " -->");
        }
        else
        {
            displayTextOnHud("scene " + sceneNumber);
            if (sceneNumber == 0)
            {
                ShowAndPositionTestObject("scotch_tape");
            }
            else if (sceneNumber == 1)
            {
                ShowAndPositionTestObject("MSH_Hammer");
            }
            else if (sceneNumber == 2)
            {
                // scene should be empty, draw circle in the air
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
                ShowAndPositionTestObject("scanner");
            }
            else if (sceneNumber == 5)
            {
                ShowTestObject("door");
            }
            else if (sceneNumber == 6)
            {
                ShowAndPositionTestObject("MSH_Screwdriver");
            }
            else if (sceneNumber == 7)
            {
                ShowAndPositionTestObject("button1");
            }
            else if (sceneNumber == 8)
            {
                ShowTestObject("hela_hurtsen");
                HighlightTestObject("Handtag_1");
            }
            else if (sceneNumber == 9)
            {
                // should be empty, point to floor
            }
            else if (sceneNumber == 10)
            {
                // should be empty, draw triangle in the air
            }
            else if (sceneNumber == 11)
            {
                ShowAndPositionTestObject("saw");
            }
            else if (sceneNumber == 12)
            {
                ShowAndPositionTestObject("scissors");
            }
            else if (sceneNumber == 13)
            {
                ShowAndPositionTestObject("monitor");
            }
            else if (sceneNumber == 14)
            {
                ShowAndPositionTestObject("pliers");
            }
            else if (sceneNumber == 15)
            {
                ShowAndPositionTestObject("MSH_Nail");
                HighlightTestObject("MSH_Nail");
            }
            else if (sceneNumber == 16)
            {
                ShowAndPositionTestObject("wrench_with_ratchet");
            }
            else if (sceneNumber == 17)
            {
                ShowAndPositionTestObject("Spray_can");
            }
            else if (sceneNumber == 18)
            {
                ShowTestObject("Crank");
            }
            else if (sceneNumber == 19)
            {
                ShowAndPositionTestObject("line_level");
            }
            else if (sceneNumber == 20)
            {
                ShowAndPositionTestObject("linjal");
            }
            else if (sceneNumber == 21)
            {
                ShowAndPositionTestObject("Worker_Helm");
            }
            else if (sceneNumber == 22)
            {
                ShowAndPositionTestObject("nut1");
            }
            else if (sceneNumber == 23)
            {
                ShowAndPositionTestObject("Lightbulb");
            }
            else if (sceneNumber == 24)
            {
                ShowAndPositionTestObject("keyboard");
            }
            else if (sceneNumber == 25)
            {
                ShowAndPositionTestObject("Folder");
            }
            else if (sceneNumber == 26)
            {
                ShowAndPositionTestObject("Battery");
            }
            else if (sceneNumber == 27)
            {
                ShowAndPositionTestObject("earmuffs");
            }
            else if (sceneNumber == 28)
            {
                ShowAndPositionTestObject("SafetyGlasses");
            }
            else if (sceneNumber == 29)
            {
                ShowAndPositionTestObject("brush");
            }
            else if (sceneNumber == 30)
            {
                // dimmer
                // ShowAndPositionTestObject("");
            }
            else if (sceneNumber == 31)
            {
                ShowTestObject("armatur");
            }
            else if (sceneNumber == 32)
            {
                ShowAndPositionTestObject("dril");
            }
            else if (sceneNumber == 33)
            {
                ShowAndPositionTestObject("chisel");
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
            else
            {
                displayTextOnHud("END");
                HideAllTestObjects();
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
