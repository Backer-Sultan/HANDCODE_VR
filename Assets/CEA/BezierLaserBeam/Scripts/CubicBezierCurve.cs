using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicBezierCurve {

	public Vector3[] cps = new Vector3[4]; //Control points

	public CubicBezierCurve()
	{
		for (int i = 0; i < 4; i++)
			cps[0] = new Vector3(0, 0, 0);
	}

  public CubicBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
  {
    for (int i = 0; i < 4; i++)
      cps[0] = new Vector3(0, 0, 0);
    SetControlPoints(p0, p1, p2, p3);
  }

  public void SetControlPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		cps[0] = p0;
		cps[1] = p1;
		cps[2] = p2;
		cps[3] = p3;
	}

	public Vector3 GetCurvePoint(float t)
	{
		return	(1 - t) * (1 - t) * (1 - t) * cps[0] +
						3 * (1 - t) * (1 - t) * t * cps[1] +
						3 * (1 - t) * t * t * cps[2] +
						t * t * t * cps[3];
	}

	public Vector3 GetCurveDerivative(float t)
	{
		return 3 * (1 - t) * (1 - t) * (cps[1] - cps[0]) +
						6 * (1 - t) * t * (cps[2] - cps[1]) +
						3 * t * t * (cps[3] - cps[2]);
	}


	public Vector3[] GetCurvePoints(int resolution)
	{
		Vector3[] points = new Vector3[resolution];

		for(int i = 0;i<resolution;i++)
		{
			float t = i / (float) (resolution-1);
			points[i] = GetCurvePoint(t);
		}

		return points;
	}

	public float GetCurveLength(int resolution)
	{
    float length = 0;

		Vector3 p, po = Vector3.zero;
		for (int i = 0; i <= resolution; i++)
		{
			float t = i / (float)resolution;
			p = GetCurvePoint(t);
			if (po != Vector3.zero)
				length += Vector3.Distance(p, po);
			po = p;
		}

		return length;
	}
}