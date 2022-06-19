
using UnityEngine;
using UnityEngine.UI;

public class UserProgressView : MonoBehaviour
{
    int position;
    UserData userData;

    private Text positionTxt;
    private Text nameTxt;
    private Text scoreTxt;

    private void Awake()
    {
        positionTxt = transform.Find("Position").GetComponentInChildren<Text>();
        nameTxt = transform.Find("Name").GetComponent<Text>();
        scoreTxt = transform.Find("Score").GetComponent<Text>();

        
    }

    private void Start()
    {
        updateData();
    }

    public void loadViewData(int position, UserData ud)
    {
        this.position = position;
        userData = ud;
    }

    private void updateData()
    {
        positionTxt.text = position.ToString();

        nameTxt.text = userData.ProgressData.Name;
        scoreTxt.text = userData.ProgressData.Highscore.ToString();
    }
}
