using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
  public static UIManager instance;

  //Screen object variables
  public GameObject loginUI;
  public GameObject registerUI;
  public GameObject mainMenuUI;
  public GameObject newMovieUI;
  public GameObject listMoviesUI;
  public GameObject listEditsUI;
  public GameObject listUsersUI;

  private void Awake()
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
    ClearScreen();
    LoginScreen();
  }

  //Functions to change the login screen UI

  public void ClearScreen() //Turn off all screens
  {
    loginUI.SetActive(false);
    registerUI.SetActive(false);
    mainMenuUI.SetActive(false);
    newMovieUI.SetActive(false);
    listMoviesUI.SetActive(false);
    listUsersUI.SetActive(false);
    listEditsUI.SetActive(false);
  }

  public void LoginScreen()
  {
    ClearScreen();
    loginUI.SetActive(true);
  }

  public void RegisterScreen()
  {
    ClearScreen();
    registerUI.SetActive(true);
  }

  public void MainMenuScreen()
  {
    ClearScreen();

    int userType = FirebaseManager.instance.userData.userType;
    
    var buttonsTfrm = mainMenuUI.transform.GetChild(1);
    buttonsTfrm.GetChild(0).gameObject.SetActive(userType > 0); // New Movie Button
    buttonsTfrm.GetChild(2).gameObject.SetActive(userType > 1); // List Users Button
    buttonsTfrm.GetChild(3).gameObject.SetActive(userType > 1); // List Edits Button

    mainMenuUI.SetActive(true);
  }

  public void NewMovieScreen(string title = "Película Nueva")
  {
    ClearScreen();
    newMovieUI.SetActive(true);
    newMovieUI.transform.GetChild(0).gameObject.name = title;
  }

  public void ListMoviesScreen()
  {
    ClearScreen();
    listMoviesUI.SetActive(true);
  }

  public void ListEditsScreen()
  {
    ClearScreen();
    listEditsUI.SetActive(true);
  }

  public void ListUsersScreen()
  {
    ClearScreen();
    listUsersUI.SetActive(true);
  }
}
