using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADE RID: 2782
	[ConsoleSystem.Factory("pool")]
	public class Pool : ConsoleSystem
	{
		// Token: 0x060042BC RID: 17084 RVA: 0x0018AC8C File Offset: 0x00188E8C
		[ServerVar]
		[ClientVar]
		public static void print_memory(ConsoleSystem.Arg arg)
		{
			if (Pool.Directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("capacity");
			textTable.AddColumn("pooled");
			textTable.AddColumn("active");
			textTable.AddColumn("hits");
			textTable.AddColumn("misses");
			textTable.AddColumn("spills");
			foreach (KeyValuePair<Type, Pool.IPoolCollection> keyValuePair in Pool.Directory.OrderByDescending((KeyValuePair<Type, Pool.IPoolCollection> x) => x.Value.ItemsCreated))
			{
				Type key = keyValuePair.Key;
				Pool.IPoolCollection value = keyValuePair.Value;
				textTable.AddRow(new string[]
				{
					key.ToString().Replace("System.Collections.Generic.", ""),
					value.ItemsCapacity.FormatNumberShort(),
					value.ItemsInStack.FormatNumberShort(),
					value.ItemsInUse.FormatNumberShort(),
					value.ItemsTaken.FormatNumberShort(),
					value.ItemsCreated.FormatNumberShort(),
					value.ItemsSpilled.FormatNumberShort()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x0018AE18 File Offset: 0x00189018
		[ServerVar]
		[ClientVar]
		public static void print_arraypool(ConsoleSystem.Arg arg)
		{
			ArrayPool<byte> arrayPool = BaseNetwork.ArrayPool;
			ConcurrentQueue<byte[]>[] buffer = arrayPool.GetBuffer();
			TextTable textTable = new TextTable();
			textTable.AddColumn("index");
			textTable.AddColumn("size");
			textTable.AddColumn("bytes");
			textTable.AddColumn("count");
			textTable.AddColumn("memory");
			for (int i = 0; i < buffer.Length; i++)
			{
				int num = arrayPool.IndexToSize(i);
				int count = buffer[i].Count;
				int num2 = num * count;
				textTable.AddRow(new string[]
				{
					i.ToString(),
					num.ToString(),
					num.FormatBytes(false),
					count.ToString(),
					num2.FormatBytes(false)
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x0018AEF8 File Offset: 0x001890F8
		[ServerVar]
		[ClientVar]
		public static void print_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text2.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[]
					{
						text,
						Path.GetFileNameWithoutExtension(text2),
						text3
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042BF RID: 17087 RVA: 0x0018B028 File Offset: 0x00189228
		[ServerVar]
		[ClientVar]
		public static void print_assets(ConsoleSystem.Arg arg)
		{
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("allocated");
			textTable.AddColumn("available");
			foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = keyValuePair.Value.allocated.ToString();
				string text3 = keyValuePair.Value.available.ToString();
				if (string.IsNullOrEmpty(@string) || text.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[] { text, text2, text3 });
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042C0 RID: 17088 RVA: 0x0018B144 File Offset: 0x00189344
		[ServerVar]
		[ClientVar]
		public static void clear_memory(ConsoleSystem.Arg arg)
		{
			Pool.Clear(arg.GetString(0, string.Empty));
		}

		// Token: 0x060042C1 RID: 17089 RVA: 0x0018B158 File Offset: 0x00189358
		[ServerVar]
		[ClientVar]
		public static void clear_prefabs(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, string.Empty);
			GameManager.server.pool.Clear(@string);
		}

		// Token: 0x060042C2 RID: 17090 RVA: 0x0018B182 File Offset: 0x00189382
		[ServerVar]
		[ClientVar]
		public static void clear_assets(ConsoleSystem.Arg arg)
		{
			AssetPool.Clear(arg.GetString(0, string.Empty));
		}

		// Token: 0x060042C3 RID: 17091 RVA: 0x0018B198 File Offset: 0x00189398
		[ServerVar]
		[ClientVar]
		public static void export_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text2.Contains(@string, CompareOptions.IgnoreCase))
				{
					stringBuilder.AppendLine(string.Format("{0},{1},{2}", text, Path.GetFileNameWithoutExtension(text2), text3));
				}
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}

		// Token: 0x060042C4 RID: 17092 RVA: 0x0018B294 File Offset: 0x00189494
		[ServerVar]
		[ClientVar]
		public static void fill_prefabs(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, string.Empty);
			int @int = arg.GetInt(1, 0);
			PrefabPoolWarmup.Run(@string, @int);
		}

		// Token: 0x04003BFF RID: 15359
		[ServerVar]
		[ClientVar]
		public static int mode = 2;

		// Token: 0x04003C00 RID: 15360
		[ServerVar]
		[ClientVar]
		public static bool prewarm = true;

		// Token: 0x04003C01 RID: 15361
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003C02 RID: 15362
		[ServerVar]
		[ClientVar]
		public static bool debug = false;
	}
}
