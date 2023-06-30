using System;

// Token: 0x02000105 RID: 261
public struct CameraViewerId : IEquatable<CameraViewerId>
{
	// Token: 0x060015CA RID: 5578 RVA: 0x000ABAB9 File Offset: 0x000A9CB9
	public CameraViewerId(ulong steamId, long connectionId)
	{
		this.SteamId = steamId;
		this.ConnectionId = connectionId;
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000ABAC9 File Offset: 0x000A9CC9
	public bool Equals(CameraViewerId other)
	{
		return this.SteamId == other.SteamId && this.ConnectionId == other.ConnectionId;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x000ABAEC File Offset: 0x000A9CEC
	public override bool Equals(object obj)
	{
		if (obj is CameraViewerId)
		{
			CameraViewerId cameraViewerId = (CameraViewerId)obj;
			return this.Equals(cameraViewerId);
		}
		return false;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000ABB14 File Offset: 0x000A9D14
	public override int GetHashCode()
	{
		return (this.SteamId.GetHashCode() * 397) ^ this.ConnectionId.GetHashCode();
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000ABB44 File Offset: 0x000A9D44
	public static bool operator ==(CameraViewerId left, CameraViewerId right)
	{
		return left.Equals(right);
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x000ABB4E File Offset: 0x000A9D4E
	public static bool operator !=(CameraViewerId left, CameraViewerId right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000DF7 RID: 3575
	public readonly ulong SteamId;

	// Token: 0x04000DF8 RID: 3576
	public readonly long ConnectionId;
}
