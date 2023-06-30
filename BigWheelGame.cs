using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class BigWheelGame : SpinnerWheel
{
	// Token: 0x06001704 RID: 5892 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool AllowPlayerSpins()
	{
		return false;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CanUpdateSign(BasePlayer player)
	{
		return false;
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x000B02FF File Offset: 0x000AE4FF
	public override float GetMaxSpinSpeed()
	{
		return 180f;
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x000B0306 File Offset: 0x000AE506
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitBettingTerminals), 3f);
		base.Invoke(new Action(this.DoSpin), 10f);
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x000B033C File Offset: 0x000AE53C
	public void DoSpin()
	{
		if (this.velocity > 0f)
		{
			return;
		}
		this.velocity += UnityEngine.Random.Range(7f, 16f);
		this.spinNumber++;
		this.SetTerminalsLocked(true);
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000B0388 File Offset: 0x000AE588
	public void SetTerminalsLocked(bool isLocked)
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.inventory.SetLocked(isLocked);
		}
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x000B03E0 File Offset: 0x000AE5E0
	public void RemoveTerminal(BigWheelBettingTerminal terminal)
	{
		this.terminals.Remove(terminal);
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x000B03F0 File Offset: 0x000AE5F0
	protected void InitBettingTerminals()
	{
		this.terminals.Clear();
		Vis.Entities<BigWheelBettingTerminal>(base.transform.position, 30f, this.terminals, 256, QueryTriggerInteraction.Collide);
		this.terminals = this.terminals.Distinct<BigWheelBettingTerminal>().ToList<BigWheelBettingTerminal>();
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x000B0440 File Offset: 0x000AE640
	public override void Update_Server()
	{
		float velocity = this.velocity;
		base.Update_Server();
		float velocity2 = this.velocity;
		if (velocity > 0f && velocity2 == 0f && this.spinNumber > this.lastPaidSpinNumber)
		{
			this.Payout();
			this.lastPaidSpinNumber = this.spinNumber;
			this.QueueSpin();
		}
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x000B0495 File Offset: 0x000AE695
	public float SpinSpacing()
	{
		return BigWheelGame.spinFrequencySeconds;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x000B049C File Offset: 0x000AE69C
	public void QueueSpin()
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.ClientRPC<float>(null, "SetTimeUntilNextSpin", this.SpinSpacing());
		}
		base.Invoke(new Action(this.DoSpin), this.SpinSpacing());
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x000B0510 File Offset: 0x000AE710
	public void Payout()
	{
		HitNumber currentHitType = this.GetCurrentHitType();
		Guid guid = Guid.NewGuid();
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			if (!bigWheelBettingTerminal.isClient)
			{
				bool flag = false;
				bool flag2 = false;
				Item slot = bigWheelBettingTerminal.inventory.GetSlot((int)currentHitType.hitType);
				if (slot != null)
				{
					int num = currentHitType.ColorToMultiplier(currentHitType.hitType);
					int amount = slot.amount;
					slot.amount += slot.amount * num;
					slot.RemoveFromContainer();
					slot.MoveToContainer(bigWheelBettingTerminal.inventory, 5, true, false, null, true);
					flag = true;
					Analytics.Azure.OnGamblingResult(bigWheelBettingTerminal.lastPlayer, bigWheelBettingTerminal, amount, slot.amount, new Guid?(guid));
				}
				for (int i = 0; i < 5; i++)
				{
					Item slot2 = bigWheelBettingTerminal.inventory.GetSlot(i);
					if (slot2 != null)
					{
						Analytics.Azure.OnGamblingResult(bigWheelBettingTerminal.lastPlayer, bigWheelBettingTerminal, slot2.amount, 0, new Guid?(guid));
						slot2.Remove(0f);
						flag2 = true;
					}
				}
				if (flag || flag2)
				{
					bigWheelBettingTerminal.ClientRPC<bool>(null, "WinOrLoseSound", flag);
				}
			}
		}
		ItemManager.DoRemoves();
		this.SetTerminalsLocked(false);
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x000B0678 File Offset: 0x000AE878
	public HitNumber GetCurrentHitType()
	{
		HitNumber hitNumber = null;
		float num = float.PositiveInfinity;
		foreach (HitNumber hitNumber2 in this.hitNumbers)
		{
			float num2 = Vector3.Distance(this.indicator.transform.position, hitNumber2.transform.position);
			if (num2 < num)
			{
				hitNumber = hitNumber2;
				num = num2;
			}
		}
		return hitNumber;
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x000B06D8 File Offset: 0x000AE8D8
	[ContextMenu("LoadHitNumbers")]
	private void LoadHitNumbers()
	{
		HitNumber[] componentsInChildren = base.GetComponentsInChildren<HitNumber>();
		this.hitNumbers = componentsInChildren;
	}

	// Token: 0x04000F3E RID: 3902
	public HitNumber[] hitNumbers;

	// Token: 0x04000F3F RID: 3903
	public GameObject indicator;

	// Token: 0x04000F40 RID: 3904
	public GameObjectRef winEffect;

	// Token: 0x04000F41 RID: 3905
	[ServerVar]
	public static float spinFrequencySeconds = 45f;

	// Token: 0x04000F42 RID: 3906
	protected int spinNumber;

	// Token: 0x04000F43 RID: 3907
	protected int lastPaidSpinNumber = -1;

	// Token: 0x04000F44 RID: 3908
	protected List<BigWheelBettingTerminal> terminals = new List<BigWheelBettingTerminal>();
}
