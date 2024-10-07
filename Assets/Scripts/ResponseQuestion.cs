using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ResponseQuestion
{
    public string QuestionText { get; private set; }
    public char AnswerChoice { get; private set; }

    public ResponseQuestion()
    {

    }
    public ResponseQuestion(string _questionText, char _answerChoice)
    {
        QuestionText = _questionText;
        AnswerChoice = _answerChoice;
    }

}