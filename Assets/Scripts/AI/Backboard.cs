using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
	/// <summary>
	/// Данный класс реализует память бота.
	///
	/// Принцип работы очень прост, пока бот ползает по карте и видит что-то,
	/// он записывает место положение увиденных предметов в свою память чтобы потом
	/// быстро вернуться к ним когда понадобятся.
	/// </summary>
	public class Backboard : MonoBehaviour
	{
		private List<BackboardData> _board;

		private void Awake()
		{
			_board = new List<BackboardData>();
		}

		public void AddData(BackboardData aData)
		{
			int index = _board.FindIndex(x => string.Equals(x.conditionName, aData.conditionName));
			if (index >= 0 && index < _board.Count)
			{
				_board[index] = aData;
			}
			else
			{
				_board.Add(aData);
			}
		}

		public void Clear()
		{
			_board.Clear();
		}

		public void Remove(BackboardData aData)
		{
			_board.Remove(aData);
		}

		public BackboardData Find(string aCondition)
		{
			int index = _board.FindIndex(x => string.Equals(x.conditionName, aCondition));
			if (index >= 0 && index < _board.Count)
			{
				return _board[index];
			}
			return new BackboardData();
		}
	}

	public struct BackboardData
	{
		public string conditionName;
		public Vector2 position;
		public bool isValid;
	}
}