using NonsensicalKit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleQuestions : ListManagerBase<QuestionArgs,AnswerElement,AnswerArgs>
{
    [SerializeField] Text txt_Question;
    [SerializeField] InputField ipf_Question;

    protected override void UpdateUI()
    {
        base.UpdateUI();

        txt_Question.text = listData.QuestionText;
        ipf_Question.text = listData.QuestionText;
    }

    private void OnEditEnd(string text)
    {
        if (text!=listData.QuestionText)
        {
            listData.QuestionText = text;
            UpdateUI();
        }
    }
}
