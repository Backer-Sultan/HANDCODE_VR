using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ClothAnimation))]
public class ClothAnimationEditor : Editor
{

  private ClothAnimation clothAnimation;
  private SerializedProperty cloth;
  private SerializedProperty animationSpeed;
  private SerializedProperty keypoints;
  private SerializedProperty constraintPoint;

  void OnEnable()
  {
    clothAnimation = target as ClothAnimation;
    cloth = serializedObject.FindProperty("cloth");
    keypoints = serializedObject.FindProperty("keypoints");
    animationSpeed = serializedObject.FindProperty("animationSpeed");
    constraintPoint = serializedObject.FindProperty("constraintPoint");
  }


  // Update is called once per frame
  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    GUIContent labelCloth = new GUIContent("Animated cloth");
    EditorGUILayout.PropertyField(cloth, labelCloth);

    GUILayout.Label("Animation path");
    EditorGUI.indentLevel++;
    for (int i=0;i< keypoints.arraySize;i++)
      EditorGUILayout.PropertyField(keypoints.GetArrayElementAtIndex(i),true);
    EditorGUI.indentLevel--;

    EditorGUILayout.PropertyField(constraintPoint, new GUIContent("Constraint point"));

    GUIContent labelLength = new GUIContent("Animation speed");
    EditorGUILayout.PropertyField(animationSpeed, labelLength);

    serializedObject.ApplyModifiedProperties();

    if (GUI.changed)
    {
      EditorUtility.SetDirty(target);
    }
  }

  public void OnSceneGUI()
  {
    if (clothAnimation.keypoints[0] != null && clothAnimation.keypoints[1] != null && clothAnimation.keypoints[2] != null  && clothAnimation.keypoints[3] != null)
    {
      CubicBezierCurve curve = new CubicBezierCurve();

      curve.SetControlPoints(clothAnimation.keypoints[0].position,
                            clothAnimation.keypoints[1].position,
                            clothAnimation.keypoints[2].position,
                            clothAnimation.keypoints[3].position);

      Vector3[]  points = curve.GetCurvePoints(20);
      Handles.color = new Color(1, 0, 0, 1);
      for (int i = 0; i < points.Length - 1; i++)
      {
          Handles.DrawLine(points[i], points[i + 1]);
      }
    }
  }

}
