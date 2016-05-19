using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ImageDownloader : MonoBehaviour {
    static string file_path = "/29veIwD38rVL2qY74emXQw4y25H.jpg";
    static string tmdb_api_key = "";    // TODO: read key from config file
    static string url = "http://image.tmdb.org/t/p/w780" + file_path + "?api_key=" + tmdb_api_key;
    
    // Use this for initialization
    public static IEnumerator downloadImage(GameObject go, string imagePath, string name, System.Action<Texture2D> result) {

        print("Downloading image");
        WWW www = new WWW(imagePath);
        yield return www;   // wait for www to finish downloading before resuming
        Texture2D texture = www.texture;    // grab the image
        result(texture);
        go.GetComponent<Renderer>().material.mainTexture = texture;
        byte[] bytes = texture.EncodeToJPG();
        File.WriteAllBytes(Application.persistentDataPath + "poster.jpg", bytes);

    }
}
