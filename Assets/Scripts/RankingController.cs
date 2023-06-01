using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RankingController : MonoBehaviour 
{
    public int maxRankingPositions;

    public Text rankingNamesText;
    public Text rankingPointsText;

    public void ResetRankingData()
    {
        for (int i = 1; i < maxRankingPositions + 1; i++)
        {
            PlayerPrefs.SetInt("PlayerPoint" + i.ToString(), 0);
            PlayerPrefs.SetString("PlayerName" + i.ToString(), ""); // "--------------"
        }
    }

    public bool IsTop20(int points)
    {
        for (int i = 1; i < maxRankingPositions + 1; i++)
        {
            if (points > PlayerPrefs.GetInt("PlayerPoint" + i.ToString()))
            {
                return true;
            }
        }

        return false;
    }

    public int GetTop20Position(int pontuation)
    {
        for (int i = 1; i < maxRankingPositions + 1; i++)
        {
            if (pontuation > PlayerPrefs.GetInt("PlayerPoint" + i))
            {
                return i;
            }
        }

        return 0;
    }

    public void RefreshRanking(string top20Name, int pontuation, int position)
    {
        for (int i = maxRankingPositions; i >= position; i--)
        {
            int nextPosition = i + 1;
            int currentPoints = PlayerPrefs.GetInt("PlayerPoint" + i);
            string currentName = PlayerPrefs.GetString("PlayerName" + i);

            PlayerPrefs.SetInt("PlayerPoint" + nextPosition, currentPoints);
            
            PlayerPrefs.SetString("PlayerName" + nextPosition, currentName);
        }

        PlayerPrefs.SetInt("PlayerPoint" + position, pontuation);
        PlayerPrefs.SetString("PlayerName" + position, top20Name);
    }

    public void PrintRankingData()
    {       
        for (int i = 1; i < maxRankingPositions + 1; i++)
        {
            if (i == 1)
            {
                rankingNamesText.text = PlayerPrefs.GetString("PlayerName" + i) + "\n";
                rankingPointsText.text = PlayerPrefs.GetInt("PlayerPoint" + i) + "\n";                
            }
            else
            {
                rankingNamesText.text += PlayerPrefs.GetString("PlayerName" + i) + "\n";
                rankingPointsText.text += PlayerPrefs.GetInt("PlayerPoint" + i) + "\n";
            }            
        }
    }      
}
