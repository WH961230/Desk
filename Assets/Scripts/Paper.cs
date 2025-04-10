using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using SFB; // SimpleFileBrowser插件

public class Paper : MonoBehaviour {
    public Image targetImage; // 需要设置Sprite的Image组件
    public Button uploadButton;
    public Slider slider;
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