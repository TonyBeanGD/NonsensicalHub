using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
[RequireComponent(typeof(RectTransform))]
public class InputAutoSize : MonoBehaviour {
    [SerializeField] private Text text;
    [SerializeField] private float min=100;
    private InputField inputField;
    private RectTransform rectTransform;
    private TextGenerator textGen;

    private void Awake()
    {
        inputField = GetComponent<InputField>();
        rectTransform = GetComponent<RectTransform>();
        textGen = new TextGenerator();
        inputField.onValueChanged.AddListener((value) =>
        {
            //替换空格编码格式
            inputField.text = inputField.text.Replace(" ", no_breaking_space);

            text.text = inputField.text;
        });
    }

    private void Update()
    {
        //TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
        //float height = textGen.GetPreferredHeight(inputField.text, generationSettings);
        //Debug.Log(height);
        //if (height<min)
        //{
        //    height = min;
        //}
        //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,height+10);
    }

    private static readonly string no_breaking_space = "\u00A0";
    
}
