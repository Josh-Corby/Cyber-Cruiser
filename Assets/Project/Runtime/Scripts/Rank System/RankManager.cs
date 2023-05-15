using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class RankManager : GameBehaviour<RankManager>
    {
        [SerializeField] private RankScriptableObject[] _rankSos;

        public List<Rank> ranks = new List<Rank>();

        protected override void Awake()
        {
            base.Awake();
            PopulateRanksList();
        }

        private void PopulateRanksList()
        {
            for (int i = 0; i < _rankSos.Length; i++)
            {
                ranks.Add(_rankSos[i].rank);
            }
        }

        public Rank GetRank(int rankID)
        {
            return ranks[rankID];
        }

        public Rank RankUp(int rankID)
        {
            return ranks[rankID + 1];
        }
    }
}