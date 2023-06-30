using System;

namespace CompanionServer
{
	// Token: 0x020009E7 RID: 2535
	public readonly struct CameraTarget : IEquatable<CameraTarget>
	{
		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06003C6D RID: 15469 RVA: 0x00163008 File Offset: 0x00161208
		public NetworkableId EntityId { get; }

		// Token: 0x06003C6E RID: 15470 RVA: 0x00163010 File Offset: 0x00161210
		public CameraTarget(NetworkableId entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x06003C6F RID: 15471 RVA: 0x00163019 File Offset: 0x00161219
		public bool Equals(CameraTarget other)
		{
			return this.EntityId == other.EntityId;
		}

		// Token: 0x06003C70 RID: 15472 RVA: 0x00163030 File Offset: 0x00161230
		public override bool Equals(object obj)
		{
			if (obj is CameraTarget)
			{
				CameraTarget cameraTarget = (CameraTarget)obj;
				return this.Equals(cameraTarget);
			}
			return false;
		}

		// Token: 0x06003C71 RID: 15473 RVA: 0x00163058 File Offset: 0x00161258
		public override int GetHashCode()
		{
			return this.EntityId.GetHashCode();
		}

		// Token: 0x06003C72 RID: 15474 RVA: 0x00163079 File Offset: 0x00161279
		public static bool operator ==(CameraTarget left, CameraTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003C73 RID: 15475 RVA: 0x00163083 File Offset: 0x00161283
		public static bool operator !=(CameraTarget left, CameraTarget right)
		{
			return !left.Equals(right);
		}
	}
}
