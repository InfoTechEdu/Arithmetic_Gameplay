using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тот же астроид, но без некоторых компонентов. В основном используется для анимации взрыва 
/// </summary>


public class SimpleAsteroid : MonoBehaviour
{
    Vector3 position;
    float scale;
    float velocity;

    Rigidbody2D rb2D;

    bool destroyingStarted = false;
    float destroyFadeDuration = 2f;

    private void Start()
    {
        //init transform
        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        transform.localScale = transform.localScale * scale;


        GetComponent<SpriteRenderer>().sprite = 
            Resources.Load<Sprite>("asteroids/asteroid_" + Random.Range(1, 5)) as Sprite;

        rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = transform.right * velocity;
    }

    public void Update()
    {
        if (!destroyingStarted && gameObject.activeInHierarchy)
            startDestroying();
    }

    public void init(Vector3 position, float scale, float velocity)
    {
        this.position = position;
        this.scale = scale;
        this.velocity = velocity;
    }

    public void onFarAwayFlyed()
    {
        //edit add chicking code
        Destroy(gameObject);
    }

    private void startDestroying()
    {
        destroyingStarted = true;

        StartCoroutine(fadeOut());
    }

    public IEnumerator fadeOut()
    {
        SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();

        Color startColor = spriteRend.color;
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / destroyFadeDuration, 0, 1);
            spriteRend.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < destroyFadeDuration);

        Destroy(gameObject);
    }
}
