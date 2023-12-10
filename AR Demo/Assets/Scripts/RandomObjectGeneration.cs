using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class RandomObjectGeneration : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;
    private ARRaycastManager raycastManager;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        StartCoroutine(GenerateObjects());
    }

    IEnumerator GenerateObjects()
    {
        int maxObjects = 3;

        while (true)
        {
            if (spawnedObjects.Count < maxObjects)
            {
                bool objectSpawned = false;

                if (raycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hitList, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hitList[0].pose;

                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                    Vector3 spawnPosition = hitPose.position + randomOffset;

                    bool positionValid = true;
                    foreach (GameObject obj in spawnedObjects)
                    {
                        if (Vector3.Distance(obj.transform.position, spawnPosition) < 0.3f)
                        {
                            positionValid = false;
                            break;
                        }
                    }

                    if (positionValid)
                    {
                        GameObject newObject = Instantiate(gameObjectToInstantiate, spawnPosition, hitPose.rotation);
                        spawnedObjects.Add(newObject);
                        objectSpawned = true;
                    }
                }

                if (!objectSpawned)
                {
                    // If no object was spawned in the current iteration, yield for a shorter duration before retrying
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                // If max objects reached, yield for a longer duration before checking again
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}