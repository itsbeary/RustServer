using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class ScientistNPC : HumanNPC, IAIMounted
{
	// Token: 0x06001A7A RID: 6778 RVA: 0x000BEFA0 File Offset: 0x000BD1A0
	public void SetChatterType(ScientistNPC.RadioChatterType newType)
	{
		if (newType == this.radioChatterType)
		{
			return;
		}
		if (newType == ScientistNPC.RadioChatterType.Idle)
		{
			this.QueueRadioChatter();
			return;
		}
		base.CancelInvoke(new Action(this.PlayRadioChatter));
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000BEFC9 File Offset: 0x000BD1C9
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		base.InvokeRandomized(new Action(this.IdleCheck), 0f, 20f, 1f);
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x000BEFF9 File Offset: 0x000BD1F9
	public void IdleCheck()
	{
		if (Time.time > this.lastAlertedTime + 20f)
		{
			this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		}
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x000BF015 File Offset: 0x000BD215
	public void QueueRadioChatter()
	{
		if (!this.IsAlive() || base.IsDestroyed)
		{
			return;
		}
		base.Invoke(new Action(this.PlayRadioChatter), UnityEngine.Random.Range(this.IdleChatterRepeatRange.x, this.IdleChatterRepeatRange.y));
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x000BF055 File Offset: 0x000BD255
	public override bool ShotTest(float targetDist)
	{
		bool flag = base.ShotTest(targetDist);
		if (Time.time - this.lastGunShotTime < 5f)
		{
			this.Alert();
		}
		return flag;
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x000BF077 File Offset: 0x000BD277
	public void Alert()
	{
		this.lastAlertedTime = Time.time;
		this.SetChatterType(ScientistNPC.RadioChatterType.Alert);
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x000BF08B File Offset: 0x000BD28B
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.Alert();
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x000BF09C File Offset: 0x000BD29C
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.SetChatterType(ScientistNPC.RadioChatterType.NONE);
		if (this.DeathEffects.Length != 0)
		{
			Effect.server.Run(this.DeathEffects[UnityEngine.Random.Range(0, this.DeathEffects.Length)].resourcePath, this.ServerPosition, Vector3.up, null, false);
		}
		if (info != null && info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
		}
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000BF124 File Offset: 0x000BD324
	public void PlayRadioChatter()
	{
		if (this.RadioChatterEffects.Length == 0)
		{
			return;
		}
		if (base.IsDestroyed || base.transform == null)
		{
			base.CancelInvoke(new Action(this.PlayRadioChatter));
			return;
		}
		Effect.server.Run(this.RadioChatterEffects[UnityEngine.Random.Range(0, this.RadioChatterEffects.Length)].resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
		this.QueueRadioChatter();
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000BF1A0 File Offset: 0x000BD3A0
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity != null)
		{
			Item item = heldEntity.GetItem();
			if (item != null && item.contents != null)
			{
				if (UnityEngine.Random.Range(0, 3) == 0)
				{
					Item item2 = ItemManager.CreateByName("weapon.mod.flashlight", 1, 0UL);
					if (!item2.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item2.Remove(0f);
						return;
					}
					this.lightsOn = false;
					base.InvokeRandomized(new Action(base.LightCheck), 0f, 30f, 5f);
					base.LightCheck();
					return;
				}
				else
				{
					Item item3 = ItemManager.CreateByName("weapon.mod.lasersight", 1, 0UL);
					if (!item3.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item3.Remove(0f);
					}
					base.LightToggle(true);
					this.lightsOn = true;
				}
			}
		}
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000BF27E File Offset: 0x000BD47E
	public bool IsMounted()
	{
		return base.isMounted;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x000BF286 File Offset: 0x000BD486
	protected override string OverrideCorpseName()
	{
		return "Scientist";
	}

	// Token: 0x040012CE RID: 4814
	public GameObjectRef[] RadioChatterEffects;

	// Token: 0x040012CF RID: 4815
	public GameObjectRef[] DeathEffects;

	// Token: 0x040012D0 RID: 4816
	public string deathStatName = "kill_scientist";

	// Token: 0x040012D1 RID: 4817
	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	// Token: 0x040012D2 RID: 4818
	public ScientistNPC.RadioChatterType radioChatterType;

	// Token: 0x040012D3 RID: 4819
	protected float lastAlertedTime = -100f;

	// Token: 0x02000C6C RID: 3180
	public enum RadioChatterType
	{
		// Token: 0x04004380 RID: 17280
		NONE,
		// Token: 0x04004381 RID: 17281
		Idle,
		// Token: 0x04004382 RID: 17282
		Alert
	}
}
