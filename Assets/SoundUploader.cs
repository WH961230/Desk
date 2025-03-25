using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SFB; // 需要导入SimpleFileBrowser插件

public class SoundUploader : MonoBehaviour {
    public List<GlobalKeySound.KeySoundData> CustomerSoundDatas = new List<GlobalKeySound.KeySoundData>();
    public Button uploadButton;
    public Button quitButton;

    private string soundFolderPath;

    void Start() {
        soundFolderPath = Path.Combine(Application.persistentDataPath, "UserSounds");
        Directory.CreateDirectory(soundFolderPath);
        uploadButton.onClick.AddListener(UploadSound);
        quitButton.onClick.AddListener(QuitGame);
        StartCoroutine(LoadAllUserSounds());
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator LoadAllUserSounds() {
        var files = Directory.GetFiles(soundFolderPath)
            .Where(IsValidAudioFile)
            .ToArray();
        foreach (var file in files) {
            yield return StartCoroutine(LoadAndAddAudio(file));
        }
    }

    IEnumerator LoadAndAddAudio(string filePath) {
        using (WWW www = new WWW("file:///" + filePath)) {
            yield return www;

            if (string.IsNullOrEmpty(www.error)) {
                var clip = www.GetAudioClip();
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                // 避免重复添加
                if (!GlobalKeySound.Instance.keyPressSound.Exists(x => x.sign == fileName)) {
                    GlobalKeySound.Instance.keyPressSound.Add(new GlobalKeySound.KeySoundData {
                        audioClip = clip,
                        sign = fileName
                    });
                }
            } else {
                Debug.LogError($"初始化加载失败: {filePath} - {www.error}");
            }
        }

        // 更新下拉菜单
        GlobalKeySound.Instance.RefreshDropDown();
    }


    public void UploadSound() {
        // 使用跨平台文件浏览器
        var paths = StandaloneFileBrowser.OpenFilePanel("选择音频文件", "", new[] {
            new ExtensionFilter("音频文件", "wav", "mp3", "ogg")
        }, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0])) {
            StartCoroutine(HandleFileUpload(paths[0]));
        }
    }

    IEnumerator HandleFileUpload(string sourcePath) {
        string fileName = GenerateUniqueFileName(Path.GetFileName(sourcePath));
        string targetPath = Path.Combine(soundFolderPath, fileName);

        byte[] fileData;
        try {
            // 使用File.ReadAllBytes读取文件
            fileData = File.ReadAllBytes(sourcePath);
            File.WriteAllBytes(targetPath, fileData);
            // 验证加载
            StartCoroutine(LoadAudioFile(targetPath));
        } catch (System.Exception e) {
            yield break;
        }
    }

    // 其他辅助方法保持不变
    private string GenerateUniqueFileName(string originalName) {
        string baseName = Path.GetFileNameWithoutExtension(originalName);
        string extension = Path.GetExtension(originalName);
        int counter = 1;

        while (File.Exists(Path.Combine(soundFolderPath, originalName))) {
            originalName = $"{baseName}_{counter++}{extension}";
        }

        return originalName;
    }

    IEnumerator LoadAudioFile(string path) {
        using (WWW www = new WWW("file:///" + path)) {
            yield return www;
            if (string.IsNullOrEmpty(www.error)) {
                AudioClip clip = www.GetAudioClip();
                Debug.Log($"成功加载：{Path.GetFileNameWithoutExtension(path)}");
                GlobalKeySound.Instance.keyPressSound.Add(new GlobalKeySound.KeySoundData() {
                    sign = Path.GetFileNameWithoutExtension(path),
                    audioClip = clip,
                });
                GlobalKeySound.Instance.RefreshDropDown();
            } else {
                Debug.LogError($"加载失败：{www.error}");
            }
        }
    }

    private bool IsValidAudioFile(string path) {
        string ext = Path.GetExtension(path).ToLower();
        return ext == ".wav" || ext == ".mp3" || ext == ".ogg";
    }
}