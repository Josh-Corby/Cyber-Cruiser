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