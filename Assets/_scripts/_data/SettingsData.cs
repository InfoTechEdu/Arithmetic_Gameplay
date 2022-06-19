
[System.Serializable]
public class SettingsData
{
    public int gameVolume;
    public int gameMode;

    public override string ToString()
    {
        return $"[gameVolume - {gameVolume}, gameMode - {gameMode}]";
    }
}
