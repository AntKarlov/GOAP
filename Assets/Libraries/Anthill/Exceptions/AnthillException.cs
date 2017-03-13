using System;

namespace Anthill.Exceptions
{
	public class AnthillException : Exception
	{
		public AnthillException(string aMessage, Exception aInnerException) : base(aMessage, aInnerException)
		{
			//...
		}
	}
}