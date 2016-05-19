using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddMovieController : MonoBehaviour {
    Texture2D img;
    private string tmdbId = "";
    private TMDBMovie movie = null;
    private OMDBMovie omdbMovie = null;
    CloudUpLoader uploader;

    // flags
    private bool hasTmdbId;
    private bool hasPosterURL;

    // register click handlers on startup
    void start()
    {
        Debug.Log("initializing click handler");
        

        // register "Submit Button" click handler
        Button openTrailerButton = GameObject.Find("Submit Movie").GetComponent<Button>();
        openTrailerButton.onClick.AddListener(uploadToCloud);
    }

    public void uploadToCloud()
    {
        if (tmdbId != null && img != null)
        {
            Debug.Log("<color=red>Uploading Image for "+tmdbId+"</color>");
            StartCoroutine(CloudUpLoader.PostNewTarget(img, tmdbId));
        }
    }

    void onGUI()
    {
        Debug.Log("onGUI(): Showing movie poster");
        GUILayout.Label(img);
    }

	public void GetInput(string movieTitle)
    {
        Debug.Log("You entered " + movieTitle);
        if (movieTitle != "") StartCoroutine(getPoster(movieTitle));
    }

    private IEnumerator getPoster(string movieTitle)
    {
        // get TMDB ID
        yield return StartCoroutine(QueryAPI.queryTmdbIdFromTitle(movieTitle, value => tmdbId = value));
        if (tmdbId == "")
        {
            GameObject.Find("TMDB ID").GetComponent<Text>().text = "Movie doesn't exist!";
            yield break;
        }
        else
        {
            GameObject.Find("TMDB ID").GetComponent<Text>().text = "ID: " + tmdbId;
        }

        // get poster path using TMDB ID
        TMDBandOMDB movieWrapperObj = null;
        yield return StartCoroutine(QueryAPI.queryMovieInfo(tmdbId, value => movieWrapperObj = value));
        omdbMovie = movieWrapperObj.om;
        if (omdbMovie == null)
        {
            Debug.Log("omdbMovie iS NULL");
            yield break;
        }

        // load the poster url
        string posterURL = omdbMovie.Poster;
        Debug.Log("POSTER: " + posterURL);
        GameObject posterImage = GameObject.Find("Poster Image");
        yield return ImageDownloader.downloadImage(posterImage, posterURL, tmdbId, value => img = value);
        

    }

    IEnumerator LoadImg(string posterURL)
    {
        yield return 0;
        WWW imgLink = new WWW(posterURL);
        yield return imgLink;
        img = imgLink.texture;
    }

}
