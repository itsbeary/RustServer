using System;
using Network;

// Token: 0x020000CF RID: 207
public class SlotMachineStorage : StorageContainer
{
	// Token: 0x06001282 RID: 4738 RVA: 0x00095B68 File Offset: 0x00093D68
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachineStorage.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00095BA8 File Offset: 0x00093DA8
	public bool IsPlayerValid(BasePlayer player)
	{
		return player.isMounted && !(player.GetMounted() != base.GetParentEntity());
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x00095BC8 File Offset: 0x00093DC8
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return this.IsPlayerValid(player) && base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00095BE0 File Offset: 0x00093DE0
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		Item slot = base.inventory.GetSlot(0);
		this.UpdateAmount((slot != null) ? slot.amount : 0);
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x00095C12 File Offset: 0x00093E12
	public void UpdateAmount(int amount)
	{
		if (this.Amount == amount)
		{
			return;
		}
		this.Amount = amount;
		(base.GetParentEntity() as SlotMachine).OnBettingScrapUpdated(amount);
		base.ClientRPC<int>(null, "RPC_UpdateAmount", this.Amount);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x00095C48 File Offset: 0x00093E48
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.IsPlayerValid(player) && base.CanBeLooted(player);
	}

	// Token: 0x04000B8C RID: 2956
	public int Amount;
}
