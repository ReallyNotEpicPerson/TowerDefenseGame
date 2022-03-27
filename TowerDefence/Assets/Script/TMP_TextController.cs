using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMP_TextController : MonoBehaviour
{
    public TMP_Text txt;
    public Color32 faceColor;
    public Color32 outlineColor;

    private TMP_Text _text;

    void Start()
    {
        _text = txt.GetComponent<TMP_Text>();
    }
    public void Blushing()
    {
        txt.faceColor = faceColor;
        txt.outlineColor = outlineColor;
    }
    public void NormalColor(Color32 normalFaceColor, Color32 normalOutlineColor)
    {
        txt.faceColor = normalFaceColor;
        txt.outlineColor = normalOutlineColor;
    }
    public void SetText(string t) 
    {
        txt.text = t;
    }
}
