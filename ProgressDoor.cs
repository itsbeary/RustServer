using System;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class ProgressDoor : IOEntity
{
	// Token: 0x0600288A RID: 10378 RVA: 0x000FACA6 File Offset: 0x000F8EA6
	public override void ResetIOState()
	{
		this.storedEnergy = 0f;
		this.UpdateProgress();
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000FACB9 File Offset: 0x000F8EB9
	public override float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount <= 0f)
		{
			this.NoEnergy();
			return inputAmount;
		}
		this.AddEnergy(inputAmount);
		if (this.storedEnergy == this.energyForOpen)
		{
			return inputAmount;
		}
		return 0f;
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void NoEnergy()
	{
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x000FACE7 File Offset: 0x000F8EE7
	public virtual void AddEnergy(float amount)
	{
		if (amount <= 0f)
		{
			return;
		}
		this.storedEnergy += amount;
		this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
	}

	// Token: 0x0600288E RID: 10382 RVA: 0x00007D2F File Offset: 0x00005F2F
	public virtual void UpdateProgress()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x040020DB RID: 8411
	public float storedEnergy;

	// Token: 0x040020DC RID: 8412
	public float energyForOpen = 1f;

	// Token: 0x040020DD RID: 8413
	public float secondsToClose = 1f;

	// Token: 0x040020DE RID: 8414
	public float openProgress;
}
