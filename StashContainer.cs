using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D9 RID: 217
public class StashContainer : StorageContainer
{
	// Token: 0x0600132C RID: 4908 RVA: 0x0009A098 File Offset: 0x00098298
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StashContainer.OnRpcMessage", 0))
		{
			if (rpc == 4130263076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_HideStash ");
				}
				using (TimeWarning.New("RPC_HideStash", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4130263076U, "RPC_HideStash", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_HideStash(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_HideStash");
					}
				}
				return true;
			}
			if (rpc == 298671803U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsUnhide ");
				}
				using (TimeWarning.New("RPC_WantsUnhide", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(298671803U, "RPC_WantsUnhide", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsUnhide(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_WantsUnhide");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool IsHidden()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x0009A398 File Offset: 0x00098598
	public bool PlayerInRange(BasePlayer ply)
	{
		if (Vector3.Distance(base.transform.position, ply.transform.position) <= this.uncoverRange)
		{
			Vector3 normalized = (base.transform.position - ply.eyes.position).normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x0009A407 File Offset: 0x00098607
	public override void InitShared()
	{
		base.InitShared();
		this.visuals.transform.localPosition = this.visuals.transform.localPosition.WithY(this.raisedOffset);
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x0009A43C File Offset: 0x0009863C
	public void DoOccludedCheck()
	{
		if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + Vector3.up * 5f, Vector3.down), 0.25f, 5f, 2097152))
		{
			base.DropItems(null);
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x0009A496 File Offset: 0x00098696
	public void OnPhysicsNeighbourChanged()
	{
		if (!base.IsInvoking(new Action(this.DoOccludedCheck)))
		{
			base.Invoke(new Action(this.DoOccludedCheck), UnityEngine.Random.Range(5f, 10f));
		}
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x0009A4D0 File Offset: 0x000986D0
	public void SetHidden(bool isHidden)
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastToggleTime < 3f)
		{
			return;
		}
		if (isHidden == base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.Invoke(new Action(this.Decay), 259200f);
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, isHidden, false, true);
		}
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x0009A538 File Offset: 0x00098738
	public void DisableNetworking()
	{
		base.limitNetworking = true;
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x00003384 File Offset: 0x00001584
	public void Decay()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x0009A54C File Offset: 0x0009874C
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetHidden(false);
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x0009A55B File Offset: 0x0009875B
	public void ToggleHidden()
	{
		this.SetHidden(!this.IsHidden());
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x0009A56C File Offset: 0x0009876C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_HideStash(BaseEntity.RPCMessage rpc)
	{
		Analytics.Azure.OnStashHidden(rpc.player, this);
		this.SetHidden(true);
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x0009A584 File Offset: 0x00098784
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_WantsUnhide(BaseEntity.RPCMessage rpc)
	{
		if (!this.IsHidden())
		{
			return;
		}
		BasePlayer player = rpc.player;
		if (this.PlayerInRange(player))
		{
			Analytics.Azure.OnStashRevealed(rpc.player, this);
			this.SetHidden(false);
		}
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x0009A5C0 File Offset: 0x000987C0
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = (old & BaseEntity.Flags.Reserved5) == BaseEntity.Flags.Reserved5;
		bool flag2 = (next & BaseEntity.Flags.Reserved5) == BaseEntity.Flags.Reserved5;
		if (flag != flag2)
		{
			float num = (flag2 ? this.burriedOffset : this.raisedOffset);
			LeanTween.cancel(this.visuals.gameObject);
			LeanTween.moveLocalY(this.visuals.gameObject, num, 1f);
		}
	}

	// Token: 0x04000C00 RID: 3072
	public Transform visuals;

	// Token: 0x04000C01 RID: 3073
	public float burriedOffset;

	// Token: 0x04000C02 RID: 3074
	public float raisedOffset;

	// Token: 0x04000C03 RID: 3075
	public GameObjectRef buryEffect;

	// Token: 0x04000C04 RID: 3076
	public float uncoverRange = 3f;

	// Token: 0x04000C05 RID: 3077
	private float lastToggleTime;

	// Token: 0x02000C17 RID: 3095
	public static class StashContainerFlags
	{
		// Token: 0x04004267 RID: 16999
		public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
	}
}
