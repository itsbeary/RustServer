using System;
using System.Collections;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x02000883 RID: 2179
public static class SteamNewsSource
{
	// Token: 0x06003683 RID: 13955 RVA: 0x0014945A File Offset: 0x0014765A
	public static IEnumerator GetStories()
	{
		WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return www;
		JSON.Object @object = JSON.Object.Parse(www.text);
		www.Dispose();
		if (@object == null)
		{
			yield break;
		}
		JSON.Array array = @object.GetObject("appnews").GetArray("newsitems");
		List<SteamNewsSource.Story> list = new List<SteamNewsSource.Story>();
		foreach (Value value in array)
		{
			string text = value.Obj.GetString("contents", "Missing Contents");
			text = text.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
			list.Add(new SteamNewsSource.Story
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = text,
				author = value.Obj.GetString("author", "Missing Author")
			});
		}
		SteamNewsSource.Stories = list.ToArray();
		yield break;
	}

	// Token: 0x0400314E RID: 12622
	public static SteamNewsSource.Story[] Stories;

	// Token: 0x02000EA9 RID: 3753
	public struct Story
	{
		// Token: 0x04004CDB RID: 19675
		public string name;

		// Token: 0x04004CDC RID: 19676
		public string url;

		// Token: 0x04004CDD RID: 19677
		public int date;

		// Token: 0x04004CDE RID: 19678
		public string text;

		// Token: 0x04004CDF RID: 19679
		public string author;
	}
}
