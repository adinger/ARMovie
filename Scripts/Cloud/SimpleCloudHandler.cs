using System;
using UnityEngine;
using Vuforia;
using System.Collections;

public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;
    private string mTargetName = "";

    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        mTargetName = targetSearchResult.TargetName;
        // stop scanning when a result is found in the cloud
        mCloudRecoBehaviour.CloudRecoEnabled = false;
    }

    public void OnGUI()
    {
        // Display current 'scanning' status
        GUI.Box(new Rect(20, 0, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(20, 100, 200, 50), "Name: " + mTargetName);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(20, 200, 200, 50), "Restart Scanning"))
            {
                // Restart TargetFinder
                mCloudRecoBehaviour.CloudRecoEnabled = true;
            }
        }
    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;
        if (scanning)
        {
            // clear all known trackables
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);
        }
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }




}
