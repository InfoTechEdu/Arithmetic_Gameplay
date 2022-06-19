using FullSerializer;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataParser
{
    private static fsSerializer serializer = new fsSerializer();

    public static void ParseUserData(string jsonData, out UserData userData)
    {
        userData = new UserData();

        JSONNode userDataJsonObj = JSONNode.Parse(jsonData);

        Debug.Log("[debug] parsing user jsonData - " + jsonData);

        //loading progress data
        UserProgressData upd = JsonUtility.FromJson<UserProgressData>(userDataJsonObj["public"]["progressData"].ToString());
        StatisticsData sd = JsonUtility.FromJson<StatisticsData>(userDataJsonObj["public"]["statistics"].ToString());
        userData.updatePublicData(upd, sd);

        Debug.LogWarning("[debug] result progress - " + upd);
        Debug.LogWarning("[debug] result statistics - " + sd);

        //getting class data
        userData.userClass = userDataJsonObj["public"]["userClass"].Value.ToString();
        userData.status = userDataJsonObj["public"]["status"].Value.ToString();
        Debug.LogWarning("[debug] User class - " + userData.userClass);
        Debug.LogWarning("[debug] User status - " + userData.status);

        //getting profilePhotoUrl (private)
        userData.setProfilePhotoUrl(userDataJsonObj["public"]["profilePhoto"].Value.ToString()); //Так как без Value ставит кавычки в начале и конце
    }
    public static void ParsePublicUserData(string publicUserJsonData, out UserData userData)
    {
        Debug.Log("Passed - " + publicUserJsonData);
        if (string.IsNullOrEmpty(publicUserJsonData))
        {
            Debug.LogWarning("No data for parsing. Return...");
            userData = null;
            return;
        }

        userData = new UserData();

        JSONNode publicUserDataJsonObj = JSONNode.Parse(publicUserJsonData);

        //loading progress data
        UserProgressData upd = JsonUtility.FromJson<UserProgressData>(publicUserDataJsonObj["progressData"].ToString());
        StatisticsData sd = JsonUtility.FromJson<StatisticsData>(publicUserDataJsonObj["statistics"].ToString());
        userData.updatePublicData(upd, sd);

        Debug.LogWarning("[debug] result progress - " + upd);
        Debug.LogWarning("[debug] result statistics - " + sd);

        //getting class data
        userData.userClass = publicUserDataJsonObj["userClass"].Value.ToString();
        userData.status = publicUserDataJsonObj["status"].Value.ToString();
        Debug.LogWarning("[debug] User class - " + userData.userClass);
        Debug.LogWarning("[debug] User status - " + userData.status);

        //getting profilePhotoUrl (private)
        userData.setProfilePhotoUrl(publicUserDataJsonObj["profilePhoto"].Value.ToString()); //Так как без Value ставит кавычки в начале и конце
    }
    public static void ParseLeaderboardData(string jsonData, out LeaderboardData leaderboardData)
    {
        Debug.Log($"Leaderboard data was get - {jsonData}. Starting parsing...");
        JSONNode leaderboardJsonObj = JSONNode.Parse(jsonData);

        List<UserData> users = new List<UserData>();
        foreach (var nextUserNode in leaderboardJsonObj)
        {
            UserData next = new UserData();

            UserProgressData upd = JsonUtility.FromJson<UserProgressData>(nextUserNode.Value["progressData"].ToString());
            StatisticsData sd = JsonUtility.FromJson<StatisticsData>(nextUserNode.Value["statistics"].ToString());
            next.updatePublicData(upd, sd);

            next.setProfilePhotoUrl(nextUserNode.Value["profilePhoto"].Value);

            next.setId(nextUserNode.Key);

            Debug.Log("[debug] Next leaderboard user was parsed. Result - " + next);

            users.Add(next);
        }

        leaderboardData = new LeaderboardData(users.ToArray());
    }
    public static void ParseFriendsList(string jsonData, out List<UserData> activeFriends, out List<UserData> expectedFriends, out List<UserData> wishingToBeFriends)
    {
        activeFriends = new List<UserData>();
        expectedFriends = new List<UserData>();
        wishingToBeFriends = new List<UserData>();

        JSONNode friendsJsonObj = JSONNode.Parse(jsonData);
        if (friendsJsonObj == null)
            return;


        foreach (var active in friendsJsonObj["active"])
            activeFriends.Add(new UserData(active.Key));

        foreach (var expected in friendsJsonObj["expected"])
            expectedFriends.Add(new UserData(expected.Key));

        foreach (var wishing in friendsJsonObj["wishing"])
            wishingToBeFriends.Add(new UserData(wishing.Key));
    }
    

}
