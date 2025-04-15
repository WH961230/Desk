using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 多语言组件
/// </summary>
public class LanguageTool : MonoBehaviour {
    private string _key;
    private TextMeshProUGUI TargetText;
    private TMP_FontAsset ChineseFont;
    private TMP_FontAsset JapaneseFont;

    public void Awake() {
        LanguageUtil.InitLanguage();
        TargetText = GetComponent<TextMeshProUGUI>();
        _key = TargetText.text;
        ChineseFont = Resources.Load<TMP_FontAsset>("Alimama_ShuHeiTi_Bold SDF");
        JapaneseFont = Resources.Load<TMP_FontAsset>("Jap SDF");
        LanguageUtil.OnChangeLanguage.AddListener(RefreshLanguageText);
        RefreshLanguageText();
    }

    public void SetLanguageKey(string key) {
        _key = key;
        RefreshLanguageText();
    }

    private void Update() {
        RefreshLanguageText();
    }

    private void RefreshLanguageText() {
        TargetText.text = LanguageUtil.GetText(_key);
        switch (LanguageUtil.languageIndex) {
            case 0:
            case 1:
                TargetText.font = ChineseFont;
                break;
            case 2:
                TargetText.font = JapaneseFont;
                break;
        }
    }
}