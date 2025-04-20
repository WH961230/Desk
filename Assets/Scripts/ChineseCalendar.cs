using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Lunar;
using Lunar.EightChar;
using TMPro;

public class ChineseCalendar : MonoBehaviour {
    public TMP_InputField birthdayZiInput;
    private Dictionary<string, ZiColorData> ziColorDics;
    public List<ZiColorData> ziColorDatas;
    public GameObject ziCalendarParent;
    public GameObject yunCalendarParent;
    public List<GameObject> calendarTipParent;
    public List<Data> calendarData;
    public static ChineseCalendar Instance;
    private DateTime inputDateTime;
    private void Awake() {
        Instance = this;
    }

    public void Start() {
        ziColorDics = new Dictionary<string, ZiColorData>();
        foreach (var tmpData in ziColorDatas) {
            ziColorDics.Add(tmpData.Zi, tmpData);
        }

        RefreshCalendar();
        birthdayZiInput.onValueChanged.AddListener(AddBirthday);
    }

    private void AddBirthday(string input) {
        if (IsValidDateTimeString(input)) {
            int year = int.Parse(input.Substring(0, 4));   // 2025
            int month = int.Parse(input.Substring(4, 2));     // 04
            int day = int.Parse(input.Substring(6, 2));       // 20
            int hour = int.Parse(input.Substring(8, 2));      // 13
            int minute = int.Parse(input.Substring(10, 2));  // 23
            inputDateTime = new DateTime(year, month, day, hour, minute, 0);
            birthdayZiInput.interactable = false;
            RefreshCalendar();
        } else {
            inputDateTime = default;
            
        }
    }

    public bool IsValidDateTimeString(string input) {
        string pattern = @"^\d{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])([01][0-9]|2[0-3])[0-5][0-9]$";
        return Regex.IsMatch(input, pattern);
    }

    private void RefreshCalendar() {
        Solar solar = new Solar(inputDateTime.Year, inputDateTime.Month, inputDateTime.Day, inputDateTime.Hour, inputDateTime.Minute);
        Lunar.Lunar lunar = solar.Lunar;
        EightChar d = lunar.EightChar;
        
        Set(LunarType.八字年天干, d.Year);
        Set(LunarType.八字年地支, d.Year);
        Set(LunarType.八字月天干, d.Month);
        Set(LunarType.八字月地支, d.Month);
        Set(LunarType.八字日天干, d.Day);
        Set(LunarType.八字日地支, d.Day);
        Set(LunarType.八字时天干, d.Time);
        Set(LunarType.八字时地支, d.Time);

        Solar solarNow = new Solar(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
        Lunar.Lunar lunarNow = solarNow.Lunar;
        EightChar now = lunarNow.EightChar;

        Yun yun = d.GetYun(1);
        int currentYear = DateTime.Now.Year; // 2025
        DaYun[] daYunArr = yun.GetDaYun();
        string ganZhi = "";
        for (int i = 0, j = daYunArr.Length; i < j; i++) {
            DaYun daYun = daYunArr[i];
            if (currentYear >= daYun.StartYear &&
                (i == daYunArr.Length - 1 || currentYear < daYunArr[i + 1].StartYear)) {
                ganZhi = daYun.GanZhi;
                break;
            }
        }

        Set(LunarType.运大运天干, ganZhi);
        Set(LunarType.运大运地支, ganZhi);
        Set(LunarType.运年天干, now.Year);
        Set(LunarType.运年地支, now.Year);
        Set(LunarType.运月天干, now.Month);
        Set(LunarType.运月地支, now.Month);
        Set(LunarType.运日天干, now.Day);
        Set(LunarType.运日地支, now.Day);
        Set(LunarType.运时天干, now.Time);
        Set(LunarType.运时地支, now.Time);
    }

    private void Set(LunarType type, string str) {
        foreach (var VARIABLE in calendarData) {
            if (VARIABLE.Key == type) {
                Char[] c = str.ToCharArray();
                int index = 0;
                if (type.ToString().Contains("天干")) {
                    index = 0;
                } else if (type.ToString().Contains("地支")) {
                    index = 1;
                }

                string zi = c[index].ToString();
                VARIABLE.Value.text = zi;
                if (ziColorDics.TryGetValue(zi, out ZiColorData data)) {
                    VARIABLE.Value.color = data.ZiColor;
                }
                break;
            }
        }
    }

    //刷新八字
    public void ActiveZi(bool b) {
        ziCalendarParent.SetActive(b);

        if (b) {
            if (inputDateTime == default) {
                ziCalendarParent.SetActive(false);
            }
        }

        birthdayZiInput.interactable = !ziCalendarParent.activeSelf;
    }

    //刷新大运流年
    public void ActiveYun(bool b) {
        yunCalendarParent.SetActive(b);
    }

    //刷新提示
    public void ActiveTip(bool b) {
        foreach (var tmp in calendarTipParent) {
            tmp.SetActive(b);
        }
    }
}

[Serializable]
public class Data {
    public LunarType Key;
    public TextMeshProUGUI Value;
}

[Serializable]
public class ZiColorData {
    public string Zi;
    public Color ZiColor;
}

[Serializable]
public enum LunarType {
    八字年天干,
    八字年地支,
    八字月天干,
    八字月地支,
    八字日天干,
    八字日地支,
    八字时天干,
    八字时地支,
    运大运天干,
    运大运地支,
    运年天干,
    运年地支,
    运月天干,
    运月地支,
    运日天干,
    运日地支,
    运时天干,
    运时地支
}