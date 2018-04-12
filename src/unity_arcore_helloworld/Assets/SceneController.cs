using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

    public GameObject trackedPlanePrefab;
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
