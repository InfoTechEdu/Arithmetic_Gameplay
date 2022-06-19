
[System.Serializable]
public class LevelData
{
    public string[] rules;
    public int maxAnswersCount;
    public float speed;
    public int points;
    public float delay;

    public LevelData(int maxAnswersCount, float speed, int points, float delay, string[] rules)
    {
        this.maxAnswersCount = maxAnswersCount;
        this.speed = speed;
        this.points = points;
        this.delay = delay;
        this.rules = rules;
    }

    public void updateRules(string[] rules)
    {
        this.rules = rules;
    }

    public string[] Rules { get => rules; }
    public int MaxAnswersCount { get => maxAnswersCount; }
    public float Speed { get => speed; }
    public int Points { get => points; }
    public float Delay { get => delay; }

    public override string ToString()
    {
        return $"Level data : [speed - {speed}, points - {points}, max answers count - {maxAnswersCount}]";
    }
}
