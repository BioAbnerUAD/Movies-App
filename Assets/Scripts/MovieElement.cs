using System;
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
  public Button deleteBtn;

  string imageUrl;
  string entryName;

  public void NewMovieElement(string _entryName, MovieData data)
  {
    entryName = _entryName;

    titleText.text       = data.title;
    descriptionText.text = data.description;
    releaseDateText.text = data.releaseDate;
    ratingText.text      = data.rating.ToString();
    genreText.text       = data.genre;
    imageUrl             = data.imageUri;

    deleteBtn.interactable = FirebaseManager.instance.userData.userType > 0;
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

  public void EditMovie()
  {
    var firebase = FirebaseManager.instance;
    firebase.titleField.text = titleText.text;
    firebase.descriptionField.text = descriptionText.text;
    firebase.releaseDatePicker.startDate = DateTime.Parse(releaseDateText.text);
    firebase.ratingField.text = ratingText.text;
    firebase.genreField.text = genreText.text;
    firebase.imagePicker.rawImage.texture = rawImage.texture;
    UIManager.instance.NewMovieScreen("Editar Película");
  }

  public void DeleteMovie()
  {
    FirebaseManager.instance.DeleteMovie(titleText.text);
    Destroy(gameObject);
  }

  public void DeleteEdit()
  {
    FirebaseManager.instance.DeleteEdit(entryName);
    Destroy(gameObject);
  }

  public void ConfirmEdit()
  {
    FirebaseManager.instance.UpdateData(new MovieData
    {
      title = titleText.text,
      description = descriptionText.text,
      releaseDate = releaseDateText.text,
      rating = float.Parse(ratingText.text),
      genre = genreText.text,
      imageUri = imageUrl
    });
  }
}
