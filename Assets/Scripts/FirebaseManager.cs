﻿using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;

using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using TMPro;
using SimpleFileBrowser;

public struct MovieData
{
  public string title;
  public string description;
  public string releaseDate;
  public float rating;
  public string genre;
  public string imageUri;
};

public struct UserData
{
  public string userID;
  public int userType;
};

public class FirebaseManager : MonoBehaviour
{
  public static FirebaseManager instance;

  //Firebase variables
  [Header("Firebase")]
  public DependencyStatus dependencyStatus;
  public FirebaseAuth auth;  
  public FirebaseUser user;
  public DatabaseReference dbReference;
  public FirebaseStorage storage;
  public StorageReference storageReference;

  //Login variables
  [Header("Login")]
  public TMP_InputField emailLoginField;
  public TMP_InputField passwordLoginField;
  public TMP_Text warningLoginText;
  public TMP_Text confirmLoginText;

  //Register variables
  [Header("Register")]
  public TMP_InputField usernameRegisterField;
  public TMP_InputField emailRegisterField;
  public TMP_InputField passwordRegisterField;
  public TMP_InputField passwordRegisterVerifyField;
  public TMP_Text warningRegisterText;

  //New Movie variables
  [Header("New Movie")]
  public TMP_InputField titleField;
  public TMP_InputField descriptionField;
  public Single_DatePicker releaseDatePicker;
  public TMP_InputField ratingField;
  public TMP_InputField genreField;
  public ImageLoader imagePicker;

  //List Movies variables
  [Header("List Movies")]
  public Transform listMoviesContent;
  public GameObject movieElementPref;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != null)
    {
      Debug.Log("Instance already exists, destroying object!");
      Destroy(this);
    }
  }

  private void Start()
  {
    //Check that all of the necessary dependencies for Firebase are present on the system
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    {
      dependencyStatus = task.Result;
      if (dependencyStatus == DependencyStatus.Available)
      {
        //If they are avalible Initialize Firebase
        InitializeFirebase();
      }
      else
      {
        Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
      }
    });
  }

  private void InitializeFirebase()
  {
    Debug.Log("Setting up Firebase Auth");
    //Set the authentication instance object
    auth = FirebaseAuth.DefaultInstance;
    dbReference = FirebaseDatabase.DefaultInstance.RootReference;

    storage = FirebaseStorage.DefaultInstance;
    storageReference = storage.GetReferenceFromUrl("gs://peliculas-3d155.appspot.com/");

    //Set the file browser in case we need to load an image
    FileBrowser.SetFilters(false, new FileBrowser.Filter("Images", ".jpg", ".png"));
    FileBrowser.SetDefaultFilter(".jpg");
    FileBrowser.AddQuickLink("Users", "C:\\Users", null);
  }

  public void ClearLoginFields()
  {
    emailLoginField.text = "";
    passwordLoginField.text = "";
  }

  public void ClearRegisterFields()
  {
    usernameRegisterField.text = "";
    emailRegisterField.text = "";
    passwordRegisterField.text = "";
    passwordRegisterVerifyField.text = "";
  }
  
  //Function for the login button
  public void LoginButton()
  {
    //Call the login coroutine passing the email and password
    StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
  }
  //Function for the register button
  public void RegisterButton()
  {
    //Call the register coroutine passing the email, password, and username
    StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
  }
  //Function for the sign out button
  public void SignOutButton()
  {
    auth.SignOut();
    UIManager.instance.LoginScreen();
    ClearRegisterFields();
    ClearLoginFields();
  }

  //Function for the save data button
  public void SaveDataButton()
  {
    StartCoroutine(UpdateMovieData(new MovieData {
      title = titleField.text,
      description = descriptionField.text,
      releaseDate = releaseDatePicker.startDate?.ToString(@"D"),
      rating = float.Parse(ratingField.text),
      genre = genreField.text,
      imageUri = imagePicker.imagePath,
    }));
  }

  //Function for the scoreboard button
  public void ListMoviesButton()
  {
    StartCoroutine(LoadMoviesData());
  }

  private IEnumerator Login(string _email, string _password)
  {
    //Call the Firebase auth signin function passing the email and password
    var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
    //Wait until the task completes
    yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

    if (LoginTask.Exception != null)
    {
      //If there are errors handle them
      Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
      FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
      AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

      string message = "Login Failed!";
      switch (errorCode)
      {
        case AuthError.MissingEmail:
          message = "Missing Email";
          break;
        case AuthError.MissingPassword:
          message = "Missing Password";
          break;
        case AuthError.WrongPassword:
          message = "Wrong Password";
          break;
        case AuthError.InvalidEmail:
          message = "Invalid Email";
          break;
        case AuthError.UserNotFound:
          message = "Account does not exist";
          break;
      }
      warningLoginText.text = message;
    }
    else
    {
      //User is now logged in
      //Now get the result
      user = LoginTask.Result;
      Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
      warningLoginText.text = "";
      confirmLoginText.text = "Logged In";
      StartCoroutine(LoadUserData());

      yield return new WaitForSeconds(1);

      UIManager.instance.MainMenuScreen(); // Change to user data UI
      confirmLoginText.text = "";
      ClearLoginFields();
      ClearRegisterFields();
    }
  }

  private IEnumerator Register(string _email, string _password, string _username)
  {
    if (_username == "")
    {
      //If the username field is blank show a warning
      warningRegisterText.text = "Missing Username";
    }
    else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
    {
      //If the password does not match show a warning
      warningRegisterText.text = "Password Does Not Match!";
    }
    else 
    {
      //Call the Firebase auth signin function passing the email and password
      var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
      //Wait until the task completes
      yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

      if (RegisterTask.Exception != null)
      {
        //If there are errors handle them
        Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
        FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        string message = "Register Failed!";
        switch (errorCode)
        {
          case AuthError.MissingEmail:
            message = "Missing Email";
            break;
          case AuthError.MissingPassword:
            message = "Missing Password";
            break;
          case AuthError.WeakPassword:
            message = "Weak Password";
            break;
          case AuthError.EmailAlreadyInUse:
            message = "Email Already In Use";
            break;
        }
        warningRegisterText.text = message;
      }
      else
      {
        //User has now been created
        //Now get the result
        user = RegisterTask.Result;

        if (user != null)
        {
          //Create a user profile and set the username
          UserProfile profile = new UserProfile{DisplayName = _username};

          //Call the Firebase auth update user profile function passing the profile with the username
          var ProfileTask = user.UpdateUserProfileAsync(profile);
          //Wait until the task completes
          yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

          if (ProfileTask.Exception != null)
          {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
            warningRegisterText.text = "Username Set Failed!";
          }
          else
          {
            //yield return UpdateDBUserData();

            //Username is now set
            //Now return to login screen
            UIManager.instance.LoginScreen();            
            warningRegisterText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
          }
        }
      }
    }
  }

  private IEnumerator UpdateUsernameAuth(string _username)
  {
    //Create a user profile and set the username
    UserProfile profile = new UserProfile { DisplayName = _username };

    //Call the Firebase auth update user profile function passing the profile with the username
    var ProfileTask = user.UpdateUserProfileAsync(profile);
    //Wait until the task completes
    yield return new WaitUntil(() => ProfileTask.IsCompleted);

    if(ProfileTask.Exception != null)
    {
      Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
    }
    else
    {
      //Auth username is now updated
    }
  }

  /*private IEnumerator UpdateDBUserData(UserData data)
  {
    if (data.title == "")
    {
      yield break;
    }

    if (data.imageUri != "")
    {
      //Create a reference to where the file needs to be uploaded
      string extension = Path.GetExtension(data.imageUri);
      string pathStr = "images/" + data.title + extension;
      StorageReference uploadRef = storageReference.Child(pathStr);

      //Editing Metadata
      var newMetadata = new MetadataChange();
      newMetadata.ContentType = "image/" + extension.Substring(1);

      var imageTask = uploadRef.PutFileAsync(data.imageUri, newMetadata);
      yield return new WaitUntil(() => imageTask.IsCompleted);

      if (imageTask.Exception != null)
      {
        data.imageUri = "";
        Debug.LogWarning(message: $"Failed to register task with {imageTask.Exception}");
        yield break;
      }
      else
      {
        var urlTask = imageTask.Result.Reference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => urlTask.IsCompleted);
        data.imageUri = urlTask.Result.AbsoluteUri;
      }
    }

    var DBTask = dbReference.Child("movies").Child(data.title).SetRawJsonValueAsync(
      JsonUtility.ToJson(data)
    );

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
      Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    }
  }*/

  private IEnumerator UpdateMovieData(MovieData data)
  {
    if(data.title == "")
    {
      yield break;
    }

    if(data.imageUri != "")
    {
      //Create a reference to where the file needs to be uploaded
      string extension = Path.GetExtension(data.imageUri);
      string pathStr = "images/" + data.title + extension;
      StorageReference uploadRef = storageReference.Child(pathStr);

      //Editing Metadata
      var newMetadata = new MetadataChange();
      newMetadata.ContentType = "image/" + extension.Substring(1);

      var imageTask = uploadRef.PutFileAsync(data.imageUri, newMetadata);
      yield return new WaitUntil(() => imageTask.IsCompleted);

      if (imageTask.Exception != null)
      {
        data.imageUri = "";
        Debug.LogWarning(message: $"Failed to register task with {imageTask.Exception}");
        yield break;
      }
      else
      {
        var urlTask = imageTask.Result.Reference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => urlTask.IsCompleted);
        data.imageUri = urlTask.Result.AbsoluteUri;
      }
    }

    var DBTask = dbReference.Child("movies").Child(data.title).SetRawJsonValueAsync(
      JsonUtility.ToJson(data)
    );

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if(DBTask.Exception != null)
    {
      Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    }
  }

  private IEnumerator LoadUserData()
  {
    var DBTask = dbReference.Child("users").Child(user.UserId).GetValueAsync();

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if(DBTask.Exception != null)
    {
      Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    }
    /*else if(DBTask.Result.Value == null)
    {
      //No data exists yet
      xpField.text = "0";
      killsField.text = "0";
      deathsField.text = "0";
    }
    else
    {
      //Data has been retrieved
      DataSnapshot snapshot = DBTask.Result;

      xpField.text = snapshot.Child("xp").Value.ToString();
      killsField.text = snapshot.Child("kills").Value.ToString();
      deathsField.text = snapshot.Child("deaths").Value.ToString();
    }*/
  }

  private IEnumerator LoadMoviesData()
  {
    var DBTask = dbReference.Child("movies").OrderByChild("rating").GetValueAsync();

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if(DBTask.Exception != null)
    {
      Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    }
    else
    {
      //Data has been retrieved
      DataSnapshot snapshot = DBTask.Result;

      //Destroy any existing scoreboard elements
      foreach (Transform child in listMoviesContent.transform)
      {
        Destroy(child.gameObject);
      }

      //Loop through every users UID
      foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
      {
        var data = JsonUtility.FromJson<MovieData>(childSnapshot.GetRawJsonValue());

        //Instantiate new scoreboard elements
        GameObject movieElement = Instantiate(movieElementPref, listMoviesContent);
        movieElement.GetComponent<MovieElement>().NewMovieElement(data);
      }

      //Goto scoreboard screen
      UIManager.instance.ListMoviesScreen();
    }
  }

  public IEnumerator ShowLoadDialogCoroutine()
  {
    yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

    if (FileBrowser.Success)
    {
      yield return FileBrowser.Result[0];
    }
  }
}