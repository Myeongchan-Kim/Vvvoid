using UnityEngine;
using System.Collections;
using GooglePlayGames;

public class RankingManager : MonoBehaviour {

    public StatManager statManger;

	public void OnClick()
    {
        
        Social.ReportScore((long)statManger.Distance, "CgkI5YeLpasSEAIQAg", (bool sucess) => { if (sucess) Debug.Log("Score Update Success"); else Debug.Log("Scored Update Fail"); });

        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI("CgkI5YeLpasSEAIQAg");
    }
}
