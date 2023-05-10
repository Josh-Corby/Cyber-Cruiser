using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankManager : GameBehaviour<RankManager>
{
    [SerializeField] private RankScriptableObject[] _rankSOs;

    public List<Rank> ranks = new List<Rank>();

    public Rank GetRank(int rankID)
    { 
        return ranks[rankID]; 
    }
}
