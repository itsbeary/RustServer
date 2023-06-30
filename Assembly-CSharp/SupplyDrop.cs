using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200042A RID: 1066
public class SupplyDrop : LootContainer
{
	// Token: 0x0600242C RID: 9260 RVA: 0x000E6DDC File Offset: 0x000E4FDC
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			if (this.parachutePrefab.isValid)
			{
				this.parachute = GameManager.server.CreateEntity(this.parachutePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			}
			if (this.parachute)
			{
				this.parachute.SetParent(this, "parachute_attach", false, false);
				this.parachute.Spawn();
			}
		}
		this.isLootable = false;
		base.Invoke(new Action(this.MakeLootable), 300f);
		base.InvokeRepeating(new Action(this.CheckNightLight), 0f, 30f);
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000E6E96 File Offset: 0x000E5096
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && Rust.Application.isLoadingSave)
		{
			if (this.parachute != null)
			{
				Debug.LogWarning("More than one child entity was added to SupplyDrop! Expected only the parachute.", this);
			}
			this.parachute = child;
		}
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000E6ECE File Offset: 0x000E50CE
	private void RemoveParachute()
	{
		if (this.parachute)
		{
			this.parachute.Kill(BaseNetworkable.DestroyMode.None);
			this.parachute = null;
		}
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000E6EF0 File Offset: 0x000E50F0
	public void MakeLootable()
	{
		this.isLootable = true;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000E6EF9 File Offset: 0x000E50F9
	private void OnCollisionEnter(Collision collision)
	{
		if (((1 << collision.collider.gameObject.layer) & 1084293393) > 0)
		{
			this.RemoveParachute();
			this.MakeLootable();
		}
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000E6F27 File Offset: 0x000E5127
	private void CheckNightLight()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, Env.time > 20f || Env.time < 7f, false, true);
	}

	// Token: 0x04001C2B RID: 7211
	public GameObjectRef parachutePrefab;

	// Token: 0x04001C2C RID: 7212
	private const BaseEntity.Flags FlagNightLight = BaseEntity.Flags.Reserved1;

	// Token: 0x04001C2D RID: 7213
	private BaseEntity parachute;
}
