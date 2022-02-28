using UnityEngine;
using UnityEngine.UI;

using TMPro;

enum UserTypeEnum
{
  User = 0,
  Editor = 1,
  Admin = 2
}

public class UserElement : MonoBehaviour
{
  public TMP_Text userNameText;
  public TMP_Text emailText;
  public TMP_Text userTypeText;
  public Toggle verifiedToggle;
  public Button deleteBtn; 

  public string userID;

  public void NewUserElement(UserData data)
  {
    userID                      = data.userID;
    userNameText.text           = data.userName;
    emailText.text              = data.email;
    userTypeText.text           = ((UserTypeEnum)data.userType).ToString();
    verifiedToggle.isOn         = data.isVerified;
    verifiedToggle.interactable = data.userType < 2;
    deleteBtn.interactable      = data.userType < 2;
    verifiedToggle.onValueChanged.AddListener(onVerifiedChange);
  }

  public void onVerifiedChange(bool value)
  {
    FirebaseManager.instance.SetUserVerified(userID, value);
  }

  public void DeleteUser()
  {
    FirebaseManager.instance.DeleteUser(userID);
    Destroy(gameObject);
  }
}
