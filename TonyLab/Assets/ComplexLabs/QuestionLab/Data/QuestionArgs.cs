using NonsensicalKit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionArgs : DataObject<AnswerArgs>
{
    public string QuestionText;

    public int CorrectIndex;
}
