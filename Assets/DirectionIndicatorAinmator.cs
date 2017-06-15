using UnityEngine;

public class DirectionIndicatorAinmator : MonoBehaviour
{
    [Range(0f, 1f)]
    public float speed = 0.5f;

    private void Update()
    {
         transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
}
