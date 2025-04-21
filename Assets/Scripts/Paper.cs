using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using SFB;
using TMPro;

public class Paper : MonoBehaviour {
    public Image targetImage; // 需要设置Sprite的Image组件
    public ChineseCalendar targetLunar;
    public Button uploadButton;
    public Button quitButton;
    public Slider slider;
    public Slider calendarSlider;
    public Button switchLanguage;
    public Button baziButton;
    private int baziOpen = -1;//0 null 1 bazi 2 tip
    private string imageFolderPath;

    void Start() {
        // 初始化图片存储目录
        imageFolderPath = Path.Combine(Application.persistentDataPath, "UserImages");
        Directory.CreateDirectory(imageFolderPath);
        // 绑定按钮事件
        uploadButton.onClick.AddListener(UploadImage);
        // 加载已保存的图片（可选）
        StartCoroutine(LoadLatestUserImage());
        
        slider.onValueChanged.AddListener(OnValueChanged);
        
        calendarSlider.onValueChanged.AddListener(OnCalendarValueChanged);
        
        quitButton.onClick.AddListener(QuitGame);
        
        switchLanguage.onClick.AddListener(SwitchLanguage);

        Bazi();
        baziButton.onClick.AddListener(Bazi);
    }

    private void OnCalendarValueChanged(float arg0) {
        foreach (var tmp in targetLunar.calendarTipParent) {
            TextMeshProUGUI textComponent = tmp.GetComponent<TextMeshProUGUI>();
            Color c = textComponent.color;
            c.a = arg0;
            textComponent.color = c;
        }

        foreach (var tmp in targetLunar.calendarData) {
            Color c = tmp.Value.color;
            c.a = arg0;
            tmp.Value.color = c;
        }
    }

    private void Bazi() {
        baziOpen = (baziOpen + 1) % 3;
        if (baziOpen == 0) {
            ChineseCalendar.Instance.ActiveZi(false);
            ChineseCalendar.Instance.ActiveYun(false);
            ChineseCalendar.Instance.ActiveTip(false);
        } else if (baziOpen == 1) {
            ChineseCalendar.Instance.ActiveZi(true);
            ChineseCalendar.Instance.ActiveYun(true);
            ChineseCalendar.Instance.ActiveTip(false);
        } else if (baziOpen == 2) {
            ChineseCalendar.Instance.ActiveZi(true);
            ChineseCalendar.Instance.ActiveYun(true);
            ChineseCalendar.Instance.ActiveTip(true);
        }
    }

    private void SwitchLanguage() {
        LanguageUtil.CheckNextLanguage();
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnValueChanged(float arg0) {
        Color color = targetImage.color;
        color.a = arg0;
        targetImage.color = color;
    }

    // 上传图片
    public void UploadImage() {
        var extensions = new[] {
            new ExtensionFilter("图片文件", "png", "jpg", "jpeg")
        };

        // 打开文件选择对话框
        var paths = StandaloneFileBrowser.OpenFilePanel("选择图片", "", extensions, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0])) {
            StartCoroutine(HandleImageUpload(paths[0]));
        }
    }

    IEnumerator HandleImageUpload(string sourcePath) {
        // 生成唯一文件名并保存到本地
        string fileName = GenerateUniqueFileName(Path.GetFileName(sourcePath));
        string targetPath = Path.Combine(imageFolderPath, fileName);

        // 1. 保存图片到目标路径
        byte[] fileData = File.ReadAllBytes(sourcePath);
        File.WriteAllBytes(targetPath, fileData);
        Debug.Log($"图片保存成功: {targetPath}");
        // 2. 加载图片
        yield return StartCoroutine(LoadImageSprite(targetPath));
    }

    // 加载图片并设置为Sprite
    IEnumerator LoadImageSprite(string imagePath) {
        string fileUrl = "file:///" + imagePath;
        using (WWW www = new WWW(fileUrl)) {
            yield return www;

            if (string.IsNullOrEmpty(www.error)) {
                // 创建Texture2D并转换为Sprite
                Texture2D texture = new Texture2D(2, 2);
                www.LoadImageIntoTexture(texture);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f)
                );

                // 设置到Image组件
                if (targetImage != null) {
                    targetImage.gameObject.SetActive(true);
                    targetImage.sprite = sprite;
                    Debug.Log($"图片加载成功: {Path.GetFileName(imagePath)}");
                }
            } else {
                Debug.LogError($"图片加载失败: {www.error}");
            }
        }
    }

    // 加载最近一张用户图片（可选）
    IEnumerator LoadLatestUserImage() {
        var files = Directory.GetFiles(imageFolderPath)
            .Where(IsValidImageFile)
            .OrderByDescending(f => new FileInfo(f).LastWriteTime)
            .ToArray();

        if (files.Length > 0) {
            yield return StartCoroutine(LoadImageSprite(files[0]));
        }
    }

    // 生成唯一文件名
    private string GenerateUniqueFileName(string originalName) {
        string baseName = Path.GetFileNameWithoutExtension(originalName);
        string extension = Path.GetExtension(originalName);
        int counter = 1;

        while (File.Exists(Path.Combine(imageFolderPath, originalName))) {
            originalName = $"{baseName}_{counter++}{extension}";
        }

        return originalName;
    }

    // 验证是否为图片文件
    private bool IsValidImageFile(string path) {
        string ext = Path.GetExtension(path).ToLower();
        return ext == ".png" || ext == ".jpg" || ext == ".jpeg";
    }
}