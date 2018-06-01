using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(ChecklistUI))]
public class ChecklistUIEditor : Editor
{

  private ChecklistUI m_Checklist;

  // Use this for initialization
  void OnEnable()
  {
    m_Checklist = (ChecklistUI)target;
    m_Checklist.m_Frame = m_Checklist.transform.GetChild(0).GetComponent<RectTransform>();
  }

  public override void OnInspectorGUI()
  {

    //Title Update
    //Transform frame = m_Checklist.m_Frame;

    m_Checklist.SetTitle(EditorGUILayout.TextField("Checklist Title", m_Checklist.GetTitle()));
    
    int nbItems = EditorGUILayout.IntField("Number of UI elements:", m_Checklist.GetItemCount());

    if (nbItems < 1)
      return;

    for (int i = 0; i < nbItems; i++)
    {
      if (i >= m_Checklist.GetItemCount())
        m_Checklist.AddItem();

      GUILayout.Label("Item " + i);
      EditorGUI.indentLevel++;
      GUILayout.BeginHorizontal();
      m_Checklist.SetItemCheck(i,EditorGUILayout.Toggle(m_Checklist.GetItemCheck(i), GUILayout.Width(30)));
      GUILayout.Label("Text", GUILayout.Width(55));
      m_Checklist.SetItemText(i, EditorGUILayout.TextField(m_Checklist.GetItemText(i), GUILayout.ExpandWidth(true)));
      GUILayout.EndHorizontal();
      EditorGUI.indentLevel--;

    }

    while (m_Checklist.GetItemCount() > nbItems)
      Destroy(m_Checklist.m_Frame.GetChild(m_Checklist.GetItemCount()));

    if (GUI.changed)
    {
      EditorUtility.SetDirty(target);
      m_Checklist.Reshape();
    }

    if (GUILayout.Button("Check/Uncheck all"))
    {
      bool check = !m_Checklist.GetItemCheck(0);
      for (int i = 0; i < m_Checklist.GetItemCount(); i++)
        m_Checklist.SetItemCheck(i, check);
    }

  }

  private void Destroy(Transform trans)
  {
    while (trans.childCount != 0)
      Destroy(trans.GetChild(0));

    GameObject.DestroyImmediate(trans.gameObject);
  }
}