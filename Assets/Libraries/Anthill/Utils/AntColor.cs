using UnityEngine;

namespace Anthill.Utils
{
	public class AntColor
	{
		/// <summary>
		/// Преобразует значение Color в Hex формат.
		/// </summary>
		/// <param name="aColor">Color значение.</param>
		/// <returns>Возвращает Hex код цвета в формате string.</returns>
		public static string ColorToHex(Color aColor)
		{
			return aColor.r.ToString("X2") + aColor.g.ToString("X2") + aColor.b.ToString("X2");
		}

		/// <summary>
		/// Преобразует значение Hex в Color формат.
		/// </summary>
		/// <param name="aHex">Hex код цвета в формате string.</param>
		/// <returns>Возвращает Color значение.</returns>
		public static Color HexToColor(string aHex)
		{
			aHex = aHex.Replace("0x", "");
			aHex = aHex.Replace("#", "");

			byte a = 255;
			byte r = byte.Parse(aHex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(aHex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(aHex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

			if (aHex.Length == 8)
			{
				a = byte.Parse(aHex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			}

			return new Color(r, g, b, a);
		}
	}
}