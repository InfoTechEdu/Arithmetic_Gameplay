using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public enum GameMode
{
    PC = 1,
    TABLET = 2
}

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField]
    private ViewController viewController;

    [SerializeField]
    private ExpressionController expController;

    [Header("GameObjects")]
    [SerializeField]
    private SpaceShip spaceShip;

    private DataController dataController;

    int points = 0; //Текущее количество баллов (без учета времени)
    
    float generationDelay = 0; //Задержка до создания следующего астероида
    float lastGeneratedAt = 0;

    bool gameRunning = false;
    float firstAsteroidDelay = 3f;
    bool gameStopped = false;

    float playedTime;

    bool newHighscore = false;


    public int Points { get => points; set => points = value; }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        viewController.showStartScreen();
        viewController.onGameLoaded();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!gameRunning)
            return;

        if(dataController.GameMode == GameMode.TABLET)
        {
            if (Input.GetKeyDown(KeyCode.A))
                viewController.onPrevTargetButtonClicked();
            if (Input.GetKeyDown(KeyCode.D))
                viewController.onNextTargetButtonClicked();
        }

        if(Time.time - lastGeneratedAt >= generationDelay) //#message Генерация нового астероида через промежуток времени
        {
            expController.newExpAsteroid();
            lastGeneratedAt = Time.time;
        }

        playedTime += Time.deltaTime;
        viewController.updateTimerValue(playedTime);
    }

    void OnApplicationQuit()
    {
        dataController.UpdateUserProgress(gameRunning);
    }

    //Когда пользователь выбрал верный ответ, корабль выстреливает ракетой, и обновляются очки
    public void successAnswer(Transform asteroid, int answerPoints)
    {
        //edit здесь выстрел ракеты, сохранение очков и тд
        points += answerPoints;

        spaceShip.fire(asteroid);
        viewController.updatePointsText();
    }

    //Вызывается, при старте новый игры
    public void startGame()
    {
        InitData();

        viewController.showGameScreen();
        viewController.updatePointsText();
        viewController.updateDamageView(spaceShip.MaxDamage, 0);

        spaceShip.activate();

        StartCoroutine(runGameProcess(firstAsteroidDelay));

        string gameMode = dataController.GameMode == GameMode.PC ? "PC" : "TABLET";
    }
    private IEnumerator runGameProcess(float delay)
    {
        //instructionController.InitData(dataController.instructionData, dataController.GetUserId());

        //if (!instructionController.InstructionPassed)
        if(false)
        {
            yield return new WaitForSeconds(5);
            viewController.hideGameScreen();
            //instructionController.ShowInstructionPanel();
            spaceShip.destroy();

            //StartCoroutine(SomeCoroutine());
            //yield return new WaitUntil(() => instructionController.InstructionPassed);

            viewController.showGameScreen();
            viewController.updatePointsText();
            viewController.updateDamageView(spaceShip.MaxDamage, 0);

            spaceShip.activate();
        }
        else
        {
            if (delay != 0)
                yield return new WaitForSeconds(delay);
        }
        //danger. Зачем добавил?
        //if (gameStopped)
        //    yield break;

        gameRunning = true;
    }
    //Проигрыш
    public void gameOver()
    {
        gameRunning = false;

        spaceShip.destroy();

        destroyAllAsteroids();

        saveProgressDataLocal();
        dataController.UpdateUserProgress(gameRunning);

        viewController.showRestartScreen(newHighscore, points, (int)playedTime);

        dataController.sendGameOverEventToGameAnalytics(points);

        //TestUILog.UPDATE_HIGHSCORE_TEXT(HIGHSCORE);

        //edit show animation and other
    }
    //Перезапуск игры
    public void restartGame()
    {
        points = 0;
        newHighscore = false;

        spaceShip.activate();
        InitData();
        
        playedTime = 0;

        StartCoroutine(runGameProcess(firstAsteroidDelay));

        dataController.sendGameRestartedEventToGameAnalytics();
    }
    //Преждевременный выход пользователя из игры
    public void onGameExit()
    {
        gameRunning = false;
        gameStopped = true;

        spaceShip.destroy();

        destroyAllAsteroids();
    }

    public void onLoggedOut()
    {
        FirebaseManager.Instance.Auth.LogOut();
        SceneManager.LoadScene("AuthScene");
    }

    [Obsolete]
    private void saveProgressDataLocal() //local
    {
        if(points != 0)
            points += (int) playedTime;

        if(points > dataController.Highscore)
        {
            dataController.OnNewHighscore(points);
            newHighscore = true;
        }
    }
    [Obsolete]
    public IEnumerator loadSettings(int SelectedClass) //edit to online
    {
        string fileName = string.Format("levels_{0}_settings.json", SelectedClass);
        string settingsPath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (settingsPath.Contains("://") || settingsPath.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(settingsPath))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR loading progress data");
                    // handle failure
                }
                else
                {
                    try
                    {
                        // entire file is returned via downloadHandler
                        //string fileContents = request.downloadHandler.text;
                        // or
                        //byte[] fileContents = request.downloadHandler.data;

                        string settings = request.downloadHandler.text;
                        //expController.parseLevelsSettings(settings); //old code
                        viewController.onSettingsLoaded();
                        Debug.Log("settings loaded. text - " + settings);
                        // do whatever you need to do with the file contents
                        //if (loadAsset(fileContents))
                        //    isAssetRead = true;
                    }
                    catch (System.Exception x)
                    {
                        // handle failure
                    }
                }
            }
        }
        else
        {
            string settings = File.ReadAllText(settingsPath);
            //expController.parseLevelsSettings(settings); //old code
            viewController.onSettingsLoaded();
            Debug.Log("settings loaded. text - " + settings);
        }

       
    }

    //Устаревший код. Получение данных с локальных файлов
    ////Получение уровня, очков и т.д. в файл или сервер
    /*IEnumerator loadProgressData()
    {
        string progressPath = System.IO.Path.Combine(Application.streamingAssetsPath, "user_progress.json");
        string progress = null;

        if (progressPath.Contains("://") || progressPath.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(progressPath))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR loading progress data");
                    // handle failure
                }
                else
                {
                    try
                    {
                        // entire file is returned via downloadHandler
                        //string fileContents = request.downloadHandler.text;
                        // or
                        //byte[] fileContents = request.downloadHandler.data;

                        progress = request.downloadHandler.text;
                        JSONNode progressJSON = JSONNode.Parse(progress);
                        GAME_LEVEL = progressJSON["level"];
                        HIGHSCORE = progressJSON["highscore"];

                        // do whatever you need to do with the file contents
                        //if (loadAsset(fileContents))
                        //    isAssetRead = true;
                    }
                    catch (System.Exception x)
                    {
                        // handle failure
                    }
                }
            }
        }
        else
        {
            if (!System.IO.File.Exists(progressPath))
                yield break;
            progress = System.IO.File.ReadAllText(progressPath);

            JSONNode progressJSON = JSONNode.Parse(progress);
            GAME_LEVEL = progressJSON["level"];
            HIGHSCORE = progressJSON["highscore"];
        }


    }

    //Получение данных о лучших пользователях
    IEnumerator loadLeaderboardData()
    {
        string leaderboardPath = System.IO.Path.Combine(Application.streamingAssetsPath, "leaderboard.json");
        string leaderboard = null;

        if (leaderboardPath.Contains("://") || leaderboardPath.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(leaderboardPath))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR loading progress data");
                    // handle failure
                }
                else
                {
                    try
                    {
                        // entire file is returned via downloadHandler
                        //string fileContents = request.downloadHandler.text;
                        // or
                        //byte[] fileContents = request.downloadHandler.data;

                        leaderboard = request.downloadHandler.text;
                        JSONNode leaderboardJSON = JSONNode.Parse(leaderboard);
                        List<UserProgressData> users = new List<UserProgressData>();
                        foreach (var userNode in leaderboardJSON)
                        {
                            users.Add(new UserProgressData(userNode.Value["name"], userNode.Value["highscore"]));
                        }

                        viewController.updateLeaderboardPanel(users);
                        // do whatever you need to do with the file contents
                        //if (loadAsset(fileContents))
                        //    isAssetRead = true;
                    }
                    catch (System.Exception x)
                    {
                        // handle failure
                    }
                }
            }
        }
        else
        {
            leaderboard = System.IO.File.ReadAllText(leaderboardPath);
            JSONNode leaderboardJSON = JSONNode.Parse(leaderboard);
            List<UserProgressData> users = new List<UserProgressData>();
            foreach (var userNode in leaderboardJSON)
            {
                users.Add(new UserProgressData(userNode.Value["name"], userNode.Value["highscore"]));
            }

            viewController.updateLeaderboardPanel(users);
        }

        ///




    }

    //Получение данных настроек
    IEnumerator loadSettingsData()
    {
        string progressPath = System.IO.Path.Combine(Application.streamingAssetsPath, "user_settings.json");
        string settings = null;

        if (progressPath.Contains("://") || progressPath.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(progressPath))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR loading progress data");
                    // handle failure
                }
                else
                {
                    try
                    {
                        // entire file is returned via downloadHandler
                        //string fileContents = request.downloadHandler.text;
                        // or
                        //byte[] fileContents = request.downloadHandler.data;

                        settings = request.downloadHandler.text;
                        JSONNode settingsJSON = JSONNode.Parse(settings);
                        GAME_VOLUME = settingsJSON["game_volume"];
                        GAME_MODE = (GameMode)settingsJSON["game_mode"].AsInt;

                        viewController.setVolumeValue(GAME_VOLUME);
                        viewController.setToggleValue(GAME_MODE);

                        // do whatever you need to do with the file contents
                        //if (loadAsset(fileContents))
                        //    isAssetRead = true;
                    }
                    catch (System.Exception x)
                    {
                        // handle failure
                    }
                }
            }
        }
        else
        {
            if (!System.IO.File.Exists(progressPath))
                yield break;
            settings = System.IO.File.ReadAllText(progressPath);

            JSONNode progressJSON = JSONNode.Parse(settings);
            JSONNode settingsJSON = JSONNode.Parse(settings);
            GAME_VOLUME = settingsJSON["game_volume"];
            GAME_MODE = (GameMode)settingsJSON["game_mode"].AsInt;

            viewController.setVolumeValue(GAME_VOLUME);
            viewController.setToggleValue(GAME_MODE);
        }


    }*/


    //Обновление конфигураций. Урон, уровень и т.д.
    private void InitData()
    {
        //edit
        expController.updateExpressionsList();
        generationDelay = dataController.levels[dataController.GameLevelName].Delay;
        lastGeneratedAt = 0;
        points = 0;
        newHighscore = false;
        playedTime = 0;
    }

    public void onLevelUpdated(bool isLevelUp)
    {
        expController.updateExpressionsList();
        generationDelay = dataController.levels[dataController.GameLevelName].Delay; //expController.getCurrentLevelDelay();
        dataController.answersToNextLevel = dataController.levels[dataController.GameLevelName].MaxAnswersCount;

        if (isLevelUp)
            viewController.ShowLevelUpMessage();
        //lastGeneratedAt = 0;
    }

    public void onDamaged()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, AnimController.CAMERA_SHAKE_DURATION);
        viewController.updateDamageView(spaceShip.MaxDamage, spaceShip.Damage);
    }

    private void destroyAllAsteroids()
    {
        foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag(GameTags.ASTEROID))
        {
            asteroid.GetComponent<Asteroid>().destroy(null, false);
        }
    }

    
} 
