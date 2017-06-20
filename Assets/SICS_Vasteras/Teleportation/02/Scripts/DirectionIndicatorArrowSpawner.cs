/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;

public class DirectionIndicatorArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    [Tooltip("You need to exit play mode and start again to catch the change!")]
    [Range(0.1f, 5f)]
    public float SpawnsPerSecond = 0.5f;

    private WaitForSeconds wait;

   
    private void Start()
    {
        //initialization
        if (arrowPrefab == null)
            Debug.LogError("DirectionIndicatorArrowSpawner.cs: arrowPrefab is not assigned!");
        wait = new WaitForSeconds(1f / SpawnsPerSecond);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            GameObject newArrow = Instantiate(arrowPrefab, transform, false);
            yield return wait;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        foreach(DirectionIndicatorArrow arrow in GetComponentsInChildren<DirectionIndicatorArrow>())
        {
            StopAllCoroutines();
            if(arrow.name.Contains("(Clone)"))
            {
                Destroy(arrow.gameObject);
            }
        }
    }
}
