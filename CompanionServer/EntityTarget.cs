using System;

namespace CompanionServer
{
	// Token: 0x020009EC RID: 2540
	public readonly struct EntityTarget : IEquatable<EntityTarget>
	{
		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06003C9B RID: 15515 RVA: 0x00163E0C File Offset: 0x0016200C
		public NetworkableId EntityId { get; }

		// Token: 0x06003C9C RID: 15516 RVA: 0x00163E14 File Offset: 0x00162014
		public EntityTarget(NetworkableId entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x06003C9D RID: 15517 RVA: 0x00163E1D File Offset: 0x0016201D
		public bool Equals(EntityTarget other)
		{
			return this.EntityId == other.EntityId;
		}

		// Token: 0x06003C9E RID: 15518 RVA: 0x00163E34 File Offset: 0x00162034
		public override bool Equals(object obj)
		{
			if (obj is EntityTarget)
			{
				EntityTarget entityTarget = (EntityTarget)obj;
				return this.Equals(entityTarget);
			}
			return false;
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x00163E5C File Offset: 0x0016205C
		public override int GetHashCode()
		{
			return this.EntityId.GetHashCode();
		}

		// Token: 0x06003CA0 RID: 15520 RVA: 0x00163E7D File Offset: 0x0016207D
		public static bool operator ==(EntityTarget left, EntityTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003CA1 RID: 15521 RVA: 0x00163E87 File Offset: 0x00162087
		public static bool operator !=(EntityTarget left, EntityTarget right)
		{
			return !left.Equals(right);
		}
	}
}
