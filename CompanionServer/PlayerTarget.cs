using System;

namespace CompanionServer
{
	// Token: 0x020009F2 RID: 2546
	public readonly struct PlayerTarget : IEquatable<PlayerTarget>
	{
		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06003CC3 RID: 15555 RVA: 0x001649F5 File Offset: 0x00162BF5
		public ulong SteamId { get; }

		// Token: 0x06003CC4 RID: 15556 RVA: 0x001649FD File Offset: 0x00162BFD
		public PlayerTarget(ulong steamId)
		{
			this.SteamId = steamId;
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x00164A06 File Offset: 0x00162C06
		public bool Equals(PlayerTarget other)
		{
			return this.SteamId == other.SteamId;
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x00164A18 File Offset: 0x00162C18
		public override bool Equals(object obj)
		{
			if (obj is PlayerTarget)
			{
				PlayerTarget playerTarget = (PlayerTarget)obj;
				return this.Equals(playerTarget);
			}
			return false;
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x00164A40 File Offset: 0x00162C40
		public override int GetHashCode()
		{
			return this.SteamId.GetHashCode();
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x00164A5B File Offset: 0x00162C5B
		public static bool operator ==(PlayerTarget left, PlayerTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x00164A65 File Offset: 0x00162C65
		public static bool operator !=(PlayerTarget left, PlayerTarget right)
		{
			return !left.Equals(right);
		}
	}
}
