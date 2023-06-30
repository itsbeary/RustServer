using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class ElectricGenerator : global::IOEntity
{
	// Token: 0x0600283E RID: 10302 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000FA32B File Offset: 0x000F852B
	public override int MaximalPowerOutput()
	{
		return Mathf.FloorToInt(this.electricAmount);
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000FA338 File Offset: 0x000F8538
	public override int GetCurrentEnergy()
	{
		return (int)this.electricAmount;
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000FA341 File Offset: 0x000F8541
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return this.GetCurrentEnergy();
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000FA34C File Offset: 0x000F854C
	public override void UpdateOutputs()
	{
		this.currentEnergy = this.GetCurrentEnergy();
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).UpdateFromInput(this.currentEnergy, ioslot.connectedToSlot);
			}
		}
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000582CD File Offset: 0x000564CD
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000FA3AF File Offset: 0x000F85AF
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.ForcePuzzleReset), 1f);
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000FA3D0 File Offset: 0x000F85D0
	private void ForcePuzzleReset()
	{
		global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
		if (component != null)
		{
			component.DoReset();
			component.ResetTimer();
		}
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000FA3FC File Offset: 0x000F85FC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component)
			{
				info.msg.puzzleReset = Pool.Get<ProtoBuf.PuzzleReset>();
				info.msg.puzzleReset.playerBlocksReset = component.playersBlockReset;
				if (component.playerDetectionOrigin != null)
				{
					info.msg.puzzleReset.playerDetectionOrigin = component.playerDetectionOrigin.position;
				}
				info.msg.puzzleReset.playerDetectionRadius = component.playerDetectionRadius;
				info.msg.puzzleReset.scaleWithServerPopulation = component.scaleWithServerPopulation;
				info.msg.puzzleReset.timeBetweenResets = component.timeBetweenResets;
			}
		}
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000FA4C0 File Offset: 0x000F86C0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.puzzleReset != null)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component != null)
			{
				component.playersBlockReset = info.msg.puzzleReset.playerBlocksReset;
				if (component.playerDetectionOrigin != null)
				{
					component.playerDetectionOrigin.position = info.msg.puzzleReset.playerDetectionOrigin;
				}
				component.playerDetectionRadius = info.msg.puzzleReset.playerDetectionRadius;
				component.scaleWithServerPopulation = info.msg.puzzleReset.scaleWithServerPopulation;
				component.timeBetweenResets = info.msg.puzzleReset.timeBetweenResets;
				component.ResetTimer();
			}
		}
	}

	// Token: 0x040020A8 RID: 8360
	public float electricAmount = 8f;
}
