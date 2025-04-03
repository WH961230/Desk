using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalBGM : MonoBehaviour {
    public static GlobalBGM Instance;

    private void Awake() {
        Instance = this;
    }

    public List<SoundData> BGMSound; // 绑定的按键音效文件
    public TMP_Dropdown _dropdown;
    private AudioSource audioSource;

    void Start() {
        // 初始化音频组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        RefreshDropDown();
        OnBGMStartPlay(0);
        _dropdown.onValueChanged.AddListener(OnBGMStartPlay);
    }

    private void RefreshDropDown() {
        _dropdown.ClearOptions();
        _dropdown.AddOptions(BGMSound.ConvertAll(x => x.sign));
    }

    private void OnBGMStartPlay(int value) {
        AudioClip clip = null;
        foreach (var VARIABLE in BGMSound) {
            if (VARIABLE.sign == _dropdown.options[value].text) {
                clip = VARIABLE.audioClip;
            }
        }
        if (BGMSound != null) {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void OnDestroy() {
        audioSource.Stop();
    }
}