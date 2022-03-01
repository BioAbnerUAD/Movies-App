using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreInput : MonoBehaviour
{
  public TMP_InputField field;
  string oldValue;

  void Start()
  {
    oldValue = field.text;
  }

  public void OnTextChange()
  {
    float value;
    if(float.TryParse(field.text, out value))
    {
      float mod = value % 0.5f;
      if (mod != 0.0f) {
        field.text = (value - mod).ToString();
      }
      if(value > 5 || value < 0)
      {
        field.text = Mathf.Clamp((float)value, 0, 5).ToString();
      }
    }
    else
    {
      field.text = oldValue;
    }
  }
}
