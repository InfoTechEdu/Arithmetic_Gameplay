using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    [SerializeField] GameObject asteroidPrefab;


    //Создает астероид с выражением
    public void spawnExpAsteroid(ExpressionController expController, Expression exp, float velocity)
    {
        GameObject expAst = Instantiate(asteroidPrefab);
        if (GameObject.Find(expAst.GetComponent<Asteroid>().targetName))
        { 
        expAst.GetComponent<Asteroid>().init(expController, exp, getAsteroidPos(), velocity);
        }
        else
        { Destroy(expAst); }
    }



    //получение новой позиции для астероида
    private float lastInstantRegion = 0; //Каждый новый астероид появляется на верхней/нижней половине экрана поочередно
    int pseudoRandInstantRegion = 0;
    private Vector2[] screenYRegions = new Vector2[] {
        new Vector2(0f, 0.2f),
        new Vector2(0.2f, 0.4f),
        new Vector2(0.4f, 0.6f),
        new Vector2(0.6f, 0.8f),
        new Vector2(0.8f, 1f)
    };
    private Vector2 getAsteroidPos() //равномерный разброс
    {
        do
        {
            pseudoRandInstantRegion = Random.Range(0, screenYRegions.Length - 1);
        } while (pseudoRandInstantRegion == lastInstantRegion);

        float pseudoRandomViewportYPos = Random.Range(screenYRegions[pseudoRandInstantRegion].x, screenYRegions[pseudoRandInstantRegion].y);

        //transforming to worldpoint //refactor
        Vector2 initPos = Camera.main.ViewportToWorldPoint(new Vector2(1f, pseudoRandomViewportYPos));
        initPos.x += asteroidPrefab.GetComponent<Renderer>().bounds.size.x;

        lastInstantRegion = pseudoRandInstantRegion;
        
        return initPos;
    }
}
