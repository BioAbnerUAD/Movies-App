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

  //Functions to change the login screen UI

  public void ClearScreen() //Turn off all screens
  {
    loginUI.SetActive(false);
    registerUI.SetActive(false);
    mainMenuUI.SetActive(false);
    newMovieUI.SetActive(false);
    listMoviesUI.SetActive(false);
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
    mainMenuUI.SetActive(true);
  }

  public void NewMovieScreen() //Scoreboard button
  {
    ClearScreen();
    newMovieUI.SetActive(true);
  }

  public void ListMoviesScreen() //Scoreboard button
  {
    ClearScreen();
    listMoviesUI.SetActive(true);
  }
}
