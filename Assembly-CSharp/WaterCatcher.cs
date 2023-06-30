using System;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class WaterCatcher : LiquidContainer
{
	// Token: 0x06002242 RID: 8770 RVA: 0x000DDE8A File Offset: 0x000DC08A
	public override void ServerInit()
	{
		base.ServerInit();
		this.AddResource(1);
		base.InvokeRandomized(new Action(this.CollectWater), 60f, 60f, 6f);
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000DDEBC File Offset: 0x000DC0BC
	private void CollectWater()
	{
		if (this.IsFull())
		{
			return;
		}
		float num = 0.25f;
		num += Climate.GetFog(base.transform.position) * 2f;
		if (this.TestIsOutside())
		{
			num += Climate.GetRain(base.transform.position);
			num += Climate.GetSnow(base.transform.position) * 0.5f;
		}
		this.AddResource(Mathf.CeilToInt(this.maxItemToCreate * num));
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x000DDF38 File Offset: 0x000DC138
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000DDF74 File Offset: 0x000DC174
	private bool TestIsOutside()
	{
		return !Physics.SphereCast(new Ray(base.transform.localToWorldMatrix.MultiplyPoint3x4(this.rainTestPosition), Vector3.up), this.rainTestSize, 256f, 161546513);
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000DDFBC File Offset: 0x000DC1BC
	private void AddResource(int iAmount)
	{
		if (this.outputs.Length != 0)
		{
			IOEntity ioentity = this.CheckPushLiquid(this.outputs[0].connectedTo.Get(true), iAmount, this, IOEntity.backtracking * 2);
			LiquidContainer liquidContainer;
			if (ioentity != null && (liquidContainer = ioentity as LiquidContainer) != null)
			{
				liquidContainer.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
				return;
			}
		}
		base.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
		base.UpdateOnFlag();
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000DE03C File Offset: 0x000DC23C
	private IOEntity CheckPushLiquid(IOEntity connected, int amount, IOEntity fromSource, int depth)
	{
		if (depth <= 0 || this.itemToCreate == null)
		{
			return null;
		}
		if (connected == null)
		{
			return null;
		}
		Vector3 zero = Vector3.zero;
		IOEntity ioentity = connected.FindGravitySource(ref zero, IOEntity.backtracking, true);
		if (ioentity != null && !connected.AllowLiquidPassthrough(ioentity, zero, false))
		{
			return null;
		}
		if (connected == this || this.ConsiderConnectedTo(connected))
		{
			return null;
		}
		if (connected.prefabID == 2150367216U)
		{
			return null;
		}
		foreach (IOEntity.IOSlot ioslot in connected.outputs)
		{
			IOEntity ioentity2 = ioslot.connectedTo.Get(true);
			Vector3 vector = connected.transform.TransformPoint(ioslot.handlePosition);
			if (ioentity2 != null && ioentity2 != fromSource && ioentity2.AllowLiquidPassthrough(connected, vector, false))
			{
				IOEntity ioentity3 = this.CheckPushLiquid(ioentity2, amount, fromSource, depth - 1);
				if (ioentity3 != null)
				{
					return ioentity3;
				}
			}
		}
		LiquidContainer liquidContainer;
		if ((liquidContainer = connected as LiquidContainer) != null && liquidContainer.inventory.GetAmount(this.itemToCreate.itemid, false) + amount < liquidContainer.maxStackSize)
		{
			return connected;
		}
		return null;
	}

	// Token: 0x04001A72 RID: 6770
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	// Token: 0x04001A73 RID: 6771
	public float maxItemToCreate = 10f;

	// Token: 0x04001A74 RID: 6772
	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	// Token: 0x04001A75 RID: 6773
	public float rainTestSize = 1f;

	// Token: 0x04001A76 RID: 6774
	private const float collectInterval = 60f;
}
