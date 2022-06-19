using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ViewController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] GameController gameController;
    [SerializeField] AnimController animController;
    [SerializeField] TargetController targetController;

    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject selectClassPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject onboardingPanel;

    [Header("UI elements")]
    [SerializeField] private Text pointsText;
    [SerializeField] private Image damagePanel;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject newHighscoreImg;
    [SerializeField] private GameObject exitButton;

    [Header("UI prefabs")]
    [SerializeField] private GameObject userPrefab;

    private GameObject activeScreen;

    private AudioSource audioSource;

    DataController dataController;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dataController = FindObjectOfType<DataController>();
    }

    private void Start()
    {
        hideSettingsScreen();
        hideLeaderBoardScreen();
        hideExitPanel();
        hideRestartScreen();
        hideGameScreen();
        hideSelectClassPanel();

        /*showStartScreen();
        activeScreen = startPanel;*/
        
    }

    public void onGameLoaded()
    {
        animController.animateStartScreenIn(startPanel);
    }

    private void goHome()
    {
        animController.animateGoHome(()=>
        {
            hideGameScreen();
            hideRestartScreen();

            showStartScreen();
            initStartScreenButtons();
            initBackground();
            
            //animController.animateStartScreenIn(startPanel);

            animController.animateOnHomeReturned();
        });
    }
    private void initStartScreenButtons()
    {
        Button[] startScreenButtons = startPanel.GetComponentsInChildren<Button>();

        Color c = startScreenButtons[0].GetComponent<Image>().color;
        c.a = 1f;

        foreach (var btn in startScreenButtons)
            btn.GetComponent<Image>().color = c;

        startPanel.transform.localScale = Vector3.one;
    }
    private void initBackground()
    {
        //changing background //refactor
        SpriteRenderer backgroundSpriteRend = TransformUtils.FindDeepChild(Camera.main.transform, "MenuBackground").GetComponent<SpriteRenderer>();
        Color c = backgroundSpriteRend.color;
        c.a = 1f;
        backgroundSpriteRend.color = c;
    }

    public void showStartScreen()
    {
        if (activeScreen != null)
            activeScreen.SetActive(false);

        startPanel.SetActive(true);
        activeScreen = startPanel;
    }
    public void hideStartScreen()
    {
        startPanel.SetActive(false);
    }

    public void showLeaderboardScreen()
    {
        activeScreen.SetActive(false);
        leaderboardPanel.SetActive(true);

        activeScreen = leaderboardPanel;
    }
    public void hideLeaderBoardScreen()
    {
        leaderboardPanel.SetActive(false);
    }

    int points;
    int timePoints;
    public void showRestartScreen(bool isHighscoreUpdated, int totalPoints, int timePoints)
    {
        hideExitPanel(); 
        hideGameScreen();
        
        if(totalPoints == 0)
        {
            this.timePoints = 0;
            this.points = totalPoints;
        }
        else
        {
            this.timePoints = timePoints;
            this.points = totalPoints - timePoints;
        }

        //old code
        //this.timePoints = (totalPoints == 0) ? 0 : timePoints;
        //this.points = (timePoints == 0) ? totalPoints : (totalPoints - timePoints);

        Text scoreText = Utils.TransformUtils.FindDeepChild(restartPanel.transform, "ScoreValue").
            GetComponent<Text>();
        scoreText.text = totalPoints.ToString();

        Text timerCopy = TransformUtils.FindDeepChild(restartPanel.transform, "TimerCopyPanel")
            .GetComponentInChildren<Text>();
        timerCopy.text = timerText.text;
        timerCopy.transform.parent.gameObject.SetActive(true);

        Text highscoreText = Utils.TransformUtils.FindDeepChild(restartPanel.transform, "HighscoreValue").
            GetComponent<Text>();
        highscoreText.text = dataController.Highscore.ToString();
          
        if (isHighscoreUpdated)
            newHighscoreImg.SetActive(true);

        restartPanel.SetActive(true);
        animController.animateRestartScreen(restartPanel, onRestartScreenShown);

        activeScreen = restartPanel;
    }
    public void onRestartScreenShown()
    {
        animController.animateTimePoints(restartPanel, gamePanel, points, timePoints);
    }

    public void hideRestartScreen()
    {
        restartPanel.SetActive(false);
        newHighscoreImg.SetActive(false);
    }

    public void showSettingsScreen()
    {
        activeScreen.SetActive(false);

        settingsPanel.SetActive(true);
        updateSettingsScreen();

        activeScreen = settingsPanel;
    }

    public void hideSettingsScreen()
    {
        settingsPanel.SetActive(false);
    }
    public void updateSettingsScreen()
    {
        setToggleValue(dataController.GameMode);
        setVolumeValue(dataController.GameVolume);

        UserData currentUser = dataController.GetUserData();

        UserProfileInfoView userProfileInfoView = settingsPanel.transform.FindDeepChild("UserProfileInfoView").GetComponent<UserProfileInfoView>();
        userProfileInfoView.LoadInfo(currentUser.ProfilePhoto, currentUser.Name, currentUser.Surname, 0);
    }

    public void showGameScreen()
    {
        activeScreen.SetActive(false);

        gamePanel.SetActive(true);

        updateTimerValue(0f);
        updatePointsText();
        updateDamageView(3, 0); //edit danger code refactor
        updateVariantButtons();

        if(dataController.GameMode == GameMode.PC)
        {
            TransformUtils.FindDeepChild(gamePanel.transform, "AimControlPanel").gameObject.SetActive(false);
            TransformUtils.FindDeepChild(gamePanel.transform, "SelectVariantPanel").gameObject.SetActive(false);
        }

        activeScreen = gamePanel;
    }

    public void hideGameScreen()
    {
        gamePanel.SetActive(false);
    }

    public void showSelectClassPanel() //refactor Перегруженный код
    {
        activeScreen.SetActive(false);

        selectClassPanel.transform.Find("BackButton").gameObject.SetActive(true);

        Color c;

        //fading class buttons
        GameObject content = selectClassPanel.transform.Find("Content").gameObject;
        foreach (var btnImg in content.GetComponentsInChildren<Image>())
        {
            c = btnImg.color;
            c.a = 1f;
            btnImg.color = c;
        }

        //fading header
        Text header = selectClassPanel.transform.Find("Header").GetComponent<Text>();
        c = header.color;
        c.a = 1f;
        header.color = c;

        selectClassPanel.SetActive(true);

        activeScreen = selectClassPanel;
    }

    public void hideSelectClassPanel()
    {
        selectClassPanel.SetActive(false);
    }

    public void showExitPanel()
    {
        exitPanel.SetActive(true);
    }

    public void hideExitPanel()
    {
        exitPanel.SetActive(false);
    }

    //----------------- Button handlers -------------------
    public void onStartButtonClicked()
    {
    
        //edit show splash screen
        //old code
        //hideStartScreen();
        //showSelectClassPanel();


        animController.animateOnGameStartingUpdated(startPanel, onClassSelectAnimEnd);
        audioSource.Play();

        Debug.Log("start clicked. Time.time = " + Time.time);
    }
    private void onClassSelectAnimEnd()
    {
        //updateStartScreen(); //danger test

        selectClassPanel.transform.localScale = Vector3.one;

        gameController.startGame();
    }

    //don't delete. Возможен переход на другой класс во время игры
    public void onClassButtonClicked(int selectedClass)
    {
        selectClassPanel.transform.Find("BackButton").gameObject.SetActive(false);
        StartCoroutine(gameController.loadSettings(selectedClass));
    }
    public void onSettingsLoaded()
    {
        animController.animateOnGameStarting(selectClassPanel, onClassSelectAnimEnd);
        audioSource.Play();
    }
    

    public void onBackButtonClicked()
    {
        if(activeScreen.Equals(leaderboardPanel))
        {
            hideLeaderBoardScreen();
            showStartScreen();
        }
        else if(activeScreen.Equals(settingsPanel))
        {
            hideSettingsScreen();
            dataController.UpdateUserSettings();
            //StartCoroutine(gameController.saveUserSettings()); //old code
            onSettingsUpdated();
            showStartScreen();
        }
        else if(activeScreen.Equals(gamePanel))
        {
            showExitPanel();
        }
        else if(activeScreen.Equals(restartPanel))
        {
            //hideGameScreen();
            //hideRestartScreen();
            goHome();
        }
        else if(activeScreen.Equals(selectClassPanel))
        {
            hideSelectClassPanel();
            showStartScreen();
        }

        audioSource.Play();
    }

    public void onLeaderboardButtonClicked()
    {
        hideStartScreen();
        showLeaderboardScreen();

        audioSource.Play();
    }

    public void onSettingsButtonClicked()
    {
        showSettingsScreen();

        audioSource.Play();
        //edit
    }

    public void onExitAcceptButtonClicked()
    {
        hideExitPanel();
        gameController.onGameExit();
        goHome();

        audioSource.Play();

        dataController.sendGameCanceledEventToGameAnalytics();
    }

    public void onRestartButtonClicked()
    {
        gameController.restartGame();

        hideRestartScreen();
        showGameScreen();

        audioSource.Play();
    }

    public void onNextTargetButtonClicked()
    {
        targetController.next();
    }

    public void onPrevTargetButtonClicked()
    {
        targetController.last();
    }

    public void onAnswerVariantButtonClicked(Text answerTxt)
    {
        targetController.onActiveTargetShooted(answerTxt.text);
        audioSource.Play();
    }
    public void onLogOutClicked()
    {
        gameController.onLoggedOut();
    }

    //------------------ Updating game UI elements ----------------
    //refactor. Может сделать как-то Thenable? Или просто забить в аниматоре
    public void ShowLevelUpMessage()
    {
        Transform newLevelMessageText = gamePanel.transform.FindDeepChild("NewLevelMessageText");
        StartCoroutine(animController.PulsateAndHide(newLevelMessageText.gameObject));
    }

    public void updatePointsText()
    {
        pointsText.text = gameController.Points.ToString();
    }

    public void updateDamageView(int maxDamage, int damage, bool minDamage = false)
    {
        //if (damage == maxDamage)
        //    return;
        int imgId = maxDamage - damage;
        damagePanel.sprite = Resources.Load<Sprite>("ui/damage_" + imgId);
    }

    public void updateTimerValue(float timerValue)
    {
        float minutes = Mathf.FloorToInt(timerValue / 60);
        float seconds = Mathf.FloorToInt(timerValue % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void updateVariantButtons()
    {
        Text[] varButtonsTxt = TransformUtils.FindDeepChild(gamePanel.transform, "SelectVariantPanel")
            .GetComponentsInChildren<Text>();
        foreach (var vbt in varButtonsTxt)
            vbt.text = "";
    }
    //------------------ Updating game panels -------------------
    //private void UpdateLeaderboardView()
    //{
    //    ClearLeaders();

        

    //    //old with sorting on application side
    //    //UserData[] leaders = dataController.SortUsersByPoints();
    //    //for (int i = 0; i < leaders.Length; i++)
    //    //{

    //    //    Debug.Log("i = "+ i+"Instatiated");
    //    //    GameObject userProgressView = Instantiate(userProgressViewPrefab, leaderboardContent.transform);
    //    //    userProgressView.GetComponent<UserLeaderboardView>().loadViewData(i + 1, leaders[i]);
    //    //    //userProgressView.GetComponent<UserLeaderboardView>().updateView();
    //    //}
    //}
    private void ClearLeaders()
    {
        foreach (Transform leaderItem in leaderboardPanel.transform.FindDeepChild("Content"))
        {
            Destroy(leaderItem.gameObject);
        }
    }
    private void ResetView(Graphic graphic)
    {
        Color startColor = graphic.color;
        startColor.a = 1f;
        graphic.color = startColor;
    }
    

    //------------------Settings handlers -----------------------
    public void setVolumeValue(int value)
    {
        TransformUtils.FindDeepChild(settingsPanel.transform, "VolumeSlider").
            GetComponent<Slider>().SetValueWithoutNotify(value);
    }
    public void onVolumeValueChanged(Slider volume)
    {
        dataController.GameVolume = (int) volume.value;
        audioSource.volume = Mathf.Clamp01(volume.value);
        Debug.Log("GameVolume = " + dataController.GameVolume);

        if(dataController.GameVolume == 0)
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("userBehaviour:volume:off");
    }

    public void setToggleValue(GameMode mode)
    {

        Debug.Log("Setting toggle value");

        ToggleController customToggle = TransformUtils.FindDeepChild(settingsPanel.transform, "ModeToggle").
            GetComponent<ToggleController>();

        switch (mode)
        {
            case GameMode.PC:
                if (customToggle.isOn)
                    customToggle.Switching();
                break;
            case GameMode.TABLET:
                if (!customToggle.isOn)
                    customToggle.Switching();
                break;
            default:
                break;
        }
    }
    public void onToggleClicked(ToggleController toggleController)
    {
        Debug.Log("OnToggleClicked");
        dataController.GameMode = toggleController.isOn ? GameMode.PC : GameMode.TABLET;
    }

    private void onSettingsUpdated()
    {
        if(dataController.GameMode == GameMode.TABLET)
        {
            TransformUtils.FindDeepChild(gamePanel.transform, "AimControlPanel").gameObject.SetActive(true);
            TransformUtils.FindDeepChild(gamePanel.transform, "SelectVariantPanel").gameObject.SetActive(true);
        }
    }
}
