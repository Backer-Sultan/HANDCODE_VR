using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;

/// <summary>
/// Allows moving the camera in Play mode, when the associated camera is active.
/// 
/// Uses XDE conventions for the mouse
/// </summary>
public class OrbitalCamera : MonoBehaviour
{
  public float xSpeed = 120.0f;
  public float ySpeed = 72.0f;
  public float zSpeed = 15.0f;
  public float panSpeed = 10.0f;
  public float defaultDistance = 1.0f;
  public Transform target;
  public Transform trackedTransform;
  public bool keep_pitch = false;

  private Transform mTarget;
  public float minDistance = 0.05f;
  private float X, Y;
  private Vector3 center;
  private bool toUpdate = false;
  private bool onDrag = false;
  private float  distance = 5.0f;

  private Camera cam = null;

  // Use this for initialization
  void Start()
  {
    if (target)
    {
      center = target.position;
      transform.LookAt(center);
      distance = (center - transform.position).magnitude;
    }
    else
    {
      distance = defaultDistance;
      center = transform.rotation * (new Vector3(0.0f, 0.0f, distance)) + transform.position;
    }

    Vector3 xyz = transform.rotation.eulerAngles;
    X = xyz.x;
    Y = xyz.y;

    cam = GetComponent<Camera>();
  }

  // Update is called once per frame
  void LateUpdate()
  {
    if (cam != null && !cam.enabled)
      return;

    //Ignore UI layer
    if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
    {
      //We can only start moving if we click outside UI element
      if (Input.GetButtonDown("Fire3") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire1"))
        onDrag = true;
    }

    if (Input.GetButtonUp("Fire3") || Input.GetButtonUp("Fire2") || Input.GetButtonUp("Fire1"))
      onDrag = false;

    if (onDrag && Input.GetButton("Fire3"))
    {

      float dx = Input.GetAxis("Mouse X") * panSpeed * -0.01f;
      float dy = Input.GetAxis("Mouse Y") * panSpeed * -0.01f;

      center += transform.rotation * new Vector3(dx, dy, 0.0f);
      toUpdate = true;
    }
    if (onDrag && Input.GetButton("Fire2"))
    {
      if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
      {
        //real zoom
        float Z = Input.GetAxis("Mouse Y") * zSpeed * -0.005f;
        if (Z != 0.0f)
        {
          distance += distance * Z;
          distance = Mathf.Max(minDistance, distance);
          toUpdate = true;
        }
      }
      else
      {
        //Z pan
        float dz = Input.GetAxis("Mouse Y") * zSpeed * 0.01f;
        center += transform.rotation * new Vector3(0, 0, dz);
        toUpdate = true;
      }
    }

    if (onDrag && Input.GetButton("Fire1"))
    {
      Y += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
      if (! keep_pitch)
        X -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

      Y = Y % 360;
      X = X % 360;
      toUpdate = true;
    }

    if(trackedTransform != null)
      toUpdate = true;

    if (toUpdate)
    {
      Quaternion rotation = Quaternion.Euler(X, Y, 0);
      Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -distance)) + center;

      if (trackedTransform != null)
        position += trackedTransform.position;

      transform.rotation = rotation;
      transform.position = position;
      toUpdate = false;
    }
  }
}
