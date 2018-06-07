using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualGrasp;

public class VG_Recorder : MonoBehaviour
{
    public enum RecordingMode
    {
        RECORD_ON_PLAY,
        REPLAY_ON_PLAY,
        MANUAL
    }

    /// Recording mode can be set by developer
    public RecordingMode m_recordingMode = RecordingMode.MANUAL;
    /// Filename to save recording to / read replay from
    public string m_recordingFilename = "recording.sdb";

    private bool m_isRecording = false;

    void Start()
    {
        switch (m_recordingMode)
        {
            case RecordingMode.RECORD_ON_PLAY:
                ToggleRecording();
                break;
            case RecordingMode.REPLAY_ON_PLAY:
                StartReplay();
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (m_recordingMode == RecordingMode.RECORD_ON_PLAY)
            ToggleRecording();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleRecording();
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartReplay();
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartSingleReplay();
    }

    public void ToggleRecording()
    {
        m_isRecording = !m_isRecording;
        if (m_isRecording) VG_Controller.StartRecording();
        else VG_Controller.StopRecording(m_recordingFilename);
    }

    public void StartReplay()
    {
        if (m_isRecording)
        {
            Debug.Log("VG is recording. Stop recording before replay.");
            return;
        }
        VG_Controller.LoadRecording(m_recordingFilename);
        VG_Controller.StartReplay();
    }

    public void StartSingleReplay()
    {
        GameObject obj = GameObject.Find("FMGP_PRE_Carrot_1024");
        if (obj != null)
        {
            //for (int i = 3; i<10; i++)
            VG_Controller.StartReplayOnObject(obj, 1, VG_HandSide.LEFT, 5);
        }
    }
}
