using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Vuforia;

public class InfoPopup : MonoBehaviour, ICloudRecoEventHandler
{
    GameObject infoPanelObject;
    private GameObject trailerTexture;
    private TrackableBehaviour mTrackableBehaviour;
    public static bool isInfoReady = false;
    private static bool isInfoPage = false;

    public static OMDBMovie movie;
    private static string tmdbId = "";
    private static string videoURL = "";
    private static List<Review> reviews;

    // Cloud Event Handling variables
    public ImageTargetBehaviour ImageTargetTemplate;
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private string mTargetName = "";

    /// <summary>
    /// Called once when camera/app is turned on
    /// </summary>
    void Start()
    {
        infoPanelObject = GameObject.Find("InfoPopupContainer");
        Debug.Log("Starting application");
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }

        // set the trailer view texture and register close trailer button's click handler before deactivating trailer view
        trailerTexture = GameObject.Find("WebTexture");
        Button closeTrailerButton = GameObject.Find("CloseTrailer").GetComponent<Button>();
        closeTrailerButton.onClick.AddListener(closeTrailer);

        if (trailerTexture.activeSelf) trailerTexture.SetActive(false);

        // register trailer button's click handler
        Button openTrailerButton = GameObject.Find("trailerButton").GetComponent<Button>();
        openTrailerButton.onClick.AddListener(openTrailer);
        
        // register imdb button's click handler
        Button openImdbPageButton = GameObject.Find("imdbPageButton").GetComponent<Button>();
        openImdbPageButton.onClick.AddListener(openImdbPage);
    }
   
    // Update is called once per frame
    void Update()
    {
        // TODO Final3: make infobox follow the target
        if (isInfoPage && isInfoReady)
        {
            GuiUtils.activateInfobox(infoPanelObject, movie, reviews);
        }
        else
        {
            GuiUtils.deactivateInfobox(infoPanelObject);
        }
    }

    /// <summary>
    /// Detects if a trackable has just been seen or is being tracked.
    /// Raises flag to show info if a target is in view.
    /// </summary>
    /// <param name="previousStatus"></param>
    /// <param name="newStatus"></param>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            isInfoPage = true; 
        } else
        {
            isInfoPage = false;
        }
    }
    
    /// <summary>
    /// Called every frame, each time checking if a movie
    /// has been detected, so its info should be shown.
    /// </summary>
    void OnGUI()
    {
        if (isInfoPage && isInfoReady)
        {
            Debug.Log("<color=green>OnGUI shouldShowInfo!!!</color>");
            GuiUtils.activateInfobox(infoPanelObject, movie, reviews);
        }         
        else if (infoPanelObject)
        {
            GuiUtils.deactivateInfobox(infoPanelObject);
        }
        // show below for debugging purposes
        //if (tmdbId != "") GUI.Box(new Rect(0, 200, 350, 50), "tmdbId: " + tmdbId);
    }

    private void openTrailer()
    {
        if (videoURL != "")
        {
            // pass the video's url to the TrailerTexture.cs script that is attached to the trailerTexture object
            trailerTexture.GetComponent<TrailerTexture>().url = videoURL;
            trailerTexture.SetActive(true);            
        }
    }

    // closes the trailer when the 'x' button is clicked in the trailer view. 
    private void closeTrailer() 
    {
        // "close" youtube by redirecting to "about:blank" so the audio doesn't persist
        trailerTexture.GetComponent<TrailerTexture>().view.LoadURL("about:blank");  
        trailerTexture.SetActive(false);
    }
    
    private void openImdbPage()
    {
        if (movie.imdbID != "") Application.OpenURL("http://www.imdb.com/title/"+movie.imdbID+"/");
    }
    
    /////////// CLOUD RECO EVENT HANDLERS ////////////
    
    /// <summary>
    /// When a search result is found in the cloud db, get its info from the TMDB API
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        mTargetName = targetSearchResult.TargetName;
        isInfoPage = true;
        // query the APIs for movie info
        tmdbId = parseImageName(mTargetName);
        getMovieInfo(tmdbId);     // moved here from OnGUI() so that API is only called once
        getVideoURL(tmdbId);
        getReviews(tmdbId);
    }

    // 
    public void OnStateChanged(bool scanning)
    {
        // clear all known trackables
        ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        tracker.TargetFinder.ClearTrackables(false);
    }

    // cloud reco error handlers
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");
    }

    /// <summary>
    /// Parses a target name like "123456_1" and returns just "123456", the movie id
    /// </summary>
    /// <param name="name">Extracts just the id from the image name</param>
    /// <returns>movie id</returns>
    private string parseImageName(string name)
    {
        char delimiter = '_';
        string[] substrings = name.Split(delimiter);
        return substrings[0];
    }

    /////////////////////// wrappers for QueryAPI's methods //////////////////////

    // wrapper for QueryMovieInfo coroutine
    public void getMovieInfo(string tmdbId)
    {
        TMDBandOMDB wrapperMovieObj = null;
        StartCoroutine(QueryAPI.queryMovieInfo(tmdbId, value => wrapperMovieObj = value));
    }

    // wrapper for getTmdbIdFromTitle coroutine
    public void getTmdbIdFromTitle(string title)
    {
        StartCoroutine(QueryAPI.queryTmdbIdFromTitle(title, value => tmdbId = value));
    }

    // Wrapper for getVideoURL coroutine
    public void getVideoURL(string tmdbId)
    {
        StartCoroutine(QueryAPI.queryVideoURL(tmdbId, value => videoURL = value));
    }

    public void getReviews(string tmdbId)
    {
        StartCoroutine(QueryAPI.queryReviews(tmdbId, value => reviews = value));
    }
}
