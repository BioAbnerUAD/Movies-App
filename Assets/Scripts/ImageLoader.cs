using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
  public RawImage rawImage;
  public string imagePath = "";

  void Start()
  {
    rawImage = GetComponent<RawImage>();
  }

  public void OnImageClick()
  {
    StartCoroutine(LoadImage());
  }

  IEnumerator LoadImage()
  {
    CoroutineWithData cd = new CoroutineWithData(
      FirebaseManager.instance, 
      FirebaseManager.instance.ShowLoadDialogCoroutine()
    );

    yield return cd.coroutine;
    string path = cd.result as string;
    if (path != "")
    {
      //Create a request
      UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path);
      //Wait for the request to complete
      yield return request.SendWebRequest();
      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Error when fetching image: " + request.error);
      }
      else
      {
        //Setting the loaded image to our object
        imagePath = path;
        rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
      }
    }
  }
}
