using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMarkerPair : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    private Dictionary<string, GameObject> spawnedGameObj = new Dictionary<string, GameObject>();
    private ARTrackedImageManager imgManager;

    private void Awake()
    {
        imgManager = FindObjectOfType<ARTrackedImageManager>();

        foreach (GameObject gObj in prefabs)
        {
            GameObject newGObj = Instantiate(gObj, Vector3.zero, Quaternion.identity);
            newGObj.name = gObj.name;
						newGObj.SetActive(false);
            spawnedGameObj.Add(gObj.name, newGObj);
        }

    }

    private void OnEnable()
    {
        imgManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        imgManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
						//Disable GameObject on image lost on ARKit
            spawnedGameObj[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {

        string name = trackedImage.referenceImage.name;
				GameObject prefab = spawnedGameObj[name];

        if (trackedImage.trackingState == TrackingState.None || trackedImage.trackingState == TrackingState.Limited)
        {
            prefab.SetActive(false);//Disable Gameobject on image lost on ARCore
        }
        else
        {
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true);
        }
    }
}