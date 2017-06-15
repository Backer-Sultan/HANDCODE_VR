using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawnerDestroyer : MonoBehaviour
{
    public GameObject arrowPrefab;
    [Tooltip("You need to exit play mode and start again to catch the change!")]
    [Range(0.1f, 5f)]
    public float SpawnsPerSecond = 0.5f;
    private WaitForSeconds wait;

    private void Start()
    {
        wait = new WaitForSeconds(1f / SpawnsPerSecond);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return wait;
            GameObject newArrow = GameObject.Instantiate(arrowPrefab, transform.parent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("DirectionIndictatorArrow"))
        {
            GameObject.Destroy(other.gameObject);
        }
    }

    private void OnValidate()
    {
        wait = new WaitForSeconds(1f / SpawnsPerSecond);
    }
}
