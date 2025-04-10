using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityRawInput;
using Random = UnityEngine.Random;

public class DropWhenType : MonoBehaviour {
    public Transform DropPos;
    public Transform DropParent;
    public float DropAlpha;
    public int DropMax;
    public List<DropData> Datas;
    public static DropWhenType Instance;
    private List<GameObject> Instances = new List<GameObject>();
    private Dictionary<string, RawKey> Dic = new Dictionary<string, RawKey>();
    private bool Open;

    private void Awake() {
        Instance = this;

        Dic.Add("A", RawKey.A);
        Dic.Add("B", RawKey.B);
        Dic.Add("C", RawKey.C);
        Dic.Add("D", RawKey.D);
        Dic.Add("E", RawKey.E);
        Dic.Add("F", RawKey.F);
        Dic.Add("G", RawKey.G);
        Dic.Add("H", RawKey.H);
        Dic.Add("I", RawKey.I);
        Dic.Add("J", RawKey.J);
        Dic.Add("K", RawKey.K);
        Dic.Add("L", RawKey.L);
        Dic.Add("M", RawKey.M);
        Dic.Add("N", RawKey.N);
        Dic.Add("O", RawKey.O);
        Dic.Add("P", RawKey.P);
        Dic.Add("Q", RawKey.Q);
        Dic.Add("R", RawKey.R);
        Dic.Add("S", RawKey.S);
        Dic.Add("T", RawKey.T);
        Dic.Add("W", RawKey.W);
        Dic.Add("V", RawKey.V);
        Dic.Add("U", RawKey.U);
        Dic.Add("X", RawKey.X);
        Dic.Add("Y", RawKey.Y);
        Dic.Add("Z", RawKey.Z);

        foreach (var VARIABLE in Datas) {
            if (Dic.TryGetValue(VARIABLE.Key, out RawKey key)) {
                VARIABLE.Raw = key;
            }
        }
    }

    public void OpenFunction(bool value) {
        Open = value;
    }

    public void DropItemInstance(RawKey key) {
        if (!Open) {
            return;
        }
        foreach (var VARIABLE in Datas) {
            if (VARIABLE.Raw == key) {
                GameObject instance = Instantiate(VARIABLE.DropItemTemplate, DropParent);
                instance.transform.position = DropPos.position + new Vector3(Random.Range(-800f, 800f), Random.Range(0f, 1000f), 0);
                instance.GetComponentInChildren<TextMeshProUGUI>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), DropAlpha);
                instance.GetComponent<Rigidbody2D>().gravityScale = Random.Range(10, 100);
                instance.transform.localScale = Vector3.one * Random.Range(1, 5);
                instance.SetActive(true);
                Instances.Add(instance);
            }
        }

        if (Instances.Count >= DropMax) {
            foreach (var tmp in Instances) {
                Destroy(tmp);
            }
            Instances.Clear();
        }
    }
}

[Serializable]
public class DropData {
    public string Key;
    public RawKey Raw;
    public GameObject DropItemTemplate;
}
