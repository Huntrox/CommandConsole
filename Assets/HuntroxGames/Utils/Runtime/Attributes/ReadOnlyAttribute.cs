
using System;
using UnityEngine;

namespace HuntroxGames.Utils
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ReadOnlyAttribute : PropertyAttribute
	{

	}
}