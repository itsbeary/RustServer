using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C4 RID: 196
public class RHIB : MotorRowboat
{
	// Token: 0x06001182 RID: 4482 RVA: 0x0008ECAC File Offset: 0x0008CEAC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RHIB.OnRpcMessage", 0))
		{
			if (rpc == 1382282393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Release ");
				}
				using (TimeWarning.New("Server_Release", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1382282393U, "Server_Release", this, player, 6f))
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
							this.Server_Release(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Release");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0008EE14 File Offset: 0x0008D014
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(6f)]
	public void Server_Release(BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() == null)
		{
			return;
		}
		base.SetParent(null, true, true);
		base.SetToNonKinematic();
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0008EE34 File Offset: 0x0008D034
	public override void VehicleFixedUpdate()
	{
		this.gasPedal = Mathf.MoveTowards(this.gasPedal, this.targetGasPedal, UnityEngine.Time.fixedDeltaTime * 1f);
		base.VehicleFixedUpdate();
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0008EE5E File Offset: 0x0008D05E
	public override bool EngineOn()
	{
		return base.EngineOn();
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0008EE68 File Offset: 0x0008D068
	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.targetGasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.targetGasPedal = -0.5f;
		}
		else
		{
			this.targetGasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x0008EEEC File Offset: 0x0008D0EC
	public void AddFuel(int amount)
	{
		StorageContainer storageContainer = this.fuelSystem.fuelStorageInstance.Get(true);
		if (storageContainer)
		{
			storageContainer.GetComponent<StorageContainer>().inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), amount, 0UL, ItemContainer.LimitStack.Existing);
		}
	}

	// Token: 0x04000AE1 RID: 2785
	public Transform steeringWheelLeftHandTarget;

	// Token: 0x04000AE2 RID: 2786
	public Transform steeringWheelRightHandTarget;

	// Token: 0x04000AE3 RID: 2787
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float rhibpopulation;

	// Token: 0x04000AE4 RID: 2788
	private float targetGasPedal;
}
