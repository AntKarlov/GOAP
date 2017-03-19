using System.Text;
using UnityEngine;
using Anthill.Exceptions;

namespace Anthill.Utils
{
	public enum Colors
	{
		aqua,
		black,
		blue,
		brown,
		darkblue,
		fuchsia,
		green,
		grey,
		lightblue,
		lime,
		maroon,
		navy,
		olive,
		orange,
		purple,
		red,
		silver,
		teal,
		white,
		yellow
	}

	public class AntLog
	{
		/// <summary>
		///	Надстройка-помошник для удобной отправки сообщений в консоль.
		///
		/// Пример использования:
		///   AntLog.Trace("Its a number", value);
		///   AntLog.Error("<color=red>Fatal Error:</color>: File", fileName, "not found");
		///   AntLog.Warning("Warning!");
		/// </summary>
		public static string Message(params object[] aArgs)
		{
			string result = null;
			if (aArgs[0] is string && CountOfBrackets((string) aArgs[0]) == aArgs.Length - 1)
			{
				object[] args = new object[aArgs.Length - 1];
				for (int i = 1, n = aArgs.Length; i < n; i++)
				{
					args[i - 1] = aArgs[i];
				}
				result = string.Format((string) aArgs[0], args);
			}

			if (result == null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0, n = aArgs.Length; i < n; i++)
				{
					sb.Append(aArgs[i].ToString());
					sb.Append(" ");
				}
				result = sb.ToString();	
			}

			return result;
		}

		private static int CountOfBrackets(string aStr)
		{
			int count = 0;
			bool opened = false;
			for (int i = 0, n = aStr.Length; i < n; i++)
			{
				switch (aStr[i])
				{
					case '{' : 
						opened = true;
					break;

					case '}' :
						if (opened)
						{
							count++;
							opened = false;
						}
					break;
				}
			}

			return count;
		}

		public static void Guard(bool aFailed, string aFormat, params object[] aArgs)
		{
			if (aFailed)
			{
				throw new AnthillException(string.Format(aFormat, aArgs), null);
			}
		}

		public static string Report(string aClassName, string aFormat, params object[] aArgs)
		{
			object[] args = new object[aArgs.Length + 1];
			args[0] = string.Concat("<color=orange>[", aClassName, "]</color> ", aFormat);
			for (int i = 0, n = aArgs.Length; i < n; i++)
			{
				args[i + 1] = aArgs[i];
			}
			string str = Message(args);
			Debug.LogWarning(str);
			return str;
		}

		public static string Trace(params object[] aArgs)
		{
			string str = Message(aArgs);
			Debug.Log(str);
			return str;
		}

		public static string Warning(params object[] aArgs)
		{
			string str = Message(aArgs);
			Debug.LogWarning(str);
			return str;
		}

		public static string Error(params object[] aArgs)
		{
			string str = Message(aArgs);
			Debug.LogError(str);
			return str;
		}

		public static string Colored(string aText, Colors aColor)
		{
			return string.Format("<color={0}>{1}</color>", aColor, aText);
		}

		public static string Colored(string aText, string aColorName)
		{
			return string.Format("<color={0}>{1}</color>", aColorName, aText);
		}

		public static string Sized(string aText, int aSize)
		{
			return string.Format("<size={0}>{1}</size>", aSize, aText);
		}

		public static string Bold(string aText)
		{
			return string.Format("<b>{0}</b>", aText);
		}

		public static string Italic(string aText)
		{
			return string.Format("<i>{0}</i>", aText);
		}
	}
}
