using UnityEngine;

public class TwoCirclesTangents
{ 
  public Vector2 c0, c1;
  public float r0, r1;
  public bool side0, side1;

  public Vector2 it0,it1;

  private const float epsilon = 10 * float.Epsilon;

  public TwoCirclesTangents()
  {

  }


  public void CalculateTangents()
  {
    float D = Vector2.Distance(c0, c1);
    if (D > Mathf.Abs(r0 - r1)) //Checking the existance of tangent lines
    {
      float xp, yp; // intersection point of the two tangent line
      bool switched = false;
      if (side0 == side1) //Outer tangents
      {
        if(Mathf.Abs(r0-r1)< epsilon) //The two circle have equal radius, there is no tangents intersection point 
        {
          Vector2 rv = c0 -c1;
          //if((rv.x>0 && side0) || (rv.x<=0 && !side0))
          if (side0)
            rv = new Vector2(rv.y, -rv.x);
          else 
            rv = new Vector2(-rv.y, rv.x);

          rv.Normalize();
          rv *= r0;

          it0 = c0 + rv;
          it1 = c1 + rv;

          return;
        }
        else if (r0 < r1) //switching circle 0 and 1 r0 as to be bigger than r1
        {
          Vector2 c = c0;
          c0 = c1;
          c1 = c;

          float r = r0;
          r0 = r1;
          r1 = r;

          side0 = side1 = !side0;
          switched = true;
        }

        //Calculating the intersection point between the two outer tangents
        xp = (c1.x * r0 - c0.x * r1) / (r0 - r1);
        yp = (c1.y * r0 - c0.y * r1) / (r0 - r1);
      }
      else //Inner tangents
      {
        //Calculating the intersection point between the two inner tangents
        xp = (c1.x * r0 + c0.x * r1) / (r0 + r1);
        yp = (c1.y * r0 + c0.y * r1) / (r0 + r1);
      }

      Vector2 p = new Vector2(xp, yp);

      //Determining the side of tangent point of circle 1
      float s0x = 1;
      float s0y = -1;
      if(side0)
      {
        s0x = -1;
        s0y = 1;
      }

      //Calculating the tangent point on cercle 0
      float sqrdC0P = Vector2.SqrMagnitude(p - c0);
      float x0 = ((r0 * r0 * (xp - c0.x) + s0x * r0 * (yp - c0.y) * Mathf.Sqrt(sqrdC0P - r0 * r0)) / sqrdC0P) + c0.x;
      float y0 = ((r0 * r0 * (yp - c0.y) + s0y * r0 * (xp - c0.x) * Mathf.Sqrt(sqrdC0P - r0 * r0)) / sqrdC0P) + c0.y;

      //Determining the side of tangent point of circle 1
      
      float s1x = 1;
      float s1y = -1;
      if (side1)
      {
        s1x = -1;
        s1y = 1;
      }

      if (side0 != side1)
      {
        s1x = -s1x;
        s1y = -s1y;
      }

      //Calculating the tangent point on cercle 1
      float sqrdC1P = Vector2.SqrMagnitude(p - c1);
      float x1 = ((r1 * r1 * (xp - c1.x) + s1x * r1 * (yp - c1.y) * Mathf.Sqrt(sqrdC1P - r1 * r1)) / sqrdC1P) + c1.x;
      float y1 = ((r1 * r1 * (yp - c1.y) + s1y * r1 * (xp - c1.x) * Mathf.Sqrt(sqrdC1P - r1 * r1)) / sqrdC1P) + c1.y;

      it0 = new Vector2(x0, y0);
      it1 = new Vector2(x1, y1);

      if(switched) //switching circle 0 and 1
      {
        Vector2 it = it0;
        it0 = it1;
        it1 = it;

        Vector2 c = c0;
        c0 = c1;
        c1 = c;

        float r = r0;
        r0 = r1;
        r1 = r;

        side0 = side1 = !side0;
      }
    }
  }
}