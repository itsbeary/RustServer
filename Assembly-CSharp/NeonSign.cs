using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A5 RID: 165
public class NeonSign : Signage
{
	// Token: 0x06000F41 RID: 3905 RVA: 0x0008033C File Offset: 0x0007E53C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NeonSign.OnRpcMessage", 0))
		{
			if (rpc == 2433901419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationSpeed ");
				}
				using (TimeWarning.New("SetAnimationSpeed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2433901419U, "SetAnimationSpeed", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2433901419U, "SetAnimationSpeed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetAnimationSpeed(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetAnimationSpeed");
					}
				}
				return true;
			}
			if (rpc == 1919786296U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateNeonColors ");
				}
				using (TimeWarning.New("UpdateNeonColors", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1919786296U, "UpdateNeonColors", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1919786296U, "UpdateNeonColors", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateNeonColors(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UpdateNeonColors");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x00080674 File Offset: 0x0007E874
	public override int ConsumptionAmount()
	{
		return this.powerConsumption;
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0008067C File Offset: 0x0007E87C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.neonSign != null)
		{
			if (this.frameLighting != null)
			{
				foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
				{
					Facepunch.Pool.Free<ProtoBuf.NeonSign.Lights>(ref lights);
				}
				Facepunch.Pool.FreeList<ProtoBuf.NeonSign.Lights>(ref this.frameLighting);
			}
			this.frameLighting = info.msg.neonSign.frameLighting;
			info.msg.neonSign.frameLighting = null;
			this.currentFrame = Mathf.Clamp(info.msg.neonSign.currentFrame, 0, this.paintableSources.Length);
			this.animationSpeed = Mathf.Clamp(info.msg.neonSign.animationSpeed, 0.5f, 5f);
		}
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0008076C File Offset: 0x0007E96C
	public override void ServerInit()
	{
		base.ServerInit();
		this.animationLoopAction = new Action(this.SwitchToNextFrame);
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00080786 File Offset: 0x0007E986
	public override void ResetState()
	{
		base.ResetState();
		base.CancelInvoke(this.animationLoopAction);
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0008079C File Offset: 0x0007E99C
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (this.paintableSources.Length <= 1)
		{
			return;
		}
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		if (flag && !this.isAnimating)
		{
			if (this.currentFrame != 0)
			{
				this.currentFrame = 0;
				base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
			}
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
			this.isAnimating = true;
			return;
		}
		if (!flag && this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			this.isAnimating = false;
		}
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x00080834 File Offset: 0x0007EA34
	private void SwitchToNextFrame()
	{
		int num = this.currentFrame;
		for (int i = 0; i < this.paintableSources.Length; i++)
		{
			this.currentFrame++;
			if (this.currentFrame >= this.paintableSources.Length)
			{
				this.currentFrame = 0;
			}
			if (this.textureIDs[this.currentFrame] != 0U)
			{
				break;
			}
		}
		if (this.currentFrame != num)
		{
			base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x000808AC File Offset: 0x0007EAAC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		List<ProtoBuf.NeonSign.Lights> list = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		if (this.frameLighting != null)
		{
			foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
			{
				list.Add(lights.Copy());
			}
		}
		info.msg.neonSign = Facepunch.Pool.Get<ProtoBuf.NeonSign>();
		info.msg.neonSign.frameLighting = list;
		info.msg.neonSign.currentFrame = this.currentFrame;
		info.msg.neonSign.animationSpeed = this.animationSpeed;
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x00080968 File Offset: 0x0007EB68
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SetAnimationSpeed(global::BaseEntity.RPCMessage msg)
	{
		float num = Mathf.Clamp(msg.read.Float(), 0.5f, 5f);
		this.animationSpeed = num;
		if (this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x000809CC File Offset: 0x0007EBCC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void UpdateNeonColors(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num < 0 || num >= this.paintableSources.Length)
		{
			return;
		}
		this.EnsureInitialized();
		this.frameLighting[num].topLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].topRight = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomRight = global::NeonSign.ClampColor(msg.read.Color());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00080A98 File Offset: 0x0007EC98
	private void EnsureInitialized()
	{
		if (this.frameLighting == null)
		{
			this.frameLighting = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		}
		while (this.frameLighting.Count < this.paintableSources.Length)
		{
			ProtoBuf.NeonSign.Lights lights = Facepunch.Pool.Get<ProtoBuf.NeonSign.Lights>();
			lights.topLeft = Color.clear;
			lights.topRight = Color.clear;
			lights.bottomLeft = Color.clear;
			lights.bottomRight = Color.clear;
			this.frameLighting.Add(lights);
		}
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x00080B0D File Offset: 0x0007ED0D
	private static Color ClampColor(Color color)
	{
		return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
	}

	// Token: 0x04000A02 RID: 2562
	private const float FastSpeed = 0.5f;

	// Token: 0x04000A03 RID: 2563
	private const float MediumSpeed = 1f;

	// Token: 0x04000A04 RID: 2564
	private const float SlowSpeed = 2f;

	// Token: 0x04000A05 RID: 2565
	private const float MinSpeed = 0.5f;

	// Token: 0x04000A06 RID: 2566
	private const float MaxSpeed = 5f;

	// Token: 0x04000A07 RID: 2567
	[Header("Neon Sign")]
	public Light topLeft;

	// Token: 0x04000A08 RID: 2568
	public Light topRight;

	// Token: 0x04000A09 RID: 2569
	public Light bottomLeft;

	// Token: 0x04000A0A RID: 2570
	public Light bottomRight;

	// Token: 0x04000A0B RID: 2571
	public float lightIntensity = 2f;

	// Token: 0x04000A0C RID: 2572
	[Range(1f, 100f)]
	public int powerConsumption = 10;

	// Token: 0x04000A0D RID: 2573
	public Material activeMaterial;

	// Token: 0x04000A0E RID: 2574
	public Material inactiveMaterial;

	// Token: 0x04000A0F RID: 2575
	private float animationSpeed = 1f;

	// Token: 0x04000A10 RID: 2576
	private int currentFrame;

	// Token: 0x04000A11 RID: 2577
	private List<ProtoBuf.NeonSign.Lights> frameLighting;

	// Token: 0x04000A12 RID: 2578
	private bool isAnimating;

	// Token: 0x04000A13 RID: 2579
	private Action animationLoopAction;

	// Token: 0x04000A14 RID: 2580
	public AmbienceEmitter ambientSoundEmitter;

	// Token: 0x04000A15 RID: 2581
	public SoundDefinition switchSoundDef;
}
