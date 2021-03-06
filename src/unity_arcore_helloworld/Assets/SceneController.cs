﻿using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

    public GameObject trackedPlanePrefab;
    public Camera firstPersonCamera;
    public ScoreboardController scoreboard;

    // Use this for initialization
    void Start () {
        QuitOnConnectionErrors();
    }
	
	// Update is called once per frame
	void Update () {
        // The session status must be Tracking in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ProcessNewPlanes();

        ProcessTouches();
    }

    void QuitOnConnectionErrors()
    {
        // Do not update if ARCore is not tracking.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                  "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            // This covers a variety of errors.  See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            StartCoroutine(CodelabUtils.ToastAndExit(
              "ARCore encountered a problem connecting. Please restart the app.", 5));
        }
    }

    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Trackable as TrackedPlane);
            // Add to the end of SetSelectedPlane.
        }
    }


    void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        Debug.Log("Selected plane centered at " + selectedPlane.CenterPose.position);
        scoreboard.SetSelectedPlane(selectedPlane);
    }

    void ProcessNewPlanes()
    {
        var planes = new List<TrackedPlane>();
        Session.GetTrackables(planes, TrackableQueryFilter.New);

        foreach(var plane in planes)
        {
            var planeObject = Instantiate(trackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(plane);
        }
    }
}
