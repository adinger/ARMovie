using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuiUtils {

    /// <summary>
    ///     Activates and populates the infobox with info for the movie
    /// </summary>
    /// <param name="movie">Movie object</param>
    public static void activateInfobox(GameObject infoPanelObject, OMDBMovie movie, List<Review> reviews)
    {        
        if (movie != null)
        {
            infoPanelObject.SetActive(true);
            Debug.Log("<color=purple>POPULATING INFO FOR "+movie.Title+"</color>");
            if (GameObject.Find("Title") == null) Debug.Log("TITLE IS NULL!!!!!!!");
            GameObject.Find("Title").GetComponent<Text>().text = movie.Title.ToUpper();
            GameObject.Find("BasicInfo").GetComponent<Text>().text =
                movie.Director + "   |   " + movie.Year + "   |   " + movie.Rated;
            GameObject.Find("Plot").GetComponent<Text>().text = movie.Plot;
            populateReviewBox(reviews);
        } else
        {
            Debug.Log("MOVIE IS NULL");
        }
        
        
    }

    /// <summary>
    ///     Makes the infobox disappear when the target can 
    ///     no longer be seen
    /// </summary>
    public static void deactivateInfobox(GameObject infoPanelObject)
    {
        if (infoPanelObject) infoPanelObject.SetActive(false);
    }

    /// <summary>
    ///     Fills in the Reviews section of UI with reviews
    /// </summary>
    public static void populateReviewBox(List<Review> reviews)
    {
        string reviewsString = "";
        if (reviews != null)
        {
            foreach (Review review in reviews)
            {
                reviewsString += "\n\"" + review.content +
                    "\"\n- " + review.author + "\n**************************";
            }
            GameObject.Find("Reviews").GetComponent<Text>().text = reviewsString;
        }
        
    }

}
