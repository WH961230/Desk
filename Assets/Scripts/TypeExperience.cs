using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRawInput;

public class TypeExperience : MonoBehaviour {
    public Slider Exp;
    public TextMeshProUGUI ExpValue;
    public static TypeExperience Instance;

    private void Awake() {
        Instance = this;
        ExpValue.text = string.Concat(Exp.value, "/", Exp.maxValue);
    }

    public void OnKeyDown(RawKey obj) {
        bool isKeyCodePress = GlobalKeySound.IsKeyCodePress(obj);
        if (isKeyCodePress) {
            Exp.value += 1;
            ExpValue.text = string.Concat(Exp.value, "/", Exp.maxValue);
        }
    }
}