using System;

// Token: 0x020003A6 RID: 934
public class MobileInventoryEntity : BaseEntity
{
	// Token: 0x060020D5 RID: 8405 RVA: 0x000D8A21 File Offset: 0x000D6C21
	public void ToggleRinging(bool state)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, state, false, true);
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000D8A31 File Offset: 0x000D6C31
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(MobileInventoryEntity.Flag_Silent, wantsSilent, false, true);
	}

	// Token: 0x040019C1 RID: 6593
	public SoundDefinition ringingLoop;

	// Token: 0x040019C2 RID: 6594
	public SoundDefinition silentLoop;

	// Token: 0x040019C3 RID: 6595
	public const BaseEntity.Flags Ringing = BaseEntity.Flags.Reserved1;

	// Token: 0x040019C4 RID: 6596
	public static BaseEntity.Flags Flag_Silent = BaseEntity.Flags.Reserved2;
}
