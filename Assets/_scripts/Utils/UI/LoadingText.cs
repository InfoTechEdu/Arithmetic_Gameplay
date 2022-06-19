using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    public string textBase = "Loading";
    public float animDelay = 0.4f;

    private Text mainTxt;

    private void Awake()
    {
        mainTxt = GetComponent<Text>();
    }

    float lastAnimatedAt;
    private void Update()
    {
        if(Time.time - lastAnimatedAt > animDelay * 3)
        {
            StartCoroutine(animate());
            lastAnimatedAt = Time.time;
        }
            
    }

    private IEnumerator animate()
    {
        mainTxt.text = textBase + ".";
        yield return new WaitForSeconds(animDelay);
        mainTxt.text = textBase + "..";
        yield return new WaitForSeconds(animDelay);
        mainTxt.text = textBase + "...";
        yield return new WaitForSeconds(animDelay);
    }
}
