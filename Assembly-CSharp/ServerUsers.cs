using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x0200074B RID: 1867
public static class ServerUsers
{
	// Token: 0x06003413 RID: 13331 RVA: 0x001418BC File Offset: 0x0013FABC
	public static void Remove(ulong uid)
	{
		ServerUsers.users.Remove(uid);
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x001418CC File Offset: 0x0013FACC
	public static void Set(ulong uid, ServerUsers.UserGroup group, string username, string notes, long expiry = -1L)
	{
		ServerUsers.Remove(uid);
		ServerUsers.User user = new ServerUsers.User
		{
			steamid = uid,
			group = group,
			username = username,
			notes = notes,
			expiry = expiry
		};
		ServerUsers.users.Add(uid, user);
	}

	// Token: 0x06003415 RID: 13333 RVA: 0x00141918 File Offset: 0x0013FB18
	public static ServerUsers.User Get(ulong uid)
	{
		ServerUsers.User user;
		if (!ServerUsers.users.TryGetValue(uid, out user))
		{
			return null;
		}
		if (!user.IsExpired)
		{
			return user;
		}
		ServerUsers.Remove(uid);
		return null;
	}

	// Token: 0x06003416 RID: 13334 RVA: 0x00141948 File Offset: 0x0013FB48
	public static bool Is(ulong uid, ServerUsers.UserGroup group)
	{
		ServerUsers.User user = ServerUsers.Get(uid);
		return user != null && user.group == group;
	}

	// Token: 0x06003417 RID: 13335 RVA: 0x0014196C File Offset: 0x0013FB6C
	public static IEnumerable<ServerUsers.User> GetAll(ServerUsers.UserGroup group)
	{
		return from x in ServerUsers.users.Values
			where x.@group == @group
			where !x.IsExpired
			select x;
	}

	// Token: 0x06003418 RID: 13336 RVA: 0x001419C5 File Offset: 0x0013FBC5
	public static void Clear()
	{
		ServerUsers.users.Clear();
	}

	// Token: 0x06003419 RID: 13337 RVA: 0x001419D4 File Offset: 0x0013FBD4
	public static void Load()
	{
		ServerUsers.Clear();
		string serverFolder = Server.GetServerFolder("cfg");
		if (File.Exists(serverFolder + "/bans.cfg"))
		{
			string text = File.ReadAllText(serverFolder + "/bans.cfg");
			if (!string.IsNullOrEmpty(text))
			{
				Debug.Log("Running " + serverFolder + "/bans.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text);
			}
		}
		if (File.Exists(serverFolder + "/users.cfg"))
		{
			string text2 = File.ReadAllText(serverFolder + "/users.cfg");
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.Log("Running " + serverFolder + "/users.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text2);
			}
		}
	}

	// Token: 0x0600341A RID: 13338 RVA: 0x00141A98 File Offset: 0x0013FC98
	public static void Save()
	{
		foreach (ulong num in (from kv in ServerUsers.users
			where kv.Value.IsExpired
			select kv.Key).ToList<ulong>())
		{
			ServerUsers.Remove(num);
		}
		string serverFolder = Server.GetServerFolder("cfg");
		StringBuilder stringBuilder = new StringBuilder(67108864);
		stringBuilder.Clear();
		foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
		{
			if (!(user.notes == "EAC"))
			{
				stringBuilder.Append("banid ");
				stringBuilder.Append(user.steamid);
				stringBuilder.Append(' ');
				stringBuilder.Append(user.username.QuoteSafe());
				stringBuilder.Append(' ');
				stringBuilder.Append(user.notes.QuoteSafe());
				stringBuilder.Append(' ');
				stringBuilder.Append(user.expiry);
				stringBuilder.Append("\r\n");
			}
		}
		File.WriteAllText(serverFolder + "/bans.cfg", stringBuilder.ToString());
		stringBuilder.Clear();
		foreach (ServerUsers.User user2 in ServerUsers.GetAll(ServerUsers.UserGroup.Owner))
		{
			stringBuilder.Append("ownerid ");
			stringBuilder.Append(user2.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user2.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user2.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		foreach (ServerUsers.User user3 in ServerUsers.GetAll(ServerUsers.UserGroup.Moderator))
		{
			stringBuilder.Append("moderatorid ");
			stringBuilder.Append(user3.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user3.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user3.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		foreach (ServerUsers.User user4 in ServerUsers.GetAll(ServerUsers.UserGroup.SkipQueue))
		{
			stringBuilder.Append("skipqueueid ");
			stringBuilder.Append(user4.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user4.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user4.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		File.WriteAllText(serverFolder + "/users.cfg", stringBuilder.ToString());
	}

	// Token: 0x0600341B RID: 13339 RVA: 0x00141E04 File Offset: 0x00140004
	public static string BanListString(bool bHeader = false)
	{
		List<ServerUsers.User> list = ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToList<ServerUsers.User>();
		StringBuilder stringBuilder = new StringBuilder(67108864);
		if (bHeader)
		{
			if (list.Count == 0)
			{
				return "ID filter list: empty\n";
			}
			if (list.Count == 1)
			{
				stringBuilder.Append("ID filter list: 1 entry\n");
			}
			else
			{
				stringBuilder.Append(string.Format("ID filter list: {0} entries\n", list.Count));
			}
		}
		int num = 1;
		foreach (ServerUsers.User user in list)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.steamid);
			stringBuilder.Append(" : ");
			if (user.expiry > 0L)
			{
				stringBuilder.Append(((double)(user.expiry - (long)Epoch.Current) / 60.0).ToString("F3"));
				stringBuilder.Append(" min");
			}
			else
			{
				stringBuilder.Append("permanent");
			}
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600341C RID: 13340 RVA: 0x00141F40 File Offset: 0x00140140
	public static string BanListStringEx()
	{
		IEnumerable<ServerUsers.User> all = ServerUsers.GetAll(ServerUsers.UserGroup.Banned);
		StringBuilder stringBuilder = new StringBuilder(67108864);
		int num = 1;
		foreach (ServerUsers.User user in all)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user.notes.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user.expiry);
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002A8D RID: 10893
	private static Dictionary<ulong, ServerUsers.User> users = new Dictionary<ulong, ServerUsers.User>();

	// Token: 0x02000E62 RID: 3682
	public enum UserGroup
	{
		// Token: 0x04004BAA RID: 19370
		None,
		// Token: 0x04004BAB RID: 19371
		Owner,
		// Token: 0x04004BAC RID: 19372
		Moderator,
		// Token: 0x04004BAD RID: 19373
		Banned,
		// Token: 0x04004BAE RID: 19374
		SkipQueue
	}

	// Token: 0x02000E63 RID: 3683
	public class User
	{
		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x060052B7 RID: 21175 RVA: 0x001B10BA File Offset: 0x001AF2BA
		[JsonIgnore]
		public bool IsExpired
		{
			get
			{
				return this.expiry > 0L && (long)Epoch.Current > this.expiry;
			}
		}

		// Token: 0x04004BAF RID: 19375
		public ulong steamid;

		// Token: 0x04004BB0 RID: 19376
		[JsonConverter(typeof(StringEnumConverter))]
		public ServerUsers.UserGroup group;

		// Token: 0x04004BB1 RID: 19377
		public string username;

		// Token: 0x04004BB2 RID: 19378
		public string notes;

		// Token: 0x04004BB3 RID: 19379
		public long expiry;
	}
}
