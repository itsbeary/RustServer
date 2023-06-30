using System;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
public class ColliderInfo : MonoBehaviour
{
	// Token: 0x0600292C RID: 10540 RVA: 0x000FDA42 File Offset: 0x000FBC42
	public bool HasFlag(ColliderInfo.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000FDA4F File Offset: 0x000FBC4F
	public void SetFlag(ColliderInfo.Flags f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000FDA74 File Offset: 0x000FBC74
	public bool Filter(HitTest info)
	{
		switch (info.type)
		{
		case HitTest.Type.ProjectileEffect:
		case HitTest.Type.Projectile:
			if ((this.flags & ColliderInfo.Flags.Shootable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.MeleeAttack:
			if ((this.flags & ColliderInfo.Flags.Melee) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.Use:
			if ((this.flags & ColliderInfo.Flags.Usable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x04002143 RID: 8515
	public const ColliderInfo.Flags FlagsNone = (ColliderInfo.Flags)0;

	// Token: 0x04002144 RID: 8516
	public const ColliderInfo.Flags FlagsEverything = (ColliderInfo.Flags)(-1);

	// Token: 0x04002145 RID: 8517
	public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x04002146 RID: 8518
	[InspectorFlags]
	public ColliderInfo.Flags flags = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x02000D47 RID: 3399
	[Flags]
	public enum Flags
	{
		// Token: 0x04004754 RID: 18260
		Usable = 1,
		// Token: 0x04004755 RID: 18261
		Shootable = 2,
		// Token: 0x04004756 RID: 18262
		Melee = 4,
		// Token: 0x04004757 RID: 18263
		Opaque = 8,
		// Token: 0x04004758 RID: 18264
		Airflow = 16,
		// Token: 0x04004759 RID: 18265
		OnlyBlockBuildingBlock = 32
	}
}
