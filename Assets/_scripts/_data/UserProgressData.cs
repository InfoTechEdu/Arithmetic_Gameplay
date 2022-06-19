
[System.Serializable]
public class UserProgressData
{
    public string name;
    private string surname;
    public int highscore; 
    public int level;

    public string Name { get => name;}
    public int Highscore { get => highscore; }
    public int Level { get => level; }
    public string Surname { get => surname; }

    public override string ToString()
    {
        return $"[name - {name}, highscore - {highscore}, level - {level}]";
    }
}
