using System;
using UnityEngine;

namespace CyberCruiser
{
	[CreateAssetMenu(fileName = "Rank", menuName = "ScriptableObject/Rank")]
	public class RankScriptableObject : ScriptableObject
	{
		public Rank rank;
	}

}
//[Serializable]
//public class Rank
//{
//    [SerializeField] private string _name;
//    [SerializeField] Sprite _sprite;
//    [SerializeField] Sprite _rankTextSprite;
//    [SerializeField] private int _rankID;
//    [SerializeField] private int _starsToRankUp = 3;

//    public string Name { get => _name; }
//    public Sprite Sprite { get => _sprite; }
//    public Sprite RankTextSprite { get => _rankTextSprite; }
//    public int RankID { get => _rankID; }
//    public int StarsToRankUp { get => _starsToRankUp; }
//}