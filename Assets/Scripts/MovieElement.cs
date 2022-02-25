using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using TMPro;

public class MovieElement : MonoBehaviour
{

  public TMP_Text titleText;
  public TMP_Text descriptionText;
  public TMP_Text releaseDateText;
  public TMP_Text ratingText;
  public TMP_Text genreText;
  public RawImage rawImage;

  string imageUrl;

  public void NewMovieElement(MovieData data)
  {
    titleText.text       = data.title;
    descriptionText.text = data.description;
    releaseDateText.text = data.releaseDate;
    ratingText.text      = data.rating.ToString();
    genreText.text       = data.genre;
    imageUrl             = data.imageUri;
  }

  void Start()
  {
    StartCoroutine(LoadImage(imageUrl));
  }

  IEnumerator LoadImage(string MediaUrl)
  {
    UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
    yield return request.SendWebRequest(); //Wait for the request to complete
    if (request.result != UnityWebRequest.Result.Success)
    {
      Debug.LogError("Error when fetching image: " + request.error);
    }
    else
    {
      // setting the loaded image to our object
      rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
  }
}
