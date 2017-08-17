using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(PaperFlow))]
public class PaperFlowEditor : Editor
{

  private PaperFlow m_PaperFlow;
  private SerializedProperty rollers;
  private SerializedProperty sides;
  private SerializedProperty width, material, topLeftUV, bottomRightUV, subdivision;

  void OnEnable()
  {
    m_PaperFlow = target as PaperFlow;
    rollers = serializedObject.FindProperty("rollers");
    sides = serializedObject.FindProperty("sides");
    width = serializedObject.FindProperty("width");
    material = serializedObject.FindProperty("material");
    topLeftUV = serializedObject.FindProperty("topLeftUV");
    bottomRightUV = serializedObject.FindProperty("bottomRightUV");
    subdivision = serializedObject.FindProperty("subdivision");
  }


  // Update is called once per frame
  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    int nbCapsules = EditorGUILayout.IntField("Number of rollers:", m_PaperFlow.rollers.Count);

    if (nbCapsules >= 0)
    {
      if (m_PaperFlow.rollers == null)
      {
        m_PaperFlow.rollers = new List<CapsuleCollider>();
        m_PaperFlow.sides = new List<bool>();
      }

      if (nbCapsules < m_PaperFlow.rollers.Count)
      {
        m_PaperFlow.rollers.RemoveRange(nbCapsules, m_PaperFlow.rollers.Count - nbCapsules);
        m_PaperFlow.sides.RemoveRange(nbCapsules, m_PaperFlow.rollers.Count - nbCapsules);
      }
      else
      {
        for (int i = m_PaperFlow.rollers.Count; i < nbCapsules; i++)
        {
          if (i > 0)
          {
            m_PaperFlow.rollers.Add(m_PaperFlow.rollers[i - 1]);
            m_PaperFlow.sides.Add(m_PaperFlow.sides[i - 1]);
          }
          else
          {
            m_PaperFlow.rollers.Add(null);
            m_PaperFlow.sides.Add(false);
          }
        }
      }
    }

    for (int i = 0; i < rollers.arraySize; i++)
    {
      EditorGUI.indentLevel++;
      GUILayout.BeginHorizontal();

      SerializedProperty rollerProperty = rollers.GetArrayElementAtIndex(i);
      GUIContent labelRoll = new GUIContent("Roll " + i);
      EditorGUILayout.PropertyField(rollerProperty, labelRoll, true);

      SerializedProperty sideProperty = sides.GetArrayElementAtIndex(i);
      GUIContent labelSide = new GUIContent("Flip Side");
      EditorGUILayout.PropertyField(sideProperty, labelSide, true);

      GUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }

    GUIContent labelWidth = new GUIContent("Paper width");
    EditorGUILayout.PropertyField(width, labelWidth);

    GUIContent labelMaterial = new GUIContent("Paper material");
    EditorGUILayout.PropertyField(material, labelMaterial);

    m_PaperFlow.showAdvance = EditorGUILayout.Foldout(m_PaperFlow.showAdvance, "Advance parammeters");

    if (m_PaperFlow.showAdvance)
    {
      EditorGUI.indentLevel++;
      GUIContent labelLeftUV = new GUIContent("UV top left coordonates");
      EditorGUILayout.PropertyField(topLeftUV, labelLeftUV);

      GUIContent labelRightUV = new GUIContent("UV bottom right coordonates");
      EditorGUILayout.PropertyField(bottomRightUV, labelRightUV);

      GUIContent labelSubdivision = new GUIContent("Arc vertices subdivision");
      EditorGUILayout.PropertyField(subdivision, labelSubdivision);
      EditorGUI.indentLevel--;
    }

    serializedObject.ApplyModifiedProperties();

    if (GUI.changed)
    {
      EditorUtility.SetDirty(target);
      m_PaperFlow.BuildKeypoints();
    }
  }

  public void OnSceneGUI()
  {
    Handles.color = new Color(1, 0, 0, 1);
    for (int i = 0; i < m_PaperFlow.keypoints.Length - 1; i++)
    {
      if (m_PaperFlow.keypoints[i] != null && m_PaperFlow.keypoints[i + 1] != null)
      {
        int rollerId0 = (i + 2* m_PaperFlow.subdivision + 2) / (m_PaperFlow.subdivision +2);
        int rollerId1 = (i+1 + 2 * m_PaperFlow.subdivision + 2) / (m_PaperFlow.subdivision + 2);
        if (m_PaperFlow.rollers[rollerId0] != null && m_PaperFlow.rollers[rollerId1] != null)
        {
          Handles.DrawLine(m_PaperFlow.keypoints[i].position, m_PaperFlow.keypoints[i + 1].position);
        }
        else
          break;
      }
    }
  }

}
