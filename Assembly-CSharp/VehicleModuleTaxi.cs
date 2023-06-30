using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EC RID: 236
public class VehicleModuleTaxi : VehicleModuleStorage
{
	// Token: 0x060014BE RID: 5310 RVA: 0x000A3264 File Offset: 0x000A1464
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleTaxi.OnRpcMessage", 0))
		{
			if (rpc == 2714639811U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_KickPassengers ");
				}
				using (TimeWarning.New("RPC_KickPassengers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2714639811U, "RPC_KickPassengers", this, player, 3f))
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
							this.RPC_KickPassengers(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_KickPassengers");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060014BF RID: 5311 RVA: 0x000A33CC File Offset: 0x000A15CC
	private Vector3 KickButtonPos
	{
		get
		{
			return this.kickButtonCollider.transform.position + this.kickButtonCollider.transform.rotation * this.kickButtonCollider.center;
		}
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x000A3404 File Offset: 0x000A1604
	private bool CanKickPassengers(BasePlayer player)
	{
		if (!base.IsOnAVehicle)
		{
			return false;
		}
		if (base.Vehicle.GetSpeed() > this.maxKickVelocity)
		{
			return false;
		}
		if (player == null)
		{
			return false;
		}
		if (!base.Vehicle.PlayerIsMounted(player))
		{
			return false;
		}
		Vector3 vector = this.KickButtonPos - player.transform.position;
		return Vector3.Dot(vector, player.transform.forward) < 0f && vector.sqrMagnitude < 4f;
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x000A348C File Offset: 0x000A168C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_KickPassengers(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanKickPassengers(player))
		{
			return;
		}
		this.KickPassengers();
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000A34BC File Offset: 0x000A16BC
	private void KickPassengers()
	{
		if (!base.IsOnAVehicle)
		{
			return;
		}
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			BaseMountable mountable = mountPointInfo.mountable;
			BasePlayer mounted = mountable.GetMounted();
			if (mounted != null && mountable.HasValidDismountPosition(mounted))
			{
				mountable.AttemptDismount(mounted);
			}
		}
	}

	// Token: 0x04000D0F RID: 3343
	[Header("Taxi")]
	[SerializeField]
	private SoundDefinition kickButtonSound;

	// Token: 0x04000D10 RID: 3344
	[SerializeField]
	private SphereCollider kickButtonCollider;

	// Token: 0x04000D11 RID: 3345
	[SerializeField]
	private float maxKickVelocity = 4f;
}
