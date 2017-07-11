/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Machine : MonoBehaviour
{
    /* fields & properties */

    [HideInInspector]
    public Cradle cradle;
    [HideInInspector]
    public ArmRig armRig_Right, armRig_Left;
    [HideInInspector]
    public Spool spool_Left, spool_Right;
    [HideInInspector]
    public MainConsole mainConsole;
    [HideInInspector]
    public CradleConsole cradleConsole;
    [HideInInspector]
    public PaperConsole paperConsole;



    /* methods & coroutines */

    // returns GameObject's full path in the hierarchy.
    public static string GetPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    private void Start()
    {
        cradle = GetComponentInChildren<Cradle>();
        if (!cradle)
            Debug.LogError("Machine.cs: Cradle script is missing!");

        ArmRig[] armRigs = GetComponentsInChildren<ArmRig>();
        foreach (ArmRig rig in armRigs)
        {
            if (rig.ID.Trim().ToLower() == "left")
            {
                armRig_Left = rig;
                continue;
            }
            if (rig.ID.Trim().ToLower() == "right")
            {
                armRig_Right = rig;
                continue;
            }
        }
        //    Debug.LogError("Machine.cs: ArmRig script is missing");

        Spool[] spools = GetComponentsInChildren<Spool>();
        foreach (Spool spl in spools)
        {
            if (spl.ID == "left")
            {
                spool_Left = spl;
                continue;
            }
            if (spl.ID == "right")
            {
                spool_Right = spl;
                continue;
            }
        }
        if (!spool_Left || !spool_Right)
            Debug.LogError("Machine.cs: Spool script is missing!");

        mainConsole = GetComponentInChildren<MainConsole>();
        if (!mainConsole)
            Debug.LogError("Machine.cs: MainConsole script is missing!");

        cradleConsole = GetComponentInChildren<CradleConsole>();
        if (!cradleConsole)
            Debug.LogError("Machine.cs: CradleConsole script is missing!");

        paperConsole = GetComponentInChildren<PaperConsole>();
        if (!paperConsole)
            Debug.LogError("Machine.cs: PaperConsole script is missing!");
    }
}
