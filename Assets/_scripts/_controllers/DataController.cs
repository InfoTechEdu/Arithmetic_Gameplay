using Proyecto26;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    public UnityEngine.UI.Slider progressBar;

    private string userId;

    private UserData userData;

    private LeaderboardData leaderboardData;

    //refactor. set modificators to private
    public Dictionary<string, LevelData> levels = new Dictionary<string, LevelData>();
    public Dictionary<string, Rule> rules = new Dictionary<string, Rule>();

    //refactor. set modificators to private
    public int answersToNextLevel; //Количество правильных ответов до следующего уровня 
    public int maxAnswersInLevel; //Максимальное количество правильных ответов в уровне

    private int progress;

    public static FirebaseCustomYield firebaseCustomYield;

    public int GameLevelIndex { get => userData.ProgressData.Level; set => userData.setGameLevel(value); }
    public GameMode GameMode { get => (GameMode)userData.GameSettings.gameMode; set => userData.GameSettings.gameMode = (int) value; }
    public int GameVolume { get => userData.GameSettings.gameVolume; set => userData.GameSettings.gameVolume = (int) value; }

    public string GameLevelName { get => "level" + userData.ProgressData.Level; }
    public int Highscore { get => userData.ProgressData.Highscore; }
    public LeaderboardData LeaderboardData { get => leaderboardData; set => leaderboardData = value; }
    public UserData[] Leaders { get => leaderboardData.AllUsers; }


    #region MonoBehaviour
    private void Start()
    {
        Debug.Log("Starting download data...");
        DontDestroyOnLoad(gameObject);

        userId = FirebaseManager.Instance.Auth.UserId;

        GetPlatformUserData(OnPlatformUserDataWasGet);
    }
    #endregion

    private void GetPlatformUserData(Action<PlatformUserData> onGet)
    {
        FirebaseManager.Instance.Database.GetObject<PlatformUserData>(FirebaseProjectConfigurations.PLATFORM_DATABASE_ROOT_PATH, $"allUsers/{userId}", onGet, (exception) =>
        {
            Debug.LogError("Error getting platform user data. Exception - " + exception.Message);
        });
    }
    private void OnPlatformUserDataWasGet(PlatformUserData pud)
    {
        Debug.Log("Platform data was get. Data: " + pud);

        if (!IsUserExistInGame(pud))
        {
            Dictionary<string, string> args = new Dictionary<string, string>() { { "game", "arithmetic" } };
            //#disabled 
            FirebaseManager.Instance.Functions.CallCloudFunctionPostObject<PlatformUserData>("CreateNewUserGameData", pud, args, (statusCode) =>
            {
                Debug.Log("User successfully created in game Arithmetic");

                //PushUserToOnlineREST();
                //RemoveFromSearchers();

                StartCoroutine(LoadAllData());
            }, (exception) =>
            {
                Debug.LogError("Error while calling CreateNewUser in game Arithmetic cloud function. Message - " + exception.Message);
            });
        }
        else
        {
            //PushUserToOnlineREST();
            //RemoveFromSearchers();

            StartCoroutine(LoadAllData());
        }
    }
    private bool IsUserExistInGame(PlatformUserData pud)
    {
        return pud.Games != null && pud.Games.ContainsKey("arithmetic");
    }

    #region Data loading
    public IEnumerator LoadAllData()
    {
        userData = null;
        leaderboardData = null;

        firebaseCustomYield = new FirebaseCustomYield();

        //Getting Leaderboard Data
        firebaseCustomYield.onRequestStarted();
        LoadUserData();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadLeaderboardData();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadUserSettings();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadGameData();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        //firebaseCustomYield.onRequestStarted();
        //LoadOnboardingSprites();
        //yield return firebaseCustomYield;
        //UpdateProgressBar();


        //firebaseCustomYield.onRequestStarted();
        //LoadNotificationsREST();
        //yield return firebaseCustomYield;
        //UpdateProgressBar();

        //firebaseCustomYield.onRequestStarted();
        //LoadFriendsListREST();
        //yield return firebaseCustomYield;
        //UpdateProgressBar();

        //old?
        //firebaseCustomYield.onRequestStarted();
        //LoadUserSessionsList();
        //yield return firebaseCustomYield;
        //UpdateProgressBar();

        Debug.LogWarning("All UserData was loaded. Result - " + userData);

        OnAllDataLoaded();
        //edit. add loading settings

        

        //LoadUserData();
        //yield return new WaitUntil(() => currentUser != null);

        //LoadLeaderboardData();
        //yield return new WaitUntil(() => leaderboardData != null);

        //LoadClassSettings();
        //yield return new WaitUntil(() => rules.Count > 0);

        //onAllDataLoaded();


    }
    private void OnAllDataLoaded()
    {
        Debug.Log("All data was loaded");
       // StartCoroutine(instructionData.LoadInstruction(userId));

        SceneManager.LoadScene("Main");
    }
    //Получение уровня, очков и т.д. в файл или сервер
    public void LoadUserData()
    {

        FirebaseManager.Instance.Database.GetObject<UserData>($"users/{userId}/public", (data) =>
        {
            userData = data;
            userData.setId(userId);

            StartCoroutine(GetSpriteFromURL(userData.ProfilePhotoUrl, (sprite) =>
            {
                userData.setProfilePhotoSprite(sprite);
            }));

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading user data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                $"Exception while downloading user data. Message - {exception.Message}");
        });

        //old
        //RestClient.Get<UserData>(databaseUrl + "/users/" + userId + ".json?auth=" + idToken).Then(response =>
        //{
        //    Debug.Log("User data downloaded. User data - " + response);
        //}).Catch((exception)=>
        //{
        //    Debug.Log("Exception while getting user data. exception - " + exception);
        //});
    }
    public void LoadLeaderboardData()
    {
        FirebaseManager.Instance.Database.GetObject<LeaderboardData>($"leaderboard/allUsers", (data) =>
        {
            leaderboardData = data;

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading leaderboard data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading leaderboard data. Message - " + exception.Message);
        });

        //Debug.Log("downloading leaderboard data");
        //RestClient.GetArray<LeaderboardData>($"{databaseUrl}/leaderboard/class{userData.UserClass}.json?auth={idToken}", onLeaderboardDataLoaded);
    }
    //refactor. Remove parsers and parse by full serializer
    public void LoadUserSettings()
    {
        FirebaseManager.Instance.Database.GetObject<SettingsData>($"users/{userData.Id}/private/userSettings", (data) =>
        {
            if(data != null)
                userData.setSettingsData(data);

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading leaderboard data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading leaderboard data. Message - " + exception.Message);
        });
        //RestClient.GetArray<string>($"gameData/{userData.UserClass}.json?auth={idToken}", onGameDataLoaded);
    }
    //refactor
    public void LoadGameData()
    {
        CheckIsUserClassExistInDB((exist)=>
        {
            string downloadingDataClass = exist ? userData.UserClass : "common";
            LoadGameDataByClass(downloadingDataClass);
        });
    }
    private void CheckIsUserClassExistInDB(Action<bool> onChecked)
    {
        //check is user class levels exists in database
        FirebaseManager.Instance.Database.GetShallowTest($"gameData", (classes) =>
        {
            if (classes.ContainsKey(userData.UserClass))
                onChecked(true);
            else
                onChecked(false);
        },(exception) =>
        {
            Debug.LogError($"Exception while downloading game data (shallow). Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error, $"Exception while downloading game data (shallow). Message - {exception.Message}");
        });
    }
    private void LoadGameDataByClass(string className)
    {
        FirebaseManager.Instance.Database.GetJson($"gameData/{className}", (data) =>
        {
            if (data != null)
            {
                parseGameData(data);
                Debug.LogWarning("[debug] Game data parsed. Levels - " + Utils.CollectionUtils.DictionaryToString(levels));
                Debug.LogWarning("[debug] Game data parsed. Rules - " + Utils.CollectionUtils.DictionaryToString(rules));
            }
            else
            {
                LoadCommonClassGameData(); //edit. delete?
            }

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading game data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error, $"Exception while downloading game data. Message - {exception.Message}");
        });
    }
    private void onGameDataLoaded(RequestException exception, ResponseHelper responseHelper, string[] notUsed)
    {
        if (exception != null)
        {
            Debug.LogError("Exception while downloading game data. Message - " + exception.Message + ", response - " + exception.Response);
            return;
        }


        Debug.Log("donwloaded game data = " + responseHelper.Text);
        parseGameData(responseHelper.Text);
        firebaseCustomYield.onRequestEnd();

        Debug.LogWarning("[debug] Game data parsed. Levels - " + Utils.CollectionUtils.DictionaryToString(levels));
        Debug.LogWarning("[debug] Game data parsed. Rules - " + Utils.CollectionUtils.DictionaryToString(rules));
    }
    public void LoadCommonClassGameData()
    {

    }
    #endregion

    private void parseGameData(string jsonData)
    {
        Debug.Log("Parsing level settings...");

        JSONNode settingsJSON = JSON.Parse(jsonData);
        parseLevels(settingsJSON["levels"].AsObject);
        parseRules(settingsJSON["rules"].AsObject);

        Debug.LogWarning("Settings  = " + jsonData);
        Debug.LogWarning("SettingsJSON  = " + settingsJSON.ToString());
        Debug.LogWarning("SettingsJSON[levels]  = " + settingsJSON["levels"].ToString());
        Debug.LogWarning("GAME LEVEL  = " + GameLevelName);
        Debug.LogWarning("SettingsJSON[levels][gamelevel]  = " + settingsJSON["levels"]["2"].ToString());


        maxAnswersInLevel = settingsJSON["levels"][GameLevelName]["max_answers_count"]; //refactor Думаю лучше создать класс Level
        answersToNextLevel = maxAnswersInLevel;

        Debug.Log("Levels settings parsed");

        //Testing
        //Debug.Log("Checking parsing");
        //foreach (var lvl in levels)
        //{
        //    Debug.Log(string.Format("Level number {0}. Rules - {1}", lvl.Key, MyUtils.StringFormatter.ArrayToString(lvl.Value)));
        //}
        //foreach (var rule in rules)
        //{
        //    Debug.Log(string.Format("Level number {0}. Rules - {1}", rule.Key, rule.Value.ToString()));
        //}
    }
    private void parseLevels(JSONNode levelsJSON)
    {
        if (levels.Count != 0)
            levels.Clear();

        foreach (var lvlNode in levelsJSON)
        {
            //getting rules
            JSONNode rulesNode = lvlNode.Value["rules"];
            string[] levelsRules = new string[rulesNode.Count];
            int index = 0;
            foreach (var rule in rulesNode)
            {
                levelsRules[index++] = rule.Key;
            }

            //refactor. Update code to this (not working, error on FromJson)
            //LevelData lvlData = JsonUtility.FromJson<LevelData>(lvlNode.ToString());
            //lvlData.updateRules(levelsRules);
            //levels.Add(lvlNode.Key.ToString(), lvlData);

            //old code
            //string[] rulesToLevel = new string[rulesArr.Count];
            //for (int i = 0; i < rulesArr.Count; i++)
            //{
            //    rulesToLevel[i] = rulesArr[i];
            //}

            //getting other data. Почему-то AsFloat не сработал

            //old code
            int maxAnswers = lvlNode.Value["max_answers_count"];
            float speed = float.Parse(lvlNode.Value["speed"].Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            int points = lvlNode.Value["points"];
            float delay = float.Parse(lvlNode.Value["delay"].Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            LevelData lvlData = new LevelData(maxAnswers, speed, points, delay, levelsRules);
            levels.Add(lvlNode.Key.ToString(), lvlData);
        }
    }
    private void parseRules(JSONNode rulesJSON)
    {
        if (rules.Count != 0)
            rules.Clear();

        foreach (var node in rulesJSON)
        {
            string ruleName = node.Key.ToString();
            if (ruleName == "D2")
                Debug.Log("[debug D1 rule] next rule data. min_b=" + node.Value["min_b"] + "min_b=" + node.Value["max_b"]);

            Rule rule = new Rule(
                ruleName,
                node.Value["min_a"], node.Value["max_a"],
                node.Value["min_b"], node.Value["max_b"],
                Utils.StringFormatter.StringToChar(node.Value["sign"]), node.Value["negative_answer"].AsBool);


            rules.Add(ruleName, rule);
        }
    }
    
    public void DownloadTop10(Action<LeaderboardData> onLoaded)
    {
        Dictionary<string, object> args = new Dictionary<string, object>() { { "game", "arithmetic" }, { "className", userData.UserClass } };
        FirebaseManager.Instance.Functions.CallCloudFunction("DownloadTop10Leaderboard", args, (data) =>
        {
            DataParser.ParseLeaderboardData(data.body, out leaderboardData);
            leaderboardData.SortDescending();
            onLoaded(leaderboardData);
        }, (exception) =>
        {
            Debug.LogError("Error while downloading leaderboard data");
        });

    }

    public void UpdateUserSettings()
    {
        FirebaseManager.Instance.Database.PutObject($"users/{userData.Id}/private/userSettings", userData.GameSettings, () =>
        {
            Debug.Log("Success updating user settings");
        }, (exception) =>
        {
            Debug.LogError($"Exception while put user settings data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error, $"Exception while put user settings data. Message - {exception.Message}");
        });
    }
    //Обновление пользовательских данных (прогресс, настройки). upd 21.05.2021: настройки? тут же только прогресс
    public void UpdateUserProgress(bool isGameRunning)
    {
        if (isGameRunning) //аварийный выход 
            return;

        JSONNode jsonBody = new JSONObject();
        jsonBody[$"users/{userData.Id}/public/progressData/highscore"] = userData.ProgressData.Highscore;
        jsonBody[$"users/{userData.Id}/public/progressData/level"] = userData.ProgressData.Level;
        jsonBody[$"leaderboard/{userData.UserClass}/{userData.Id}/progressData/highscore"] = userData.ProgressData.Highscore;
        jsonBody[$"leaderboard/{userData.UserClass}/{userData.Id}/progressData/level"] = userData.ProgressData.Level;

        string jsonData = jsonBody.ToString();
        FirebaseManager.Instance.Database.PatchJson($"", jsonData, () =>
        {
            Debug.Log($"Success updating user progress data");
        }, (exception) =>
        {
            Debug.LogError($"Exception while patch user progress data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error, $"Exception while patch user progress data. Message - {exception.Message}");
        });
    }
    public void OnNewHighscore(int highscore)
    {
        userData.setHighscore(highscore);
    }
    public UserData GetUserData()
    {
        return userData;
    }
    public string GetUserId()
    {
        return userId;
    }


    #region GameAnalytics Events
    public void sendGameCanceledEventToGameAnalytics()
    {
        GameAnalyticsSDK.GameAnalytics.NewDesignEvent("userBehaviour:cancelGame");
    }
    public void sendGameOverEventToGameAnalytics(int points)
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, "endGame", "class" + userData.UserClass, points);
    }
    public void sendGameRestartedEventToGameAnalytics()
    {
        GameAnalyticsSDK.GameAnalytics.NewDesignEvent("userBehaviour:restartGame");
    }
    #endregion

    #region ProgressBar
    private void UpdateProgressBar()
    {
        if (progressBar == null)
            return;
        progress += UnityEngine.Random.Range(0, 100 - progress);
        progressBar.value = progress / 100f;
    }
    #endregion

    #region Helpers
    public IEnumerator GetSpriteFromURL(string url, Action<Sprite> callback)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError($"Error downloading texture from url {url}. Error - {www.error}");
            callback.Invoke(null);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite downloadedSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            callback.Invoke(downloadedSprite);
        }

    }
    private IEnumerator UpdateMultipleValues(string url, string jsonData, Action<bool> onUpdated)
    {
        byte[] formData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = UnityWebRequest.Put(url, formData);
        request.method = "PATCH";
        /* You may need to add header(s) */
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.responseCode != 200)
            onUpdated.Invoke(true);

        Debug.Log($"Success updating data. Response code - {request.responseCode}. Reponse - {request.downloadHandler.text}");
    }
    #endregion
}
