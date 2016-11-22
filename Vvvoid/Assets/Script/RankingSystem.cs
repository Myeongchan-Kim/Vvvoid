using UnityEngine;

public class RankingSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnRankingButtonClick()
    {
        Social.ShowLeaderboardUI();
    }
}
