using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expression : MonoBehaviour
{
    int termA;
    int termB;
    int correctAnswer;
    char sign;

    int[] allAnswers;

    expType type;

    string expString;

    bool isAnswerNegative;

    public Expression(int a, int b, int answer, char sign, bool isAnswerNegative) //refactor Как-то криво написано
    {
        termA = a;
        termB = b;

        this.sign = sign;

        this.isAnswerNegative = isAnswerNegative;

        //setting expession string
        type = (expType)Random.Range(1, 2);
        if (type == expType.NoTerm)
        {
            this.correctAnswer = termB;
            termB = answer;

            //update algorithm. Можно сделать, чтобы скрывался либо A либо B
            expString = string.Format("{0} {1} {2} = {3}", termA, sign, " ", answer);

        }
        if(type == expType.NoAnswer)
        {
            this.correctAnswer = answer;
            //expString = string.Format("{0} {1} {2} = {3}", termA, sign, termB, " "); //old
            expString = string.Format("{0} {1} {2}", termA, sign, termB);
        }

        allAnswers = generateAnswerVariants();

        //Debug.Log("Exp string = " + ExpString);

    }

    public int TermA { get => termA; set => termA = value; }
    public int TermB { get => termB; set => termB = value; }
    public expType Type { get => type; set => type = value; }
    public int CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }
    public string ExpString { get => expString; set => expString = value; }
    public int[] AllAnswers { get => allAnswers; set => allAnswers = value; }

    //Override methods
    public override string ToString()
    {
        return string.Format("Expression: a = {0}, b = {1}, sign = {2}, expType = {3}",
            termA, termB, sign, type);
    }
    public string ToString(string format)
    {
        if (format == "pretty")
            return $"{termA} {sign} {termB} = {correctAnswer}";

        return this.ToString();
    }


    private int getPseudoRandomWithExclusion(int min, int max, int[] exclusionNumbers)
    {
        Debug.LogWarning("*** Starting generate answers ***");

        int result = 0;
        bool tryAgain = false;
        do
        {
            tryAgain = false;
            result = Random.Range(min, max + 1);
            Debug.LogWarning(string.Format("---- min = {0}, max = {1}, result = {2} ----", min, max, result));

            foreach (int exNum in exclusionNumbers)
                if (result == exNum)
                    tryAgain = true;
        } while (tryAgain);

        Debug.LogWarning("*** Ending generate answers ***");

        return result;
    }
    private int[] generateAnswerVariants()
    {

        //Граница для выбора ответов будет либо [0 ... correct] либо [correct - 5, correct]
        //int min = !isAnswerNegative ? 0 : Mathf.Clamp(correctAnswer - 5, 0, correctAnswer); //не совсем понял, почему так написал
        int min = isAnswerNegative ? correctAnswer - 5 : 0;
        int max = correctAnswer + 5;

        int[] answers = new int[3];
        answers[0] = getPseudoRandomWithExclusion(min, max,  new int[] { correctAnswer });
        answers[1] = getPseudoRandomWithExclusion(min, max, new int[] { correctAnswer, answers[0] });
        answers[2] = correctAnswer;

        Utils.CollectionUtils.ShuffleArray<int>(answers);
        return answers;

        //Old Not Working
        //refactor?
        /*
        int wrongA = 0;
        int wrongB = 0;
        do
        {
            
            wrongA = Random.Range(correctAnswer, correctAnswer + 5);
        } while (wrongA == correctAnswer);

        do
        {
            if (!isAnswerNegative)
            {
                wrongB = (int) Mathf.Clamp(Random.Range(correctAnswer - 5, correctAnswer), 0, correctAnswer); 
            }
            else
            {
                wrongB = Random.Range(correctAnswer - 5, correctAnswer);
            }
                
        } while (wrongB == correctAnswer);


        int[] answers = { wrongA, wrongB, correctAnswer };
        Utils.CollectionUtils.ShuffleArray<int>(answers);

        return answers;
        */
    }
}

public enum expType
{
    NoAnswer = 1,
    NoTerm = 2
}
