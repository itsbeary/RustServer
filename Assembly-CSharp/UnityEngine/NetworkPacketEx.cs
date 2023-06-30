using System;
using Network;

namespace UnityEngine
{
	// Token: 0x02000A2D RID: 2605
	public static class NetworkPacketEx
	{
		// Token: 0x06003DB6 RID: 15798 RVA: 0x0016978C File Offset: 0x0016798C
		public static BasePlayer Player(this Message v)
		{
			if (v.connection == null)
			{
				return null;
			}
			return v.connection.player as BasePlayer;
		}
	}
}
