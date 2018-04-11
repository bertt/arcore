using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        QuitConnectionErrors();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void QuitConnectionErrors()
    {
        Debug.Log("berttemme call QuitConnectionErrors");

        if ((Session.Status == SessionStatus.ErrorPermissionNotGranted))
        {
            StartCoroutine(CodelabUtils.ToastAndExit("Camera position not given", 5));
        }
        else if (Session.Status.IsError())
        {
            StartCoroutine(CodelabUtils.ToastAndExit("Other error has occured, restart app", 5));
        }
        else
        {
            
            StartCoroutine(CodelabUtils.ToastAndExit("Allo good!", 5));
        }
    }
}
