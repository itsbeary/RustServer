using System;

namespace UnityEngine
{
	// Token: 0x02000A26 RID: 2598
	public static class ArgEx
	{
		// Token: 0x06003D9A RID: 15770 RVA: 0x00169346 File Offset: 0x00167546
		public static BasePlayer Player(this ConsoleSystem.Arg arg)
		{
			if (arg == null || arg.Connection == null)
			{
				return null;
			}
			return arg.Connection.player as BasePlayer;
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x00169368 File Offset: 0x00167568
		public static BasePlayer GetPlayer(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, null);
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.Find(@string);
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x0016938C File Offset: 0x0016758C
		public static BasePlayer GetSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindSleeping(@string);
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x001693B4 File Offset: 0x001675B4
		public static BasePlayer GetPlayerOrSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindAwakeOrSleeping(@string);
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x001693DC File Offset: 0x001675DC
		public static BasePlayer GetPlayerOrSleeperOrBot(this ConsoleSystem.Arg arg, int iArgNum)
		{
			uint num;
			if (arg.TryGetUInt(iArgNum, out num))
			{
				return BasePlayer.FindBot((ulong)num);
			}
			return arg.GetPlayerOrSleeper(iArgNum);
		}

		// Token: 0x06003D9F RID: 15775 RVA: 0x00169403 File Offset: 0x00167603
		public static NetworkableId GetEntityID(this ConsoleSystem.Arg arg, int iArg, NetworkableId def = default(NetworkableId))
		{
			return new NetworkableId(arg.GetUInt64(iArg, def.Value));
		}

		// Token: 0x06003DA0 RID: 15776 RVA: 0x00169417 File Offset: 0x00167617
		public static ItemId GetItemID(this ConsoleSystem.Arg arg, int iArg, ItemId def = default(ItemId))
		{
			return new ItemId(arg.GetUInt64(iArg, def.Value));
		}
	}
}
