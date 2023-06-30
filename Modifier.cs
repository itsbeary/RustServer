using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000437 RID: 1079
public class Modifier
{
	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06002481 RID: 9345 RVA: 0x000E88D5 File Offset: 0x000E6AD5
	// (set) Token: 0x06002482 RID: 9346 RVA: 0x000E88DD File Offset: 0x000E6ADD
	public global::Modifier.ModifierType Type { get; private set; }

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06002483 RID: 9347 RVA: 0x000E88E6 File Offset: 0x000E6AE6
	// (set) Token: 0x06002484 RID: 9348 RVA: 0x000E88EE File Offset: 0x000E6AEE
	public global::Modifier.ModifierSource Source { get; private set; }

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06002485 RID: 9349 RVA: 0x000E88F7 File Offset: 0x000E6AF7
	// (set) Token: 0x06002486 RID: 9350 RVA: 0x000E88FF File Offset: 0x000E6AFF
	public float Value { get; private set; } = 1f;

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06002487 RID: 9351 RVA: 0x000E8908 File Offset: 0x000E6B08
	// (set) Token: 0x06002488 RID: 9352 RVA: 0x000E8910 File Offset: 0x000E6B10
	public float Duration { get; private set; } = 10f;

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06002489 RID: 9353 RVA: 0x000E8919 File Offset: 0x000E6B19
	// (set) Token: 0x0600248A RID: 9354 RVA: 0x000E8921 File Offset: 0x000E6B21
	public float TimeRemaining { get; private set; }

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x0600248B RID: 9355 RVA: 0x000E892A File Offset: 0x000E6B2A
	// (set) Token: 0x0600248C RID: 9356 RVA: 0x000E8932 File Offset: 0x000E6B32
	public bool Expired { get; private set; }

	// Token: 0x0600248D RID: 9357 RVA: 0x000E893B File Offset: 0x000E6B3B
	public void Init(global::Modifier.ModifierType type, global::Modifier.ModifierSource source, float value, float duration, float remaining)
	{
		this.Type = type;
		this.Source = source;
		this.Value = value;
		this.Duration = duration;
		this.Expired = false;
		this.TimeRemaining = remaining;
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000E8969 File Offset: 0x000E6B69
	public void Tick(BaseCombatEntity ownerEntity, float delta)
	{
		this.TimeRemaining -= delta;
		this.Expired = this.TimeRemaining <= 0f;
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000E898F File Offset: 0x000E6B8F
	public ProtoBuf.Modifier Save()
	{
		ProtoBuf.Modifier modifier = Pool.Get<ProtoBuf.Modifier>();
		modifier.type = (int)this.Type;
		modifier.source = (int)this.Source;
		modifier.value = this.Value;
		modifier.timeRemaing = this.TimeRemaining;
		return modifier;
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000E89C6 File Offset: 0x000E6BC6
	public void Load(ProtoBuf.Modifier m)
	{
		this.Type = (global::Modifier.ModifierType)m.type;
		this.Source = (global::Modifier.ModifierSource)m.source;
		this.Value = m.value;
		this.TimeRemaining = m.timeRemaing;
	}

	// Token: 0x02000CFB RID: 3323
	public enum ModifierType
	{
		// Token: 0x0400462E RID: 17966
		Wood_Yield,
		// Token: 0x0400462F RID: 17967
		Ore_Yield,
		// Token: 0x04004630 RID: 17968
		Radiation_Resistance,
		// Token: 0x04004631 RID: 17969
		Radiation_Exposure_Resistance,
		// Token: 0x04004632 RID: 17970
		Max_Health,
		// Token: 0x04004633 RID: 17971
		Scrap_Yield
	}

	// Token: 0x02000CFC RID: 3324
	public enum ModifierSource
	{
		// Token: 0x04004635 RID: 17973
		Tea
	}
}
