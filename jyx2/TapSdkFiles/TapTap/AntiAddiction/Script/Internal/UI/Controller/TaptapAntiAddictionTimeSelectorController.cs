using System;
using LC.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TapSDK.UI;
using TapSDK.UI.AillieoTech;
using TapTap.AntiAddiction;
using TapTap.AntiAddiction.Model;
using TapTap.AntiAddiction.Internal;

/// <summary>
/// 年/月/日 选项中的每个 item
/// </summary>
namespace TapTap.AntiAddiction.Internal {
    public class DateTimeItem : MonoBehaviour
    {
        public TaptapAntiAddictionTimeSelectorController context;
        public int type;
        public int index;

        public Button clicker;

        public Image backgroundImage;

        public Text text;

        public void Init()
        {
            clicker = transform.GetComponent<Button>();
            backgroundImage = gameObject.GetComponent<Image>();
            text = transform.Find("Label").GetComponent<Text>();

            clicker.onClick.AddListener(() => context.OnDateTimeItemClicked(this, index, type));
        }

        public void Recover()
        {
            backgroundImage.color = Color.white;
            text.fontStyle = FontStyle.Normal;
            var v = 102 / 255.0f;
            text.color = new Color(v, v, v);
        }

        public void Highlight()
        {
            var v = 250 / 255.0f;
            backgroundImage.color = new Color(v, v, v);
            v = 34 / 255.0f;
            text.fontStyle = FontStyle.Bold;
            text.color = new Color(v, v, v);
        }
    }

    public class TaptapAntiAddictionTimeSelectorController : BasePanelController
    {
        // 默认时间
        private int _defaultYear = 2000;
        private const int DefaultMonth = 6;

        private const int DefaultDay = 15;

        // 至今时间,最低年份
        private const int MinYear = 2022;

        public Button closeButton;
        public Button confirmButton;

        public Button backgroundButton;

        public Toggle yearToggle;
        public Toggle monthToggle;
        public Toggle dayToggle;

        public ScrollViewEx yearScrollView;
        public ScrollViewEx monthScrollView;
        public ScrollViewEx dayScrollView;

        public Text yearHolder;
        public Text monthHolder;
        public Text dayHolder;

        public Text yearText;
        public Text monthText;
        public Text dayText;

        private GameObject _activeDropList;

        public Text titleText;
        public Text descriptionText;
        public Text buttonText;

        public DateTimeItem selectedYear;
        public DateTimeItem selectedMonth;
        public DateTimeItem selectedDay;

        internal Action<VerificationResult> OnVerified;
        internal Action OnException;
        internal Action OnClosed;

        private int Year { get; set; }
        private int Month { get; set; }
        private int Day { get; set; }

        private int _minYear;

        private bool _isSending;

        private bool IsSending
        {
            get => _isSending;
            set
            {
                if (value != _isSending)
                {
                    _isSending = value;
                    if (_isSending)
                        UIManager.Instance.OpenLoading();
                    else
                        UIManager.Instance.CloseLoading();
                }
            }
        }

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            closeButton = gameObject.transform.Find("Root/CloseButton").GetComponent<Button>();
            confirmButton = gameObject.transform.Find("Root/ConfirmButton").GetComponent<Button>();

            backgroundButton = gameObject.transform.Find("Root").GetComponent<Button>();

            titleText = gameObject.transform.Find("Root/Title").GetComponent<Text>();
            descriptionText = gameObject.transform.Find("Root/Description").GetComponent<Text>();
            buttonText = gameObject.transform.Find("Root/ConfirmButton/Text").GetComponent<Text>();

            yearToggle = gameObject.transform.Find("Root/YearToggle").GetComponent<Toggle>();
            monthToggle = gameObject.transform.Find("Root/MonthToggle").GetComponent<Toggle>();
            dayToggle = gameObject.transform.Find("Root/DayToggle").GetComponent<Toggle>();

            yearScrollView = gameObject.transform.Find("Root/YearScrollView").GetComponent<ScrollViewEx>();
            monthScrollView = gameObject.transform.Find("Root/MonthScrollView").GetComponent<ScrollViewEx>();
            dayScrollView = gameObject.transform.Find("Root/DayScrollView").GetComponent<ScrollViewEx>();

            yearHolder = yearToggle.transform.Find("HolderLabel").GetComponent<Text>();
            monthHolder = monthToggle.transform.Find("HolderLabel").GetComponent<Text>();
            dayHolder = dayToggle.transform.Find("HolderLabel").GetComponent<Text>();

            yearText = yearToggle.transform.Find("Label").GetComponent<Text>();
            monthText = monthToggle.transform.Find("Label").GetComponent<Text>();
            dayText = dayToggle.transform.Find("Label").GetComponent<Text>();
            yearText.gameObject.SetActive(false);
            monthText.gameObject.SetActive(false);
            dayText.gameObject.SetActive(false);

            yearScrollView.gameObject.SetActive(false);
            monthScrollView.gameObject.SetActive(false);
            dayScrollView.gameObject.SetActive(false);

            yearHolder.gameObject.SetActive(true);
            monthHolder.gameObject.SetActive(true);
            dayHolder.gameObject.SetActive(true);
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            backgroundButton.onClick.AddListener(OnClickBackgroundButton);

            yearToggle.isOn = false;
            monthToggle.isOn = false;
            dayToggle.isOn = false;

            //日期选择器默认选中时间：2000 年 6 月 15 日 日期可选范围：前 100 年 - 至今（非未来日期）
            var nowYear = DateTime.Now.Year;
            nowYear = Math.Max(nowYear, MinYear);
            _defaultYear = nowYear - 22;
            _minYear = nowYear - 100;
            int yearCount = nowYear - _minYear + 1;

            yearScrollView.SetItemCountFunc(() => yearCount);
            yearScrollView.SetUpdateFunc(UpdateYearInfo);
            yearScrollView.UpdateData(false);

            monthScrollView.SetItemCountFunc(() => 12);
            monthScrollView.SetUpdateFunc(UpdateMonthInfo);
            monthScrollView.UpdateData(false);

            dayScrollView.SetUpdateFunc(UpdateDayInfo);

            yearToggle.onValueChanged.AddListener((b) => OnTimeToggleChange(b, 0));
            monthToggle.onValueChanged.AddListener((b) => OnTimeToggleChange(b, 1));
            dayToggle.onValueChanged.AddListener((b) => OnTimeToggleChange(b, 2));

            var config = Config.Current?.UIConfig?.InputRealNameInfoVietnam;
            if (config != null)
            {
                titleText.text = config.title;
                descriptionText.text = config.description;
                buttonText.text = config.button;
            }

            yearHolder.text = _defaultYear.ToString();
            monthHolder.text = DefaultMonth.ToString("D2");
            dayHolder.text = DefaultDay.ToString("D2");
            Year = _defaultYear;
            Month = DefaultMonth;
            Day = DefaultDay;

            OnChangeYearOrMonth(_defaultYear, DefaultMonth);

            IsSending = false;
        }

        private DateTimeItem CreateDateItem(RectTransform item)
        {
            item.gameObject.SetActive(true);
            DateTimeItem dateItem = item.GetComponent<DateTimeItem>();
            if (dateItem == null)
            {
                dateItem = item.gameObject.AddComponent<DateTimeItem>();
            }

            dateItem.Init();

            return dateItem;
        }

        private void UpdateTimeInfo(int index, RectTransform item, int type, string info)
        {
            DateTimeItem dateItem = CreateDateItem(item);

            dateItem.context = this;
            dateItem.type = type;
            dateItem.index = index;
            var text = dateItem.text;
            text.gameObject.SetActive(true);
            text.text = info;
            int intV = int.Parse(info);
            int comV = 0;
            if (type == 0)
            {
                comV = Year;
            }
            else if (type == 1)
            {
                comV = Month;
            }
            else if (type == 2)
            {
                comV = Day;
            }

            if (comV == intV)
            {
                dateItem.Highlight();
            }
            else
            {
                dateItem.Recover();
            }
        }

        private void UpdateYearInfo(int index, RectTransform item)
        {
            UpdateTimeInfo(index, item, 0, $"{(_minYear + index).ToString()}");
        }

        private void UpdateMonthInfo(int index, RectTransform item)
        {
            UpdateTimeInfo(index, item, 1, $"{(index + 1).ToString("D2")}");
        }

        private void UpdateDayInfo(int index, RectTransform item)
        {
            UpdateTimeInfo(index, item, 2, $"{(index + 1).ToString("D2")}");
        }

        private void OnClickBackgroundButton()
        {
            OnTimeToggleChange(false, 0);
            OnTimeToggleChange(false, 1);
            OnTimeToggleChange(false, 2);
            yearToggle.isOn = false;
            monthToggle.isOn = false;
            dayToggle.isOn = false;
        }

        public void OnDateTimeItemClicked(DateTimeItem item, int index, int type)
        {
            if (type == 0)
            {
                UpdateCurYear(_minYear + index);
                yearHolder.gameObject.SetActive(false);
                if (selectedYear != null)
                    selectedYear.Recover();
                item.Highlight();
                selectedYear = item;
            }
            else if (type == 1)
            {
                UpdateCurMonth(1 + index);
                monthHolder.gameObject.SetActive(false);
                if (selectedMonth != null)
                    selectedMonth.Recover();
                item.Highlight();
                selectedMonth = item;
            }
            else if (type == 2)
            {
                UpdateCurDay(1 + index);
                dayHolder.gameObject.SetActive(false);
                if (selectedDay != null)
                    selectedDay.Recover();
                item.Highlight();
                selectedDay = item;
            }
        }

        private void UpdateCurYear(int newYear)
        {
            Year = newYear;
            var timeText = $"{Year.ToString()}";
            yearText.text = timeText;
            yearText.gameObject.SetActive(true);
            yearText.text = timeText;
            yearToggle.isOn = false;
            OnChangeYearOrMonth(Year, Month);
        }

        private void UpdateCurMonth(int newMonth)
        {
            Month = newMonth;
            var timeText = $"{Month.ToString("D2")}";
            monthText.text = timeText;
            monthText.gameObject.SetActive(true);
            monthText.text = timeText;
            monthToggle.isOn = false;
            OnChangeYearOrMonth(Year, Month);
        }

        private void UpdateCurDay(int newDay)
        {
            Day = newDay;
            var timeText = $"{Day.ToString("D2")}";
            dayText.text = timeText;
            dayText.gameObject.SetActive(true);
            dayText.text = timeText;
            dayToggle.isOn = false;
        }

        private void OnChangeYearOrMonth(int year, int month)
        {
            int newDayCount = Tool.GetMonthDayCount(year, month);
            dayScrollView.SetItemCountFunc(() => newDayCount);
            dayScrollView.UpdateData();
            if (Day > newDayCount)
            {
                UpdateCurDay(newDayCount);
            }
        }

        /// <summary>
        /// 切换具体时间(年/月/日)详情是否显示
        /// </summary>
        /// <param name="toggle">是否显示</param>
        /// <param name="type">0-year;1-month;2-day</param>
        private void OnTimeToggleChange(bool toggle, int type)
        {
            if (_activeDropList != null)
                _activeDropList.SetActive(false);
            if (toggle)
            {
                ScrollViewEx target = null;
                int index = 0;
                switch (type)
                {
                    case 0:
                        if (_activeDropList != null)
                        {
                            monthToggle.isOn = false;
                            dayToggle.isOn = false;
                        }

                        target = yearScrollView;
                        index = Mathf.Max(0, Year - _minYear - 1);
                        break;
                    case 1:
                        if (_activeDropList != null)
                        {
                            yearToggle.isOn = false;
                            dayToggle.isOn = false;
                        }

                        target = monthScrollView;
                        index = Mathf.Max(0, Month - 2);
                        break;
                    case 2:
                        if (_activeDropList != null)
                        {
                            yearToggle.isOn = false;
                            monthToggle.isOn = false;
                        }

                        target = dayScrollView;
                        index = Mathf.Max(0, Day - 2);
                        break;
                }

                _activeDropList = target.gameObject;
                _activeDropList.SetActive(true);
                target.ScrollTo(index);
            }
            else
            {
                _activeDropList = null;
            }
        }

        private bool Validate(out DateTime time)
        {
            try
            {
                time = new DateTime(Year, Month, Day);
                return true;
            }
            catch (Exception)
            {
                time = default;
                return false;
            }
        }

        private async void OnConfirmButtonClicked()
        {
            if (IsSending) return;
            if (Validate(out DateTime dateTime))
            {
                Debug.LogFormat("Time: {0}", dateTime.ToString("yyyy/MM/dd"));
                var dateJson = new { birthDate = dateTime.ToString("yyyy-MM-dd") };
                string json = JsonConvert.SerializeObject(dateJson, Formatting.Indented);
                try
                {
                    IsSending = true;
                    var verificationResult = await Verification.VerifyKycAsync(TapTapAntiAddictionManager.UserId, json);
                    IsSending = false;
                    Close();
                    OnVerified?.Invoke(verificationResult);
                }
                catch
                {
                    IsSending = false;
                    OnException?.Invoke();
                }
            }
            else
            {
                await UIManager.Instance.OpenToastAsync(Config.Current.UIConfig.InputRealNameInfoVietnam.invalidateMessage);
            }
        }

        private void OnCloseButtonClicked()
        {
            Close();
            OnClosed?.Invoke();
        }
    }
}