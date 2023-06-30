using System;
using Facepunch;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000516 RID: 1302
public class GameModeCapturePoint : global::BaseEntity
{
	// Token: 0x060029AF RID: 10671 RVA: 0x0002A749 File Offset: 0x00028949
	public bool IsContested()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000FF973 File Offset: 0x000FDB73
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.AssignPoints), 0f, 1f);
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000FF997 File Offset: 0x000FDB97
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateCaptureAmount();
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000FF9A8 File Offset: 0x000FDBA8
	public void AssignPoints()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			if (this.captureTeam != -1 && this.captureFraction == 1f)
			{
				activeGameMode.ModifyTeamScore(this.captureTeam, this.scorePerSecond);
				return;
			}
		}
		else if (this.capturedPlayer.IsValid(true))
		{
			activeGameMode.ModifyPlayerGameScore(this.capturedPlayer.Get(true).GetComponent<global::BasePlayer>(), "score", this.scorePerSecond);
		}
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000FFA30 File Offset: 0x000FDC30
	public void DoCaptureEffect()
	{
		Effect.server.Run(this.progressCompleteEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000FFA64 File Offset: 0x000FDC64
	public void DoProgressEffect()
	{
		if (Time.time < this.nextBeepTime)
		{
			return;
		}
		Effect.server.Run(this.progressBeepEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
		this.nextBeepTime = Time.time + 0.5f;
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000FFAB8 File Offset: 0x000FDCB8
	public void UpdateCaptureAmount()
	{
		if (base.isClient)
		{
			return;
		}
		float num = this.captureFraction;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (this.captureTrigger.entityContents == null)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			int[] array = new int[activeGameMode.GetNumTeams()];
			foreach (global::BaseEntity baseEntity in this.captureTrigger.entityContents)
			{
				if (!(baseEntity == null) && !baseEntity.isClient)
				{
					global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
					if (!(component == null) && component.IsAlive() && !component.IsNpc && component.gamemodeteam != -1)
					{
						array[component.gamemodeteam]++;
					}
				}
			}
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
				{
					num2++;
				}
			}
			if (num2 < 2)
			{
				int num3 = -1;
				int num4 = 0;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] > num4)
					{
						num4 = array[j];
						num3 = j;
					}
				}
				if (this.captureTeam == -1 && this.captureFraction == 0f)
				{
					this.capturingTeam = num3;
				}
				if (this.captureFraction > 0f && num3 != this.captureTeam && num3 != this.capturingTeam)
				{
					this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 0f)
					{
						this.captureTeam = -1;
					}
				}
				else if (this.captureTeam == -1 && this.captureFraction < 1f && this.capturingTeam == num3)
				{
					this.DoProgressEffect();
					this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 1f)
					{
						this.DoCaptureEffect();
						this.captureTeam = num3;
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, num2 > 1, false, true);
		}
		else
		{
			if (!this.capturingPlayer.IsValid(true) && !this.capturedPlayer.IsValid(true))
			{
				this.captureFraction = 0f;
			}
			if (this.captureTrigger.entityContents.Count == 0)
			{
				this.capturingPlayer.Set(null);
			}
			if (this.captureTrigger.entityContents.Count == 1)
			{
				foreach (global::BaseEntity baseEntity2 in this.captureTrigger.entityContents)
				{
					global::BasePlayer component2 = baseEntity2.GetComponent<global::BasePlayer>();
					if (!(component2 == null))
					{
						if (!this.capturedPlayer.IsValid(true) && this.captureFraction == 0f)
						{
							this.capturingPlayer.Set(component2);
						}
						if (this.captureFraction > 0f && component2 != this.capturedPlayer.Get(true) && component2 != this.capturingPlayer.Get(true))
						{
							this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 0f)
							{
								this.capturedPlayer.Set(null);
								break;
							}
							break;
						}
						else
						{
							if (this.capturedPlayer.Get(true) || this.captureFraction >= 1f || !(this.capturingPlayer.Get(true) == component2))
							{
								break;
							}
							this.DoProgressEffect();
							this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 1f)
							{
								this.DoCaptureEffect();
								this.capturedPlayer.Set(component2);
								break;
							}
							break;
						}
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, this.captureTrigger.entityContents.Count > 1, false, true);
		}
		if (num != this.captureFraction)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x000FFF0C File Offset: 0x000FE10C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.captureFraction;
		info.msg.ioEntity.genericInt1 = this.captureTeam;
		info.msg.ioEntity.genericInt2 = this.capturingTeam;
		info.msg.ioEntity.genericEntRef1 = this.capturedPlayer.uid;
		info.msg.ioEntity.genericEntRef2 = this.capturingPlayer.uid;
	}

	// Token: 0x040021C4 RID: 8644
	public CapturePointTrigger captureTrigger;

	// Token: 0x040021C5 RID: 8645
	public float timeToCapture = 3f;

	// Token: 0x040021C6 RID: 8646
	public int scorePerSecond = 1;

	// Token: 0x040021C7 RID: 8647
	public string scoreName = "score";

	// Token: 0x040021C8 RID: 8648
	private float captureFraction;

	// Token: 0x040021C9 RID: 8649
	private int captureTeam = -1;

	// Token: 0x040021CA RID: 8650
	private int capturingTeam = -1;

	// Token: 0x040021CB RID: 8651
	public EntityRef capturingPlayer;

	// Token: 0x040021CC RID: 8652
	public EntityRef capturedPlayer;

	// Token: 0x040021CD RID: 8653
	public const global::BaseEntity.Flags Flag_Contested = global::BaseEntity.Flags.Busy;

	// Token: 0x040021CE RID: 8654
	public RustText capturePointText;

	// Token: 0x040021CF RID: 8655
	public RustText captureOwnerName;

	// Token: 0x040021D0 RID: 8656
	public Image captureProgressImage;

	// Token: 0x040021D1 RID: 8657
	public GameObjectRef progressBeepEffect;

	// Token: 0x040021D2 RID: 8658
	public GameObjectRef progressCompleteEffect;

	// Token: 0x040021D3 RID: 8659
	public Transform computerPoint;

	// Token: 0x040021D4 RID: 8660
	private float nextBeepTime;
}
