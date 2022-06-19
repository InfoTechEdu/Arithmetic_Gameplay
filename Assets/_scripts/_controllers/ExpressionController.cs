using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using SimpleJSON;
using System.Linq;
using UnityEngine.Networking;

public class ExpressionController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private AsteroidSpawner asteroidSpawner;

    private DataController dataController;

    private List<Expression> exps = new List<Expression>(); //#message список ариф. выражений

    bool settingsWasLoaded = false;
    public bool SettingsWasLoaded { get => settingsWasLoaded; set => settingsWasLoaded = value; }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }

    //#message генерация нового арифметического выражения
    private Expression getNextExp()
    {
        Expression exp = exps[0];

        if (exps.Count > 1)
            exps.RemoveAt(0);

        if (exps.Count == 1)
            updateExpressionsList();

        return exp;
    }

    //#message Генерация выражения по заданному правилу
    private Expression getExpByRuleUpdated(Rule rule)
    {
        Expression result = null;
        switch (rule.Sign)
        {
            case '+':
                result = generatePlusExpression(rule);
                break;
            case '-':
                result = generateMinusExpression(rule);
                break;
            case '*':
                result = generateMultiplyExpression(rule);
                break;
            case '/':
                result = generateDivideExpression(rule);
                break;
            default:
                break;
        }

        Debug.Log($"Expression by rule {rule} generated. Data: {result?.ToString("pretty")}");

        return result;
    }
    private Expression generatePlusExpression(Rule ruleData)
    {
        int a = Random.Range(ruleData.MinTermA, ruleData.MaxTermA + 1);
        int b = Random.Range(ruleData.MinTermB, ruleData.MaxTermB + 1);
        return new Expression(a, b, calculateAnswer(a, b, ruleData.Sign), ruleData.Sign, ruleData.IsAnswerNegative);
    }
    private Expression generateMinusExpression(Rule ruleData)
    {
        int a;
        int b;

        if (ruleData.IsAnswerNegative)
        {
            a = Random.Range(ruleData.MinTermA, ruleData.MaxTermA + 1);
            b = Random.Range(a + 1, ruleData.MaxTermB);


            //int iterations = 0;
            //do
            //{


            //    if (ruleData.MaxTermA == ruleData.MaxTermB && a == ruleData.MaxTermA)
            //    {
            //        a = Random.Range(ruleData.MinTermA, ruleData.MaxTermA + 1);
            //    }


            //    b = Random.Range(a, ruleData.MaxTermB + 1);

            //    iterations++;
            //    if (iterations > 500)
            //    {
            //        Debug.LogError("Stuck while loop. break");
            //        stuck = true;
            //        break;
            //    }
            //} while (b <= a);
        }
        else
        {
            a = Random.Range(ruleData.MinTermB, ruleData.MaxTermA + 1);
            b = Random.Range(ruleData.MinTermB, a + 1);
        }

        return new Expression(a, b, calculateAnswer(a, b, ruleData.Sign), ruleData.Sign, ruleData.IsAnswerNegative);
    }
    private Expression generateMultiplyExpression(Rule ruleData) => generatePlusExpression(ruleData);
    private Expression generateDivideExpression(Rule ruleData)
    {
        int a = Random.Range(ruleData.MinTermB, ruleData.MaxTermA + 1);
        int b = Random.Range(ruleData.MinTermB, a + 1);

        if (a % 2 != 0 && b % 2 == 0) //а - нечетное, в - четное
        {
            //Debug.Log($"[debug] А- нечетное, В - четное  [a = {a}, b = {b}]");
            a--;
        }
        if (a % 2 == 0 && b % 2 != 0) //а - четное, в - нечетное
        {
            //Debug.Log($"[debug] А- четное, В - нечетное  [a = {a}, b = {b}]");
            b--;
        }
        if (a % 2 != 0 && b % 2 != 0 && calculateAnswer(a, b, '/') % 10 != 0) //а - нечетное, в - нечетное и ответ не целый (например, 7 / 3)
        {
            //Debug.Log($"[debug] А- нечетное, В - нечетное  [a = {a}, b = {b}]");
            a = b * calculateAnswer(a, b, '/');
            //Debug.Log($"[debug] Updated [a = {a}, b = {b}]");
        }
        if (a % 2 == 0 && b % 2 == 0 && calculateAnswer(a, b, '/') % 10 != 0) //а - нечетное, в - нечетное и ответ не целый (например, 8 / 6)
        {
            a = b * calculateAnswer(a, b, '/');
        }

        return new Expression(a, b, calculateAnswer(a, b, ruleData.Sign), ruleData.Sign, ruleData.IsAnswerNegative);
    }
    private int calculateAnswer(int a, int b, char sign)
    {
        int result;
        switch (sign)
        {
            case '+':
                result = a + b;
                break;
            case '-':
                result = a - b;
                break;
            case '*':
                result = a * b;
                break;
            case '/':
                result = a / b;
                break;
            default:
                result = 0;
                break;
        }

        return result;
    }


    //Создает астероид с выражением
    public void newExpAsteroid() //refactor rename
    {
        float speed = dataController.levels[dataController.GameLevelName].Speed;
        asteroidSpawner.spawnExpAsteroid(this, getNextExp(), speed);
    }

    //Обновляет список астероидов с заданиями
    public void updateExpressionsList() //refactor, update algorithm. Иногда формируется список из 2х элементов, что не очень удобно
    {
        exps.Clear();

        foreach (var rName in dataController.levels[dataController.GameLevelName].Rules)
        {
            Rule r = dataController.rules[rName];
            exps.Add(getExpByRuleUpdated(r));
        }

        //Shuffle list
        Utils.CollectionUtils.ShuffleList<Expression>(exps);

        var pseudoRnd = new System.Random();
        var result = exps.OrderBy(item => pseudoRnd.Next());

        //1. Сформировать список упражнений в обычном порядке
        //2. сделать массив индексов. Перемешать их
        //3. После выдачи нового задания, обновляеть его
        //4. Когда задания в списке закончатся, перемешать список индексов. И вновь брать с листа по ним. Нужно два листа?
    }

    //Вызывается при выборе ответа пользователем
    public void answered(Transform asteroid, bool IsAnswerCorrect)
    {
        if (IsAnswerCorrect)
        {
            gameController.successAnswer(asteroid, dataController.levels[dataController.GameLevelName].Points);

            if (dataController.answersToNextLevel > 0)
                dataController.answersToNextLevel--;
            if (dataController.answersToNextLevel == 0)
                levelUp();

        }
        else
        {
            //#message "Блокировка" астероида после ответа пользователя (чтобы юзер не ответил 2 раза)
            asteroid.GetComponent<Asteroid>().@lock();

            if (dataController.answersToNextLevel < dataController.maxAnswersInLevel)
                dataController.answersToNextLevel++;
            if (dataController.answersToNextLevel == dataController.maxAnswersInLevel)
                levelDown();
        }
    }

    private void levelUp()
    {
        Debug.Log("Level Up! Updating settings");
        if (dataController.GameLevelIndex != dataController.levels.Count - 1) //level is not last
        {
            dataController.GameLevelIndex += 1;
            gameController.onLevelUpdated(true);


            Debug.LogWarning("Game level is " + dataController.GameLevelName);
        }
    }

    private void levelDown()
    {
        Debug.Log("Level Down! Updating settings");
        if (dataController.GameLevelIndex != 0)
        {
            Debug.Log("Level down. Updating settings");
            dataController.GameLevelIndex -= 1;
            gameController.onLevelUpdated(false);
        }
    }
}

