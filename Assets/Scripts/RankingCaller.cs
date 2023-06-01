using UnityEngine;
using System.Collections;

public class RankingCaller : MonoBehaviour 
{
    public RankingController rankingController;
    
    void OnEnable()
    {             
        rankingController.PrintRankingData();
    }
}
