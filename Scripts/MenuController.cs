using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    private Button scanMovieButton;
    private Button addMovieButton;
    private GameObject scanMoviePage;
    private GameObject addMoviePage;
    private char currentPage = 'a';
    private string inactiveColor = "AAABABFF";  // gray
    private string activeColor = "FF7800FF";    // orange

    // Use this for initialization
    void Start () {
        // get pages
        scanMoviePage = GameObject.Find("ScanPosterCanvas");
        addMoviePage = GameObject.Find("AddMovieCanvas");
        activateScanPage();

        // register toggle button click listeners
        scanMovieButton = GameObject.Find("ScanButton").GetComponent<Button>();
        scanMovieButton.onClick.AddListener(activateScanPage);

        addMovieButton = GameObject.Find("AddButton").GetComponent<Button>();
        addMovieButton.onClick.AddListener(activateAddPage);
    }

    // Opens SCAN page if current page is ADD, and vice versa.
    private void activateScanPage()
    {
        Debug.Log("activating SCAN ");

        if (currentPage == 'a')   // current page is add => activate scan page
        {
            Debug.Log("Turning off Add Page");
            if (addMoviePage.activeSelf) addMoviePage.SetActive(false);
            scanMoviePage.SetActive(true);
            // set "SCAN POSTER" button to active color
            GameObject.Find("ScanButtonText").GetComponent<Text>().text = "<color=#" + activeColor + ">SCAN POSTER</color>";
            // set "ADD MOVIE" button to inactive color
            GameObject.Find("AddButtonText").GetComponent<Text>().text = "<color=#" + inactiveColor + ">ADD MOVIE</color>";
            currentPage = 's';
        }
        
    }

    private void activateAddPage()
    {
        Debug.Log("activating ADD ");
        if (currentPage == 's')
        {
            Debug.Log("Turning off Scan Page");
            if (scanMoviePage.activeSelf) scanMoviePage.SetActive(false);
            addMoviePage.SetActive(true);
            // set "ADD MOVIE" button to active color
            GameObject.Find("AddButtonText").GetComponent<Text>().text = "<color=#" + activeColor + ">ADD MOVIE</color>";
            // set "SCAN POSTER" button to inactive color
            GameObject.Find("ScanButtonText").GetComponent<Text>().text = "<color=#" + inactiveColor + ">SCAN POSTER</color>";
            currentPage = 'a';
        }        
    }
	
}
