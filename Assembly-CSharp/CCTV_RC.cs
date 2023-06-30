using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000058 RID: 88
public class CCTV_RC : PoweredRemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	// Token: 0x0600097E RID: 2430 RVA: 0x00059A30 File Offset: 0x00057C30
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CCTV_RC.OnRpcMessage", 0))
		{
			if (rpc == 3353964129U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetDir ");
				}
				using (TimeWarning.New("Server_SetDir", 0))
				{
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
							this.Server_SetDir(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_SetDir");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000037BE File Offset: 0x000019BE
	public override int ConsumptionAmount()
	{
		return 3;
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000980 RID: 2432 RVA: 0x00059B54 File Offset: 0x00057D54
	public override bool RequiresMouse
	{
		get
		{
			return this.hasPTZ;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000981 RID: 2433 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool EntityCanPing
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000982 RID: 2434 RVA: 0x00059B54 File Offset: 0x00057D54
	public override bool CanAcceptInput
	{
		get
		{
			return this.hasPTZ;
		}
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00059B5C File Offset: 0x00057D5C
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (base.IsStatic())
		{
			this.pitchAmount = this.pitch.localEulerAngles.x;
			this.yawAmount = this.yaw.localEulerAngles.y;
			base.UpdateRCAccess(true);
		}
		this.timeSinceLastServerTick = 0.0;
		base.InvokeRandomized(new Action(this.ServerTick), UnityEngine.Random.Range(0f, 1f), 0.015f, 0.01f);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00059BF2 File Offset: 0x00057DF2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateRotation(10000f);
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00059C05 File Offset: 0x00057E05
	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (this.UpdateManualAim(inputState))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00059C18 File Offset: 0x00057E18
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.rcEntity == null)
		{
			info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		}
		info.msg.rcEntity.aim.x = this.pitchAmount;
		info.msg.rcEntity.aim.y = this.yawAmount;
		info.msg.rcEntity.aim.z = 0f;
		info.msg.rcEntity.zoom = (float)this.fovScaleIndex;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00059CB0 File Offset: 0x00057EB0
	[global::BaseEntity.RPC_Server]
	public void Server_SetDir(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsStatic())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (!player.CanBuild() || !player.IsBuildingAuthed())
		{
			return;
		}
		Vector3 vector = Vector3Ex.Direction(player.eyes.position, this.yaw.transform.position);
		vector = base.transform.InverseTransformDirection(vector);
		Vector3 vector2 = BaseMountable.ConvertVector(Quaternion.LookRotation(vector).eulerAngles);
		this.pitchAmount = Mathf.Clamp(vector2.x, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(vector2.y, this.yawClamp.x, this.yawClamp.y);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00059D75 File Offset: 0x00057F75
	public override bool InitializeControl(CameraViewerId viewerID)
	{
		bool flag = base.InitializeControl(viewerID);
		this.UpdateViewers();
		return flag;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00059D84 File Offset: 0x00057F84
	public override void StopControl(CameraViewerId viewerID)
	{
		base.StopControl(viewerID);
		this.UpdateViewers();
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00059D93 File Offset: 0x00057F93
	public void UpdateViewers()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved5, base.ViewerCount > 0, false, true);
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00059DAC File Offset: 0x00057FAC
	public void ServerTick()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		float num = (float)this.timeSinceLastServerTick;
		this.timeSinceLastServerTick = 0.0;
		this.UpdateRotation(num);
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00059DF4 File Offset: 0x00057FF4
	private bool UpdateManualAim(InputState inputState)
	{
		if (!this.hasPTZ)
		{
			return false;
		}
		float num = -inputState.current.mouseDelta.y;
		float x = inputState.current.mouseDelta.x;
		bool flag = inputState.WasJustPressed(BUTTON.FIRE_PRIMARY);
		this.pitchAmount = Mathf.Clamp(this.pitchAmount + num * this.turnSpeed, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(this.yawAmount + x * this.turnSpeed, this.yawClamp.x, this.yawClamp.y) % 360f;
		if (flag)
		{
			this.fovScaleIndex = (this.fovScaleIndex + 1) % this.fovScales.Length;
		}
		return num != 0f || x != 0f || flag;
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00059ED4 File Offset: 0x000580D4
	public void UpdateRotation(float delta)
	{
		Quaternion quaternion = Quaternion.Euler(this.pitchAmount, 0f, 0f);
		Quaternion quaternion2 = Quaternion.Euler(0f, this.yawAmount, 0f);
		float num = ((base.isServer && !base.IsBeingControlled) ? this.serverLerpSpeed : this.clientLerpSpeed);
		this.pitch.transform.localRotation = Mathx.Lerp(this.pitch.transform.localRotation, quaternion, num, delta);
		this.yaw.transform.localRotation = Mathx.Lerp(this.yaw.transform.localRotation, quaternion2, num, delta);
		if (this.fovScales == null || this.fovScales.Length == 0)
		{
			this.fovScaleLerped = 1f;
			return;
		}
		if (this.fovScales.Length > 1)
		{
			this.fovScaleLerped = Mathx.Lerp(this.fovScaleLerped, this.fovScales[this.fovScaleIndex], this.zoomLerpSpeed, delta);
			return;
		}
		this.fovScaleLerped = this.fovScales[0];
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00059FD8 File Offset: 0x000581D8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null)
		{
			int num = Mathf.Clamp((int)info.msg.rcEntity.zoom, 0, this.fovScales.Length - 1);
			if (base.isServer)
			{
				this.pitchAmount = info.msg.rcEntity.aim.x;
				this.yawAmount = info.msg.rcEntity.aim.y;
				this.fovScaleIndex = num;
			}
		}
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0005A060 File Offset: 0x00058260
	public override float GetFovScale()
	{
		return this.fovScaleLerped;
	}

	// Token: 0x04000638 RID: 1592
	public Transform pivotOrigin;

	// Token: 0x04000639 RID: 1593
	public Transform yaw;

	// Token: 0x0400063A RID: 1594
	public Transform pitch;

	// Token: 0x0400063B RID: 1595
	public Vector2 pitchClamp = new Vector2(-50f, 50f);

	// Token: 0x0400063C RID: 1596
	public Vector2 yawClamp = new Vector2(-50f, 50f);

	// Token: 0x0400063D RID: 1597
	public float turnSpeed = 25f;

	// Token: 0x0400063E RID: 1598
	public float serverLerpSpeed = 15f;

	// Token: 0x0400063F RID: 1599
	public float clientLerpSpeed = 10f;

	// Token: 0x04000640 RID: 1600
	public float zoomLerpSpeed = 10f;

	// Token: 0x04000641 RID: 1601
	public float[] fovScales;

	// Token: 0x04000642 RID: 1602
	private float pitchAmount;

	// Token: 0x04000643 RID: 1603
	private float yawAmount;

	// Token: 0x04000644 RID: 1604
	private int fovScaleIndex;

	// Token: 0x04000645 RID: 1605
	private float fovScaleLerped = 1f;

	// Token: 0x04000646 RID: 1606
	public bool hasPTZ = true;

	// Token: 0x04000647 RID: 1607
	public AnimationCurve dofCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04000648 RID: 1608
	public float dofApertureMax = 10f;

	// Token: 0x04000649 RID: 1609
	public const global::BaseEntity.Flags Flag_HasViewer = global::BaseEntity.Flags.Reserved5;

	// Token: 0x0400064A RID: 1610
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x0400064B RID: 1611
	public AnimationCurve movementLoopGainCurve;

	// Token: 0x0400064C RID: 1612
	public float movementLoopSmoothing = 1f;

	// Token: 0x0400064D RID: 1613
	public float movementLoopReference = 50f;

	// Token: 0x0400064E RID: 1614
	private Sound movementLoop;

	// Token: 0x0400064F RID: 1615
	private SoundModulation.Modulator movementLoopGainModulator;

	// Token: 0x04000650 RID: 1616
	public SoundDefinition zoomInSoundDef;

	// Token: 0x04000651 RID: 1617
	public SoundDefinition zoomOutSoundDef;

	// Token: 0x04000652 RID: 1618
	private RealTimeSinceEx timeSinceLastServerTick;
}
