using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardView : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;
    public Transform content;

    public GameObject loadingText;

    DataController dataController;
    private void OnEnable()
    {
        if(dataController == null)
            dataController = FindObjectOfType<DataController>();

        clearPanel();
        loadingText.SetActive(true);

        dataController.DownloadTop10(onLeaderboardDataUpdated);
        //old code
        //dataController.LoadLeaderboardData(onLeaderboardDataUpdated);
    }

    private void onLeaderboardDataUpdated(LeaderboardData top10)
    {
        loadingText.SetActive(false);
        
        if (top10 == null)
        {
            Debug.LogError("Cannot show leaderboard");
            return;
        }

        for (int i = 0; i < top10.Count; i++)
        {
            Debug.Log("i = " + i + "Instatiated");
            GameObject userProgressView = Instantiate(leaderboardItemPrefab, content);
            userProgressView.GetComponent<UserProgressView>().loadViewData(i + 1, top10.AllUsers[i]);
        }

        //old code
        //loadingText.SetActive(false);
        //fillLeaderboard();
    }

    //old code
    //private void fillLeaderboard()
    //{
    //    for (int i = 0; i < dataController.Leaders.Length; i++)
    //    {
    //        Debug.Log("next user - " + dataController.Leaders[i]);
    //        GameObject nextUser = Instantiate(leaderboardItemPrefab, content);
    //        Debug.Log("[debug] next user instantiated in leaderboard");
    //        nextUser.GetComponent<UserProgressView>().loadViewData(i + 1, dataController.Leaders[i]);
    //    }
    //}

    private void clearPanel()
    {
        //ClearPanel
        if (content.childCount != 0)
            foreach (Transform child in content)
                Destroy(child.gameObject);
    }
}
