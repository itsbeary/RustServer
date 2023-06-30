using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000895 RID: 2197
public class ServerBrowserList : BaseMonoBehaviour, VirtualScroll.IDataSource
{
	// Token: 0x060036CB RID: 14027 RVA: 0x00007A44 File Offset: 0x00005C44
	public int GetItemCount()
	{
		return 0;
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x000063A5 File Offset: 0x000045A5
	public void SetItemData(int i, GameObject obj)
	{
	}

	// Token: 0x04003189 RID: 12681
	public ServerBrowserList.QueryType queryType;

	// Token: 0x0400318A RID: 12682
	public static string VersionTag = "v" + 2397;

	// Token: 0x0400318B RID: 12683
	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	// Token: 0x0400318C RID: 12684
	public ServerBrowserCategory categoryButton;

	// Token: 0x0400318D RID: 12685
	public bool startActive;

	// Token: 0x0400318E RID: 12686
	public Transform listTransform;

	// Token: 0x0400318F RID: 12687
	public int refreshOrder;

	// Token: 0x04003190 RID: 12688
	public bool UseOfficialServers;

	// Token: 0x04003191 RID: 12689
	public VirtualScroll VirtualScroll;

	// Token: 0x04003192 RID: 12690
	public ServerBrowserList.Rules[] rules;

	// Token: 0x04003193 RID: 12691
	public bool hideOfficialServers;

	// Token: 0x04003194 RID: 12692
	public bool excludeEmptyServersUsingQuery;

	// Token: 0x04003195 RID: 12693
	public bool alwaysIncludeEmptyServers;

	// Token: 0x04003196 RID: 12694
	public bool clampPlayerCountsToTrustedValues;

	// Token: 0x02000EAE RID: 3758
	public enum QueryType
	{
		// Token: 0x04004CEB RID: 19691
		RegularInternet,
		// Token: 0x04004CEC RID: 19692
		Friends,
		// Token: 0x04004CED RID: 19693
		History,
		// Token: 0x04004CEE RID: 19694
		LAN,
		// Token: 0x04004CEF RID: 19695
		Favourites,
		// Token: 0x04004CF0 RID: 19696
		None
	}

	// Token: 0x02000EAF RID: 3759
	[Serializable]
	public struct ServerKeyvalues
	{
		// Token: 0x04004CF1 RID: 19697
		public string key;

		// Token: 0x04004CF2 RID: 19698
		public string value;
	}

	// Token: 0x02000EB0 RID: 3760
	[Serializable]
	public struct Rules
	{
		// Token: 0x04004CF3 RID: 19699
		public string tag;

		// Token: 0x04004CF4 RID: 19700
		public ServerBrowserList serverList;
	}

	// Token: 0x02000EB1 RID: 3761
	private class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> where T : IComparable<T>
	{
		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600532C RID: 21292 RVA: 0x001B1DA7 File Offset: 0x001AFFA7
		public static ServerBrowserList.HashSetEqualityComparer<T> Instance { get; } = new ServerBrowserList.HashSetEqualityComparer<T>();

		// Token: 0x0600532D RID: 21293 RVA: 0x001B1DB0 File Offset: 0x001AFFB0
		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (y == null)
			{
				return false;
			}
			if (x.GetType() != y.GetType())
			{
				return false;
			}
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (T t in x)
			{
				if (!y.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600532E RID: 21294 RVA: 0x001B1E3C File Offset: 0x001B003C
		public int GetHashCode(HashSet<T> set)
		{
			int num = 0;
			if (set != null)
			{
				foreach (T t in set)
				{
					num ^= ((t != null) ? t.GetHashCode() : 0) & int.MaxValue;
				}
			}
			return num;
		}
	}
}
