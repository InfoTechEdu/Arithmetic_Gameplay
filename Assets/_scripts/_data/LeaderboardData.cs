using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LeaderboardData
{
    public UserData[] allUsers;

    public UserData[] AllUsers { get => allUsers; }
    public int Count { get => allUsers.Length; }

    public LeaderboardData(UserData[] allUsers)
    {
        this.allUsers = allUsers;
    }

    public UserData GetUserByName(string name)
    {
        UserData user = null;
        foreach (var u in allUsers)
        {
            if (u.ProgressData.Name == name)
            {
                user = u;
                break;
            }
        }

        return user;
    }

    public void UpdateUserProgress(UserData ud)
    {
        for (int i = 0; i < allUsers.Length; i++)
        {
            if (allUsers[i].ProgressData.Name == ud.ProgressData.Name)
            {
                allUsers[i].setProgressData(ud.ProgressData);
                //allUsers[i].ProgressData = ud.ProgressData; //old code
                Debug.Log("Progress of user " + ud.ProgressData.Name + " in leaderboard was successfully updated");
                return;
            }
        }

        Debug.LogWarning("User with name " + ud.ProgressData.Name + " not found!");
    }

    public void SortDescending()
    {
        List<UserData> usersList = new List<UserData>();
        usersList.AddRange(allUsers);
        List<UserData> sortedUsersList = usersList.OrderByDescending(o => o.ProgressData.Highscore).ToList();

        allUsers = sortedUsersList.ToArray();
    }

    public void SortAscending()
    {
        List<UserData> usersList = new List<UserData>();
        usersList.AddRange(allUsers);
        List<UserData> sortedUsersList = usersList.OrderBy(o => o.ProgressData.Highscore).ToList();

        allUsers = sortedUsersList.ToArray();
    }

    public override string ToString()
    {
        string result = "LeaderBoard - " + Utils.CollectionUtils.ArrayToString(allUsers);
        return result;
    }
}
