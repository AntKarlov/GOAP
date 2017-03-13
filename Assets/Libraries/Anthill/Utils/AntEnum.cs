using System;

namespace Anthill.Utils
{
	public class AntEnum
	{
		public static T Parse<T>(string aValue)
		{
			return (T)Enum.Parse(typeof(T), aValue);
		}

		public static string[] GetStringValues<T>()
		{
			var list = GetValues<T>();
			string[] result = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				result[i] = list[i].ToString();
			}

			return result;
		}

		public static T[] GetValues<T>()
		{
			if (typeof(T).BaseType != typeof(Enum))
			{
				throw new ArgumentException("T must be of type System.Enum");
			}

			return (T[])Enum.GetValues(typeof(T));
		}
	}
}