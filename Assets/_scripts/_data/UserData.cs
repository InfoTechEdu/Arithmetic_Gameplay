using FullSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class UserData
{
    [fsProperty] public string id;
    [fsProperty] public string status;
    [fsProperty] public string userClass;

    [fsProperty] public UserProgressData progressData;
    [fsProperty] public StatisticsData statistics;
    [fsProperty] public SettingsData userSettings;

    [fsProperty] private Dictionary<string, string> sessions = new Dictionary<string, string>();

    [fsProperty] private Sprite profilePhotoSprite;
    [fsProperty] private string profilePhoto;

    public UserProgressData ProgressData { get => progressData;}
    public SettingsData GameSettings { get => userSettings;  }
    public string Id { get => id; set => id = value; }
    public string UserClass { get => userClass; }
    public string UserClassName { get => "class" + userClass; }
    public string ProfilePhotoUrl { get => profilePhoto; }
    public Sprite ProfilePhoto { get => profilePhotoSprite; }
    public string Name { get => progressData.Name; }
    public string Surname { get => progressData.Surname; }
    public UserData(string id)
    {
        this.id = id;
    }

    public UserData()
    {
    }

    public void setId(string id)
    {
        this.id = id;
    }
    public void setStatus(string status)
    {
        this.status = status;
    }
    public void setUserClass(string _class)
    {
        this.userClass = _class;
    }
    public void setProfilePhotoUrl(string url)
    {
        this.profilePhoto = url;
    }
    public void setProfilePhotoSprite(Sprite photo)
    {
        this.profilePhotoSprite = photo;
    }
    public void setSessions(Dictionary<string, string> sessions)
    {
        this.sessions = sessions;
    }
    public void setStatistics(StatisticsData statistics)
    {
        this.statistics = statistics;
    }
    public void setSettingsData(SettingsData settings)
    {
        this.userSettings = settings;
    }
    public void setProgressData(UserProgressData data)
    {
        this.progressData = data;
    }
    public void updatePublicData(UserProgressData progressData, StatisticsData statistics)
    {
        this.progressData = progressData;
        this.statistics = statistics;
    }

    public void updateProfilePhoto(Sprite newPhoto)
    {
        profilePhotoSprite = newPhoto;
    }

    public void setGameLevel(int newLevel)
    {
        progressData.level = newLevel;
    }
    public void setHighscore(int newHighscore)
    {
        progressData.highscore = newHighscore;
    }

    public void CopyFrom(UserData data)
    {
        setId(data.id);
        setStatus(data.status);
        setUserClass(data.userClass);
        setSessions(data.sessions);
        setProfilePhotoUrl(data.profilePhoto);
        updatePublicData(data.progressData, data.statistics);
        updateProfilePhoto(data.profilePhotoSprite);
    }

    private IEnumerator GetSpriteFromURL(string url)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            this.profilePhotoSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public override string ToString()
    {
        return string.Format("UserData : [id = {0}, progressData = {1}, gameSettings = {2}]",
            id, progressData, userSettings);
    }
}
