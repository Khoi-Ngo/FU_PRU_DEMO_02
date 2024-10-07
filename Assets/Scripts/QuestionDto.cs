using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class QuestionDto
{
    public string QuestionTxt { get; private set; }
    public Dictionary<char, string> AnswerChoices { get; private set; }
    public char CorrectAnswer { get; private set; }
    public int difficultLevel { get; private set; }// Basing on the game difficult level the user choose

    public QuestionDto(string questionTxt, Dictionary<char, string> answerChoices, char correctAnswer)
    {
        QuestionTxt = questionTxt;
        AnswerChoices = answerChoices;
        CorrectAnswer = correctAnswer;
    }
}
