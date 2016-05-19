using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;


public class QueryAPI {
    private static string tmdb_api_key = "";    // TODO: read key from config file

    /// <summary>
    /// Gets a movie's TMDB ID from its string title
    /// </summary>
    /// <param name="title">movie title as string</param>
    /// <param name="result">the movie's tmdbId as string</param>
    /// <returns></returns>
    public static IEnumerator queryTmdbIdFromTitle(string title, System.Action<string> result)
    {
        float elapsedTime = 0.0f;

        // construct the query url
        string url = "http://api.themoviedb.org/3/search/movie?query=" + Uri.EscapeUriString(title) + "&api_key=" + tmdb_api_key;
        Debug.Log("<color=blue>getTmdbIdFromTitle Query:\n " + url + "</color>");
        WWW www = new WWW(url);
        yield return www;   // wait for www to finish

        // give up after 10 seconds of waiting
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f) break;
            yield return null;
        }

        // get the tmdb id from the first movie in the returned data
        string responseText = www.text;
        PossibleMovies jsonData = JsonMapper.ToObject<PossibleMovies>(responseText);

        if (jsonData.results.Count > 0) // make sure the movie actually exists
        {
            string tmdbId = jsonData.results[0].id.ToString();
            result(tmdbId);
        }
        
    }

    /// <summary>
    /// Gets TMDB's base URL for images, the domain for all of TMDB's images
    /// </summary>
    /// <param name="result">the base URL as a string</param>
    /// <returns></returns>
    public static IEnumerator queryImagesBaseURL(System.Action<string> result)
    {
        float elapsedTime = 0.0f;

        // construct the query url
        string url = "http://api.themoviedb.org/3/configuration?api_key=" + tmdb_api_key;
        WWW www = new WWW(url);
        yield return www;   // wait for www to finish before continuing

        // give up after 10 seconds of waiting
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f) break;
            yield return null;
        }

        // create the Movie object from the returned data
        string responseText = www.text;
        JsonData jsonData = JsonMapper.ToObject(responseText);
        if (JsonDataContainsKey(jsonData, "images"))
        {
            // extract the tmdbId from the results
            string base_url = jsonData["images"]["base_url"].ToString();
            result(base_url);
        }
    }
   
    /// <summary>
    ///  Queries the OMDB api
    /// </summary>
    /// <param name="tmdbId">TMDB ID of the movie</param>
    /// <param name="omdbMovie">Movie object containing all the info for it</param>
    /// <returns></returns>
    public static IEnumerator queryMovieInfo(string tmdbId, System.Action<TMDBandOMDB> result)
    {
        float elapsedTime = 0.0f;
        // construct the query url
        string url = "http://api.themoviedb.org/3/movie/" + tmdbId + "?api_key=" + tmdb_api_key;
        WWW www = new WWW(url);
        yield return www;   // wait for www to finish

        // give up after 10 seconds of waiting
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f) break;
            Debug.Log("Giving up!");
            yield return null;
        }

        // create the Movie object from the returned data
        string responseText = www.text;
        //Debug.Log("<color=green>Movie Info: " + responseText + "</color>");

        JsonData jsonData = JsonMapper.ToObject(responseText);
        if (JsonDataContainsKey(jsonData, "title"))
        {
            TMDBMovie tm = JsonMapper.ToObject<TMDBMovie>(responseText);
            //Debug.Log("<color=green>Created TMDB Object: " + tm.title + "</color>");
            OMDBMovie om = null;
            yield return queryOMDB(tm.imdb_id, value => om = value);  // OMDB uses IMDB ID
            //Debug.Log("<color=green>Created OMDB Object: " + om.Title + "</color>");

            // wrap both movie objects into one object to return
            TMDBandOMDB tmdbAndOmdb = new TMDBandOMDB(om, tm);
            InfoPopup.movie = om;
            InfoPopup.isInfoReady = true;
            result(tmdbAndOmdb);

        }
    }

    // get all the movie info from OMDB based on its IMDB ID
    public static IEnumerator queryOMDB(string imdbId, System.Action<OMDBMovie> result)
    {
        float elapsedTime = 0.0f;
        // construct the query url
        string url = "http://www.omdbapi.com/?i="+ imdbId + "&plot=short&r=json";
        WWW www = new WWW(url);
        yield return www;   // wait for www to finish

        // give up after 10 seconds of waiting
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f) break;
            Debug.Log("Giving up!");
            yield return null;
        }
        
        string responseText = www.text;
        //Debug.Log("<color=green>OMDB Response: " + responseText + "</color>");

        JsonData jsonData = JsonMapper.ToObject(responseText);
        if (JsonDataContainsKey(jsonData, "Title"))
        {
            OMDBMovie omdbMovie = JsonMapper.ToObject<OMDBMovie>(responseText);
            Debug.Log("<color=green>Created Movie Object: " + omdbMovie.Title + "</color>");
            result(omdbMovie);
        }
    }
    
    /// <summary>
    /// Gets a movie's trailer
    /// </summary>
    /// <param name="tmdbId">movie's IMDB ID</param>
    /// <param name="result">Youtube URL for the movie's trailer</param>
    /// <returns>"result" is returned</returns>
    public static IEnumerator queryVideoURL(string tmdbId, System.Action<string> result)
    {
        // e.g. http://api.themoviedb.org/3/movie/120/videos?api_key=[tmdb_api_key]
        float elapsedTime = 0.0f;

        // construct the query url
        string url = "http://api.themoviedb.org/3/movie/" + tmdbId + "/videos?api_key=" + tmdb_api_key;
        //Debug.Log("getVideoURL Query: " + url);
        WWW www = new WWW(url);
        
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 13.0f) break;
            yield return null;
        }
        
        // extract the video's Youtube key from the JSON response
        string responseText = www.text;
        JsonData jsonData = JsonMapper.ToObject(responseText);
        
        if (JsonDataContainsKey(jsonData, "results"))
        {
            Videos videos = JsonMapper.ToObject<Videos>(responseText);
            // iterate through list of videos, looking for the trailer
            foreach (Video vid in videos.results)
            {
                if (vid.type == "Trailer")
                {
                    string videoURL = "https://www.youtube.com/embed/" + vid.key;   // full screen video
                    //Debug.Log("VideoURL: " + videoURL);
                    result(videoURL);
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Gets a movie's reviews.
    /// </summary>
    /// <param name="tmdbId"></param>
    /// <param name="result">"return" value is a List<Review> type</param>
    /// <returns></returns>
    public static IEnumerator queryReviews(string tmdbId, System.Action<List<Review>> result)
    {
        // http://api.themoviedb.org/3/movie/100/reviews?api_key=[tmdb_api_key]
        float elapsedTime = 0.0f;

        // construct the query url
        string url = "http://api.themoviedb.org/3/movie/" + tmdbId + "/reviews?api_key=" + tmdb_api_key;
        WWW www = new WWW(url);

        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 13.0f) break;
            Debug.Log("getVideoURL() is giving up!");
            yield return null;
        }

        // extract the reviews from the JSON response
        string responseText = www.text;
        Debug.Log("reviews: " + responseText);
        JsonData jsonData = JsonMapper.ToObject(responseText);

        if (JsonDataContainsKey(jsonData, "results")) {
            Reviews reviews = JsonMapper.ToObject<Reviews>(responseText);
            result(reviews.results); // return the list of reviews
        }        

    }

    // source: https://gist.github.com/sinergy/5626704
    /// <summary>
    /// Checks if the key is in the json object
    /// </summary>
    /// <param name="data">the JSON object</param>
    /// <param name="key">the key you want to check for in the data</param>
    /// <returns>True if the key is in the json, false otherwise</returns>
    public static bool JsonDataContainsKey(JsonData data, string key)
    {
        bool result = false;
        if (data == null)
            return result;
        if (!data.IsObject)
        {
            return result;
        }
        IDictionary tdictionary = data as IDictionary;
        if (tdictionary == null)
            return result;
        if (tdictionary.Contains(key))
        {
            result = true;
        }
        return result;
    }

    
}