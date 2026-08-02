using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Extensions
{
	public static class ListExtensions
	{
		public static List<T> Clone<T>(this List<T> listToClone) where T: ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}
		
		public static void RandomMix <T>(this List<T> target)
		{
			for (int i = 0; i < target.Count; i++)
			{
				T tmp = target[i];
				target.RemoveAt(i);
				target.Insert(UnityEngine.Random.Range(0, target.Count), tmp);
			}
		}
		
		public static void Accept<T>(this List<T> target, List<T> transmitting)
		{
			foreach (T item in transmitting)
				target.Add(item);
		}
		
		public static T GetRandomItem<T>(this List<T> target)
		{
			return RandomItem<T>(target);
		}

		public static List<T> GetRandomItems<T>(this List<T> target,int count)
		{
			var clone = new List<T>();
			for (int i = 0; i < target.Count; i++)
				clone.Add(target[i]);

			int needCount = count;
			if (needCount > clone.Count)
			{
				Log.Assert();
				needCount = clone.Count;
			}
			
			var randomItems = new List<T>();

			for (int i = 0; i < needCount; i++)
			{
				var randomItem = RandomItem<T>(clone);
				randomItems.Add(randomItem);
				
				clone.Remove(randomItem);
			}

			return randomItems;
		}
		
		public static T GetRandomItem<T>(this List<T> target, T except)
		{
			List <T> list = new List<T>();

			foreach (T item in target)
			{
				if(!item.Equals(except))
					list.Add(item);
			}
	    
			return RandomItem<T>(list);
		}
		
		public static T GetRandomItem<T>(this List<T> target, List<T> excepts)
		{
			List <T> list = new List<T>();

			foreach (T item in target)
			{
				if(!excepts.Contains(item))
					list.Add(item);
			}
	    
			return RandomItem<T>(list);
		}
		
		private static T RandomItem<T>(List<T> list)
		{
			int randomIndex = UnityEngine.Random.Range(0, list.Count);
			return list[randomIndex];
		}
		
		public static T SafelyGetItem<T>(this List<T> target,ref int index)
		{
			if (index >= target.Count) index = 0;
			return target[index];
		}
		
		public static T SafelyGetItem<T>(this List<T> target,int index)
		{
			if (index < target.Count)
			{
				return target[index];
			}
			else
			{
				int remainder = index % target.Count;
				return target[remainder];
			}
		}
	}
}
