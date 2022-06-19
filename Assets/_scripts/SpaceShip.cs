using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] GameController gameController;

    [SerializeField] Transform muzzle;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] AudioSource fireAudio;

    DataController dataController;

    public float wiggleVielocity = 1.4f;

    public float topWiggleViewportY = 0.8f;
    public float bottomWiggleViewportY = 0.2f;

    int damage = 0;
    int maxDamage = 3;
    bool destroyed = false;

    //границы "покачивания" корабля
    float bottomWiggleY;
    float topWiggleY;

    Rigidbody2D rb2D;

    private void Awake()
    {
        fireAudio = GetComponent<AudioSource>();

        dataController = FindObjectOfType<DataController>();
    }

    private void Start()
    {
        //gameController = GameObject.Find("GameController").GetComponent<GameController>();
        

        initWiggleBorders();

        initPosAndVelocity();
    }
    
    private void Update()
    {
        if (destroyed)
            return;

        updateWiggleVelocity();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GameTags.ASTEROID)
        {
            damaged(collision.GetComponent<Asteroid>());
            
        }
    }

    public void OnEnable()
    {
        damage = 0;
        maxDamage = 3;
        destroyed = false;
        initPosAndVelocity();
        fireAudio.volume = Mathf.Clamp01(dataController.GameVolume);
        //updateWiggleVelocity()
        //edit set position
    }

    public void damaged(Asteroid asteroid)
    {
        if(damagePlaying)
            StopAllCoroutines();
        StartCoroutine(damageAnim(4f));

        asteroid.destroy(transform, false);
        damage++;
        gameController.onDamaged();
        if (damage == maxDamage)
            gameController.gameOver();
    }

    public void destroy()
    {
        destroyed = true;
        gameObject.SetActive(false);
    }

    public void activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn(Mathf.Epsilon));
    }

    public float bulletSpeed = 1;
    public void fire(Transform target)
    {
        fireAudio.Play();

        Quaternion rot = Quaternion.Euler((target.position - muzzle.position).normalized);

        GameObject shell = Instantiate(bulletPrefab);
        shell.transform.position = muzzle.position;
        //find the vector pointing from our position to the target
        Vector3 _direction = (target.position - muzzle.position);

        //create the rotation we need to be in to look at the target
        Quaternion rot2 = Quaternion.LookRotation(_direction);
        
        rot2.y = 0f;
        shell.transform.rotation = rot2;// Quaternion.EulerAngles(0f, 0f, (target.position - muzzle.position).normalized.z);
        Rigidbody2D shellRigidbody2 = shell.GetComponent<Rigidbody2D>();
        shellRigidbody2.velocity = bulletSpeed * shell.transform.right;

        //shell.transform.LookAt(target);
        return;

        shell.transform.rotation = shell.transform.rotation.normalized;

        Debug.LogWarning(" shell rot = " + shell.transform.rotation);
        Rigidbody2D shellRigidbody = shell.GetComponent<Rigidbody2D>();
        Debug.LogWarning("shell forward = " + shell.transform.forward);
        Debug.LogWarning("shell velocity = "+ (bulletSpeed * shell.transform.forward));
        shellRigidbody.velocity = bulletSpeed * shell.transform.forward;// (target.transform.position - muzzle.transform.position).normalized; //muzzle.right;
        Debug.LogWarning("velocity shell = " + shellRigidbody.velocity);

        

        //// Create an instance of the shell and store a reference to it's rigidbody.
            //Rigidbody2D shellInstance =
        //    Instantiate(bulletPrefab, muzzle.position, muzzle.rotation) as Rigidbody2D;
        //Debug.Log("muzzle forward = " + muzzle.forward);

        //// Set the shell's velocity to the launch force in the fire position's forward direction.
        //shellInstance.velocity = speed * muzzle.forward;




        //Instantiate(bulletPrefab, muzzle.transform).GetComponent<Rigidbody2D>().velocity = muzzle.forward * speed;
        //Instantiate(bulletPrefab, muzzle.transform).GetComponent<Rigidbody2D>().AddForce(muzzle.transform.position * -1, ForceMode2D.Impulse);
        //target.gameObject.SetActive(false);
    }

    
    private void initWiggleBorders()
    {
        bottomWiggleY = Camera.main.ViewportToWorldPoint(new Vector2(transform.position.x, bottomWiggleViewportY)).y;
        topWiggleY = Camera.main.ViewportToWorldPoint(new Vector2(transform.position.x, topWiggleViewportY)).y;
    }
    private void initPosAndVelocity()
    {
        if (rb2D == null) //refactor bad code
            rb2D = GetComponent<Rigidbody2D>();

        //transform.position = GetComponent<ScreenPoint>().StarterWorldPoint;
        Vector3 startPoint = GetComponent<ScreenPoint>().StarterWorldPoint;
        iTween.MoveTo(gameObject, startPoint, 2f);

        rb2D.velocity = transform.position - new Vector3(transform.position.x, Random.Range(0, topWiggleY), transform.position.z);

        targetPos = Vector3.zero; //refactor bad code
    }


    private float directionToggle = 1;
    private Vector3 targetPos;
    private void updateWiggleVelocity()
    {
        if(rb2D.velocity.y < 0 && transform.position.y <= targetPos.y ||
            rb2D.velocity.y > 0 && transform.position.y >= targetPos.y)
        {
            float wiggleBorder = (rb2D.velocity.y < 0) ? bottomWiggleY : topWiggleY;
            targetPos = new Vector3(transform.position.x, Random.Range(0, wiggleBorder), transform.position.z);

            rb2D.velocity = (targetPos - transform.position).normalized * wiggleVielocity * directionToggle;
            directionToggle *= -1;
        }



        return;

        if(directionToggle < 0 && transform.position.y >= targetPos.y ||
            directionToggle > 0 && transform.position.y <= targetPos.y)
        {

            targetPos = new Vector3(transform.position.x, Random.Range(0, topWiggleY), transform.position.z);

            Vector3 diff = (targetPos - transform.position).normalized;
            rb2D.velocity = (targetPos - transform.position).normalized * wiggleVielocity * directionToggle;
            directionToggle *= -1;

            Debug.Log(diff);
        }        
    }

    bool damagePlaying = false;
    private IEnumerator damageAnim(float duration)
    {
        damagePlaying = true;
        float fadeDuration = duration / 4f;
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(FadeOut(fadeDuration));
            yield return new WaitForSeconds(duration / 4f);
            StartCoroutine(FadeIn(fadeDuration));
            yield return new WaitForSeconds(duration / 4f);
        }

        damagePlaying = false;
    }
    private IEnumerator FadeIn(float duration)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        renderers.Add(GetComponent<SpriteRenderer>());
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

        Color startColor = Color.white;
        startColor.a = 0;
        Color endColor = startColor;
        endColor.a = 1f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            foreach (var rend in renderers)
            {
                rend.color = Color.Lerp(startColor, endColor, normalisedTime);
            }
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }
    private IEnumerator FadeOut(float duration)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        renderers.Add(GetComponent<SpriteRenderer>());
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

        Color startColor = Color.white;
        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            foreach (var rend in renderers)
            {
                rend.color = Color.Lerp(startColor, endColor, normalisedTime);
            }
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }


    public float BottomWiggleY { get => bottomWiggleY; set => bottomWiggleY = value; }
    public float TopWiggleY { get => topWiggleY; set => topWiggleY = value; }
    public int Damage { get => damage; set => damage = value; }
    public int MaxDamage { get => maxDamage; set => maxDamage = value; }
}
