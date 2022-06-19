using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour, TargetBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject simpleAsteroidPrefab;

    public string targetName;

    Camera eventCamera;

    private Transform target;

    private ExpressionController expController;
    //private GameController controller;

    private Expression expression;

    private SpriteRenderer spriteRenderer;

    private AudioSource audio;

    private DataController dataController;

    Button[] vButtons;
    Text expressionTxt;
    int[] answerVariants; //true answer included
    GameObject selectVariantPanel;


    bool isLocked = false;
    bool isAnswered = false;

    #region MONO BEHAVIOUR METHODS

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();

        Canvas canvas = GetComponentInChildren<Canvas>(); //refactor. Это сделано, чтобы у миниастероидов не вызывался данный код
        if(canvas != null)
            canvas.worldCamera = Camera.main;

        Debug.LogWarning("Test. boxColleder bounds - " + GetComponent<BoxCollider2D>().size);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("asteroids/asteroid_" + Random.Range(1, 5)) as Sprite;
        Debug.LogWarning("Asteroid Instantiated. sprite bounds - " + spriteRenderer.bounds.size);
        Debug.LogWarning("Asteroid Instantiated. boxColleder bounds - " + GetComponent<BoxCollider2D>().size);

        GetComponent<BoxCollider2D>().size = spriteRenderer.bounds.size * 2; //Возможно из-за скейла астероида без умножения не идет. Точно не знаю пока

        //Tests
        //GameObject aim = Instantiate(Resources.Load("Aim", typeof(GameObject))) as GameObject;
        //aim.GetComponent<SpriteRenderer>().size = 
        //    new Vector2(spriteRenderer.bounds.size.x + 1, spriteRenderer.bounds.size.x + 1);
        //aim.transform.SetParent(transform);
        //aim.transform.localPosition = Vector3.zero;


        vButtons = GetComponentsInChildren<Button>();
        selectVariantPanel =  
            Utils.TransformUtils.FindDeepChild(GameObject.Find("Canvas").transform, "SelectVariantPanel").gameObject;

        if (GameObject.Find(targetName))
        {
            target = GameObject.Find(targetName).transform;
        }
        if(dataController.GameMode == GameMode.TABLET)
        {
            disableVariantButtons();
        }
        else
        {
            selectVariantPanel = null;
            GetComponent<Target>().enabled = false;
        }

        audio = GetComponent<AudioSource>();
        audio.volume = Mathf.Clamp01(dataController.GameVolume);
    }
    private void OnDestroy()
    {
        //edit Добавить анимацию взрыва
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLocked)
            return;

        if(collision.tag == GameTags.BULLET && isAnswered)
        {
            destroy(collision.transform); 
            
            Destroy(collision.gameObject);
        }
    }
    #endregion

    #region PUBLIC METHODS
    //Инициализация астероида с выражением
    public void init(ExpressionController expController, Expression exp, Vector2 pos, float velocity)
    {
        this.expController = expController;

        expression = exp;
        answerVariants = exp.AllAnswers;

        //init position
        transform.position = pos;

        //init velocity and target
        setVelocity(velocity);

        expressionTxt = Utils.TransformUtils.FindDeepChild(transform, "ExpressionText")
            .GetComponent<Text>();
        if (expressionTxt == null) Debug.LogWarning("Expression text is null!");
        expressionTxt.text = expression.ExpString;

        if(dataController.GameMode == dataController.GameMode)
            initVarinatsButtons();

        //Debug.Log("Next Asteroid Instantiated. Expression - " + exp); //test
    }

    public void destroy(Transform target, bool hasExplosion = true) //refactor rename target parameter
    {
        audio.clip = Resources.Load<AudioClip>("sounds/explosion_" + Random.Range(1, 3));
        audio.Play();

        if (hasExplosion)
            if(target != null)
                initExplosion(target.transform);
        generateMiniAsteroids(Random.Range(3, 5), 0.3f, 2.5f);
        Destroy(gameObject);
    }
    
    public void @lock()
    {
        //edit animate fade asteroid
        fade();
        //hideExpression();

        isLocked = true;
        Debug.Log("Wrong answer. Asteroid locked"); //edit
    }

    public void solved()
    {
        hideExpression();
    }

    //Callback. Вызывается при клике кнопки варианта ответа пользователем
    public void OnAnswerVariantClicked(Text variantText)
    {
        isAnswered = true;

        Debug.LogWarning("OnAnswerVariant clicked");
        hideExpression();
        int.TryParse(variantText.text, out int result);
        expController.answered(transform, result == expression.CorrectAnswer);
        
        //controller.answered(this, result == expression.Answer);
    }

    private void hideExpression()
    {
        Debug.LogWarning("Called hide exp");
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);
    }
    #endregion

    #region PRIVATE METHODS
    private void initExplosion(Transform target) //refactor rename target parameter
    {
        Instantiate(explosionPrefab, target.position, target.rotation);
    }

    private void generateMiniAsteroids(int count, float scaleSize, float velocity)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject asteroidClone = Instantiate(simpleAsteroidPrefab);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0f);
            asteroidClone.GetComponent<SimpleAsteroid>().init(pos, scaleSize, velocity);
            asteroidClone.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        }
    }

    //Устанавливает скорость и траекторию движения астероида
    private void setVelocity(float velocity)
    {
        GetComponent<Rigidbody2D>().velocity = (target.position - transform.position).normalized * velocity;
    }

    private void fade()
    {
        Color c = spriteRenderer.color;
        c.a = 0.4f;
        spriteRenderer.color = c;
    }

    private void initVarinatsButtons()
    {
        int[] answers = expression.AllAnswers;
        for (int i = 0; i < vButtons.Length; i++)
            vButtons[i].GetComponentInChildren<Text>().text = answers[i].ToString();

        //edit заполнение данными кнопки
    }
    private void disableVariantButtons()
    {
        vButtons[0].transform.parent.gameObject.SetActive(false);
    }
    private void updateGameVariantButtons()
    {
        if (!selectVariantPanel.activeInHierarchy)
            selectVariantPanel.SetActive(true);

        Button[] variantButtons = selectVariantPanel.GetComponentsInChildren<Button>();

        for (int i = 0; i < vButtons.Length; i++)
        {
            variantButtons[i].GetComponentInChildren<Text>().text = answerVariants[i].ToString();
            //variantButtons[i].onClick.AddListener(OnAnswerVariantClicked(
            //    variantButtons[i].GetComponentInChildren<Text>()));
        }
            
    }

    void TargetBehaviour.OnTargeted()
    {
        updateGameVariantButtons();
    }

    void TargetBehaviour.OnShooted(string userAnswer)
    {
        isAnswered = true;

        hideExpression();
        int.TryParse(userAnswer, out int result);
        expController.answered(transform, result == expression.CorrectAnswer);
    }

    //public void OnTargeted(string param)
    //{
    //    hideExpression();
    //    int.TryParse(param, out int result);
    //    expController.answered(transform, result == expression.Answer);
    //}
    #endregion
}


