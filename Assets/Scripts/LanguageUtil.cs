using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class LanguageUtil : Singleton<LanguageUtil> {
    public static string _currentLanguage; 
    private static Dictionary<string, LanguageEntry> languageDictionary = new Dictionary<string, LanguageEntry>();
    public static UnityEvent OnChangeLanguage = new UnityEvent();
    public static string[] languages = { "English", "中文", "日本語"};
    public static int languageIndex = 0;

    /// <summary>
    /// 多语言初始化
    /// </summary>
    public static void InitLanguage() {
        if (languageDictionary.Count > 0) {
            return;
        }
        ReadLanguageCSV("LanguageConfig", out string content, out string[] lines);

        if (lines != null && lines.Length > 0) {
            languageDictionary.Clear();
            string[][] LanguageConfigStr = new string[lines.Length - 1][];
            for (int i = 0; i < lines.Length; i++) {
                if (i > 0) {
                    string[] lineStr = lines[i].Split(",");
                    LanguageConfigStr[i - 1] = new string[lineStr.Length];
                    if (lineStr.Length > 0) {
                        LanguageEntry entry = new LanguageEntry {
                            Key = lineStr[0],
                            English = lineStr[1],
                            Chinese = lineStr[2],
                            Japanese = lineStr[3]
                        };
                        languageDictionary[entry.Key] = entry;
                    }
                }
            }
        }
        
        _currentLanguage = languages[languageIndex];
    }

    private static void ReadLanguageCSV(string fileName, out string content, out string[] lines) {
        string filePath = Path.Combine("Assets/Configs/LanguageConfig.csv");
        filePath = Path.GetFullPath(filePath); // 将路径转换为绝对路径

        // 检查文件是否存在
        if (!File.Exists(filePath)) {
            content = string.Empty;
            lines = default;
            return; // 文件不存在，直接返回
        }

        try {
            using (StreamReader sr = new StreamReader(filePath)) {
                string str = null;
                string line;
                while ((line = sr.ReadLine()) != null) {
                    str += line + '\n';
                }

                content = str.TrimEnd('\n');
                lines = content.Split('\n');
            }
        } catch {
            content = string.Empty;
            lines = default;
            Debug.LogError($"错误 {filePath} 配置读取错误，需要将外部 Excel 软件关闭，请检查!");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
    
    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="currentLanguage"></param>
    public static void CheckNextLanguage() {
        languageIndex = (languageIndex + 1) % languages.Length;
        string currentLanguage = languages[languageIndex];
        string language = PlayerPrefs.GetString("Language");
        if (string.IsNullOrEmpty(language) || currentLanguage != language) {
            PlayerPrefs.SetString("Language", currentLanguage);
            PlayerPrefs.Save();
            _currentLanguage = currentLanguage;
            OnChangeLanguage.Invoke();
        }
    }

    /// <summary>
    /// 获取预言
    /// </summary>
    /// <param name="key"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static string GetText(string key) {
        InitLanguage();

        if (languageDictionary.TryGetValue(key, out LanguageEntry entry)) {
            switch (_currentLanguage) {
                case "English":
                    return entry.English;
                case "中文":
                    return entry.Chinese;
                case "日本語" :
                    return entry.Japanese;
                default:
                    return entry.Chinese; // 默认返回英文
            }
        }
        return $"<{key} not found>";
    }
}

[Serializable]
public class LanguageEntry {
    public string Key;
    public string English;
    public string Chinese;
    public string Japanese;
}