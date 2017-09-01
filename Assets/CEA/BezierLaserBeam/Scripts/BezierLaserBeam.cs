using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLaserBeam : MonoBehaviour
{

    public Transform origin, destination;
    public LineRenderer line;
    [Range(2, 100)]
    public int lineResolution = 10;

    [Range(0, 100)]
    public float curvature = 50;

    public float textureOffsetSpeed = 0.1f;
    public float textureScale = 1;

    private CubicBezierCurve curve;
    private const float curvatureCoef = 2 / 3.0f * 0.01f;


    // Use this for initialization
    void Start()
    {
        curve = new CubicBezierCurve();
        line = GetComponent<LineRenderer>();
        line.positionCount = lineResolution;
    }

    // Update is called once per frame
    void Update()
    {

        if (line)
        {
            float dist = curvatureCoef * curvature * Vector3.Distance(origin.position, destination.position);

            curve.SetControlPoints(destination.position,
                              destination.position + (dist * destination.forward),
                              origin.position + (dist * origin.forward),
                              origin.position);

            line.positionCount = lineResolution;
            line.SetPositions(curve.GetCurvePoints(lineResolution));
            if (line.material)
            {
                Vector2 offset = line.material.GetTextureOffset("_MainTex");
                line.material.SetTextureOffset("_MainTex", new Vector2(offset.x += Time.deltaTime * textureOffsetSpeed, offset.y));
                float curveLength = curve.GetCurveLength(lineResolution);

                Vector2 scale = line.material.GetTextureScale("_MainTex");
                line.material.SetTextureScale("_MainTex", new Vector2(textureScale * curveLength, scale.y));
            }
        }
    }

    private void OnEnable()
    {
        line.enabled = true;
    }

    private void OnDisable()
    {
        line.enabled = false;
    }
}
