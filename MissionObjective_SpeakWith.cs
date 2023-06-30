using System;
using UnityEngine;

// Token: 0x02000624 RID: 1572
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/SpeakWith")]
public class MissionObjective_SpeakWith : MissionObjective
{
	// Token: 0x06002E75 RID: 11893 RVA: 0x00116AFC File Offset: 0x00114CFC
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		BaseEntity baseEntity = instance.ProviderEntity();
		if (baseEntity)
		{
			instance.missionLocation = baseEntity.transform.position;
			playerFor.MissionDirty(true);
		}
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x00116B3C File Offset: 0x00114D3C
	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		if (base.IsCompleted(index, instance))
		{
			return;
		}
		if (!base.CanProgress(index, instance))
		{
			return;
		}
		if (type == BaseMission.MissionEventType.CONVERSATION)
		{
			BaseEntity baseEntity = instance.ProviderEntity();
			if (baseEntity)
			{
				IMissionProvider component = baseEntity.GetComponent<IMissionProvider>();
				if (component != null && component.ProviderID().Value.ToString() == identifier && amount == 1f)
				{
					bool flag = true;
					if (this.requiredReturnItems != null && this.requiredReturnItems.Length != 0)
					{
						foreach (ItemAmount itemAmount in this.requiredReturnItems)
						{
							if ((float)playerFor.inventory.GetAmount(itemAmount.itemDef.itemid) < itemAmount.amount)
							{
								flag = false;
								break;
							}
						}
						if (flag && this.destroyReturnItems)
						{
							foreach (ItemAmount itemAmount2 in this.requiredReturnItems)
							{
								playerFor.inventory.Take(null, itemAmount2.itemDef.itemid, (int)itemAmount2.amount);
							}
						}
					}
					if (this.requiredReturnItems == null || this.requiredReturnItems.Length == 0 || flag)
					{
						this.CompleteObjective(index, instance, playerFor);
						playerFor.MissionDirty(true);
					}
				}
			}
		}
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
	}

	// Token: 0x0400261D RID: 9757
	public ItemAmount[] requiredReturnItems;

	// Token: 0x0400261E RID: 9758
	public bool destroyReturnItems;
}
