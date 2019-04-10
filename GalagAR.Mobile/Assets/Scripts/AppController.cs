using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.ComputerVision;
using System.IO;

public class AppController : MonoBehaviour {

    private bool m_IsQuitting = false;

    private List<DetectedPlane> _allPlanes = new List<DetectedPlane>();
    private List<Vector3> _allPlaneBoundaryPoints = new List<Vector3>();
    private List<GameObject> _allBoundaryMarkers = new List<GameObject>();

    public GameObject _verticalPlaneFoundMarker;
    public GameObject _horizontalPlaneFoundMarker;
    public GameObject _planeBoundaryMarker;

	// Use this for initialization
	void Start () {
        QuitOnConnectionErrors();

        _verticalPlaneFoundMarker.SetActive(false);
        _horizontalPlaneFoundMarker.SetActive(false);
	}

    // Update is called once per frame
	void Update () {
        foreach(GameObject boundaryMarker in _allBoundaryMarkers)
        {
            boundaryMarker.SetActive(false);
        }

        Session.GetTrackables<DetectedPlane>(_allPlanes);

        bool horizontalPlanesFound = false;
        bool verticalPlanesFound = false;

        int boundaryPointCount = 0;
        for (int i = 0; i < _allPlanes.Count; i++)
        {
            //if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            //{
            //    showSearchingUI = false;
            //    break;
            //}

            if(_allPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing
               || _allPlanes[i].PlaneType == DetectedPlaneType.HorizontalDownwardFacing)
            {
                horizontalPlanesFound = true;

                _allPlanes[i].GetBoundaryPolygon(_allPlaneBoundaryPoints);
                foreach(var boundaryPoint in _allPlaneBoundaryPoints)
                {
                    if(_allBoundaryMarkers.Count < boundaryPointCount)
                    {
                        _allBoundaryMarkers[boundaryPointCount].transform.position = boundaryPoint;
                        _allBoundaryMarkers[boundaryPointCount].SetActive(true);
                    }
                    else 
                    {
                        var newMarker = Instantiate(_planeBoundaryMarker);
                        newMarker.transform.position = boundaryPoint;
                        newMarker.SetActive(true);
                        _allBoundaryMarkers.Add(newMarker);
                    }
                    boundaryPointCount++;
                }
            }

            if (_allPlanes[i].PlaneType == DetectedPlaneType.Vertical)
            {
                verticalPlanesFound = true;

                _allPlanes[i].GetBoundaryPolygon(_allPlaneBoundaryPoints);
                foreach (var boundaryPoint in _allPlaneBoundaryPoints)
                {
                    if (_allBoundaryMarkers.Count < boundaryPointCount)
                    {
                        _allBoundaryMarkers[boundaryPointCount].transform.position = boundaryPoint;
                        _allBoundaryMarkers[boundaryPointCount].SetActive(true);
                    }
                    else
                    {
                        var newMarker = Instantiate(_planeBoundaryMarker);
                        newMarker.transform.position = boundaryPoint;
                        newMarker.SetActive(true);
                        _allBoundaryMarkers.Add(newMarker);
                    }
                    boundaryPointCount++;
                }
            }
        }

        _verticalPlaneFoundMarker.SetActive(verticalPlanesFound);
        _horizontalPlaneFoundMarker.SetActive(horizontalPlanesFound);

        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;

            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;		
	}

    private void QuitOnConnectionErrors()
    {
        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }

    }

    private static void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
