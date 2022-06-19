
public class Rule
{
    public Rule(string name, int minTermA, int maxTermA, int minTermB, int maxTermB, char sign, bool isAnswerNegative)
    {
        this.name = name;

        this.minTermA = minTermA;
        this.maxTermA = maxTermA;

        this.minTermB = minTermB;
        this.maxTermB = maxTermB;

        this.sign = sign;
        this.isAnswerNegative = isAnswerNegative;
    }

    string name;

    int minTermA;
    int maxTermA;

    int minTermB;
    int maxTermB;

    char sign;
    bool isAnswerNegative; //true если ответ должен выйти отрицательным

    public string Name { get => name; set => name = value; }
    public int MinTermA { get => minTermA; set => minTermA = value; }
    public int MaxTermA { get => maxTermA; set => maxTermA = value; }
    public int MinTermB { get => minTermB; set => minTermB = value; }
    public int MaxTermB { get => maxTermB; set => maxTermB = value; }
    public char Sign { get => sign; set => sign = value; }
    public bool IsAnswerNegative { get => isAnswerNegative; set => isAnswerNegative = value; }

    public override string ToString()
    {
        return "[rule - " + name + "]";
    }

    public string ToString(string format)
    {
        if (format.Equals("full"))
            return string.Format("Rule: min_max a = {0}_{1}, min_max b = {2}_{3}, sign = {4}, negativeAnswer = {5}, rule name = {6}",
                minTermA, maxTermA, minTermB, maxTermB, sign, isAnswerNegative, name);
        return null;
    }
}
