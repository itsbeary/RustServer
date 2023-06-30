using System;
using Facepunch;
using Network;
using ProtoBuf;
using Rust.UI;
using Sonar;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class Tugboat : MotorRowboat
{
	// Token: 0x06001477 RID: 5239 RVA: 0x000A1790 File Offset: 0x0009F990
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Tugboat.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x06001478 RID: 5240 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x000A17D0 File Offset: 0x0009F9D0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x000A17DC File Offset: 0x0009F9DC
	public override void VehicleFixedUpdate()
	{
		int fuelAmount = this.fuelSystem.GetFuelAmount();
		base.VehicleFixedUpdate();
		int fuelAmount2 = this.fuelSystem.GetFuelAmount();
		if (fuelAmount2 != fuelAmount)
		{
			base.ClientRPC<int>(null, "SetFuelAmount", fuelAmount2);
		}
		if (this.LightsAreOn && !base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
		}
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x000A183B File Offset: 0x0009FA3B
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUint = Pool.Get<SimpleUInt>();
		info.msg.simpleUint.value = (uint)this.fuelSystem.GetFuelAmount();
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int StartingFuelUnits()
	{
		return 0;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x000A186F File Offset: 0x0009FA6F
	public override void LightToggle(global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x000A18AD File Offset: 0x0009FAAD
	protected override void EnterCorpseState()
	{
		base.Invoke(new Action(base.ActualDeath), Tugboat.tugboat_corpse_seconds);
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceDeployableSetParent()
	{
		return true;
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x000A18C6 File Offset: 0x0009FAC6
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return !(pusher.GetParentEntity() == this) && base.CanPushNow(pusher);
	}

	// Token: 0x04000CDF RID: 3295
	[Header("Tugboat")]
	[SerializeField]
	private Canvas monitorCanvas;

	// Token: 0x04000CE0 RID: 3296
	[SerializeField]
	private RustText fuelText;

	// Token: 0x04000CE1 RID: 3297
	[SerializeField]
	private RustText speedText;

	// Token: 0x04000CE2 RID: 3298
	[SerializeField]
	private ParticleSystemContainer exhaustEffect;

	// Token: 0x04000CE3 RID: 3299
	[SerializeField]
	private SoundDefinition lightsToggleSound;

	// Token: 0x04000CE4 RID: 3300
	[SerializeField]
	private Transform steeringWheelLeftHandTarget;

	// Token: 0x04000CE5 RID: 3301
	[SerializeField]
	private Transform steeringWheelRightHandTarget;

	// Token: 0x04000CE6 RID: 3302
	[SerializeField]
	private SonarSystem sonar;

	// Token: 0x04000CE7 RID: 3303
	[SerializeField]
	private TugboatSounds tugboatSounds;

	// Token: 0x04000CE8 RID: 3304
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04000CE9 RID: 3305
	[SerializeField]
	private EmissionToggle emissionToggle;

	// Token: 0x04000CEA RID: 3306
	[SerializeField]
	private AnimationCurve emissionCurve;

	// Token: 0x04000CEB RID: 3307
	[ServerVar]
	[Help("how long until boat corpses despawn (excluding tugboat)")]
	public static float tugboat_corpse_seconds = 7200f;
}
