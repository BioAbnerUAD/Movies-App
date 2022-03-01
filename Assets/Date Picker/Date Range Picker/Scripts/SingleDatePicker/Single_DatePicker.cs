using System;
using System.Linq;
using UnityEngine;

public class Single_DatePicker : DateRangePicker
{
  [SerializeField] DayOfWeek m_FirstDayOfWeek = DayOfWeek.Monday;

  [SerializeField] UITweenManager UITweenManager;
  [SerializeField] bool m_ShowDaysInOtherMonths = false;
  [SerializeField] bool m_CloseOnLastSelection = false;

  [SerializeField] Calender m_Calender;

  [SerializeField] private DateTime? m_StartDate;
  public DateTime? startDate {
    get => m_StartDate;
  }

  private void Start()
  {
    Setup();
  }

  public void SetStartDate(DateTime value)
  {
    m_StartDate = value;
    CalendersUpdated?.Invoke(m_StartDate, m_StartDate);
    m_Calender.Setup(value.Year, value.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public override void Setup()
  {
    m_Calender.PointerEnter = OnPointerEnter;
    m_Calender.PointerDown = OnPointerDown;
    m_Calender.PointerExit = OnPointerExit;
    m_Calender.CalenderRefreshed = OnCalenderRefreshed;
    m_Calender.Setup(DateTime.Now.Year, DateTime.Now.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public void OnPointerEnter(CalenderButton chosenCalenderButton, Calender calender)
  {
    if (chosenCalenderButton.CurrentState == CalenderButton.State.Normal && m_StartDate == null)
    {
      chosenCalenderButton.UpdateState(CalenderButton.State.Hover, calender.Date, m_StartDate, m_StartDate);
    }
  }

  public void OnPointerDown(CalenderButton chosenCalenderButton, DateTime chosenDate, Calender calender)
  {
    // clears selection
    if (m_StartDate != null)
    {
      for (int i = 0; i < 42; i++)
        m_Calender.CalenderButtons[i].ResetToOriginal();

      m_StartDate = null;

      // don't return on this one
    }

    // intiate first click
    if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
    {
      m_StartDate = chosenDate;

      CalendersUpdated?.Invoke(m_StartDate, m_StartDate);
      chosenCalenderButton.UpdateState(CalenderButton.State.Selected, chosenDate, m_StartDate, m_StartDate);
    }

    if (m_CloseOnLastSelection)
    {
      m_Calender.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Generally called when the next month is shown on a calender
  /// </summary>
  public void OnCalenderRefreshed(DateTime calenderDate, CalenderButton calenderButton, DateTime buttonDate)
  {
    calenderButton.UpdateState(CalenderButton.State.Highlighted, buttonDate, m_StartDate, m_StartDate);
  }

  public void OnPointerExit(CalenderButton chosenCalenderButton, Calender calender)
  {
    if (chosenCalenderButton.CurrentState == CalenderButton.State.Hover)
    {
      if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
        chosenCalenderButton.UpdateState(CalenderButton.State.Normal, calender.Date, m_StartDate, m_StartDate);
    }
  }

  private bool DateIsInCalenderMonth(DateTime chosenDate, DateTime calenderDate)
  {
    if (calenderDate.Month == chosenDate.Month)
    {
      return true;
    }

    return false;
  }

  public void OnClick_NextCalenderMonth()
  {
    m_Calender.Date = m_Calender.Date.AddMonths(1);

    m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public void OnClick_NextCalenderYear()
  {
    m_Calender.Date = m_Calender.Date.AddYears(1);

    m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
    m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public void OnClick_PreviousCalenderMonth()
  {
    m_Calender.Date = m_Calender.Date.AddMonths(-1);
    m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public void OnClick_PreviousCalenderYear()
  {
    m_Calender.Date = m_Calender.Date.AddYears(-1);
    m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_StartDate, UITweenManager);
  }

  public void OnClick_ToggleCalenders()
  {
    m_Calender.gameObject.SetActive(!m_Calender.gameObject.activeInHierarchy);
  }
}
