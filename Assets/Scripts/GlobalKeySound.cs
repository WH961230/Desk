using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityRawInput;

public class GlobalKeySound : MonoBehaviour {
    public static GlobalKeySound Instance;

    private void Awake() {
        Instance = this;
    }

    public List<SoundData> keyPressSound; // 绑定的按键音效文件
    public TMP_Dropdown _dropdown;
    private AudioSource audioSource;
    void Start() {
        // 初始化音频组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 初始化键盘监听
        RawInput.WorkInBackground = true; // 添加此行
        RawInput.InterceptMessages = false;
        if (!RawInput.Start()) {// true表示启用后台输入检测
            Debug.LogError("键盘钩子安装失败！请以管理员权限运行程序。");
            return;
        }
        RawInput.OnKeyDown += OnKeyPressed;
        RawInput.OnKeyDown += TypeExperience.Instance.OnKeyDown;
        Debug.Log("键盘监听已启动");

        RefreshDropDown();
    }

    public void RefreshDropDown() {
        _dropdown.ClearOptions();
        _dropdown.AddOptions(keyPressSound.ConvertAll(x => x.sign));
    }

    private void OnKeyPressed(RawKey key) {
        bool isKeyCode = IsKeyCodePress(key);
        AudioClip clip = null;
        foreach (var VARIABLE in keyPressSound) {
            if (VARIABLE.sign == _dropdown.options[_dropdown.value].text) {
                clip = VARIABLE.audioClip;
            }
        }
        if (keyPressSound != null && isKeyCode) {
            audioSource.PlayOneShot(clip);
        }
    }

    public static bool IsKeyCodePress(RawKey key) {
        switch (key) {
            case RawKey.Q:
            case RawKey.W:
            case RawKey.E:
            case RawKey.R:
            case RawKey.T:
            case RawKey.Y:
            case RawKey.U:
            case RawKey.I:
            case RawKey.O:
            case RawKey.P:
            case RawKey.A:
            case RawKey.S:
            case RawKey.D:
            case RawKey.F:
            case RawKey.G:
            case RawKey.H:
            case RawKey.J:
            case RawKey.K:
            case RawKey.L:
            case RawKey.Z:
            case RawKey.X:
            case RawKey.C:
            case RawKey.V:
            case RawKey.B:
            case RawKey.N:
            case RawKey.M:
            case RawKey.Space:
            case RawKey.Return:
            case RawKey.LeftControl:
            case RawKey.LeftWindows:
            case RawKey.LeftButtonAlt:
            case RawKey.LeftShift:
            case RawKey.Tab:
            case RawKey.CapsLock:
                return true;
        }

        return false;
    }

    void OnDestroy() {
        RawInput.Stop();
        RawInput.OnKeyDown -= OnKeyPressed;
    }
}

[Serializable]
public class SoundData {
    public string sign;
    public AudioClip audioClip;
}