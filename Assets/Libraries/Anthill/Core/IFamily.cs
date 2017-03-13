using System;

namespace Anthill.Core
{
	public interface IFamily
	{
		void ComponentAdded(AntEntity aEntity, Type aComponentType);
		void ComponentRemoved(AntEntity aEntity, Type aComponentType);
		void EntityAdded(AntEntity aEntity);
		void EntityRemoved(AntEntity aEntity);
	}

	public interface IFamily<T> : IFamily
	{
		AntNodeList<T> Nodes { get; }
	}
}