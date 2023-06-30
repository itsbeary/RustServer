using System;
using Facepunch;
using ntw.CurvedTextMeshPro;
using ProtoBuf;
using Rust.UI;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class SkullTrophy : StorageContainer
{
	// Token: 0x0600177D RID: 6013 RVA: 0x000B2434 File Offset: 0x000B0634
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x000B2448 File Offset: 0x000B0648
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (base.inventory != null && base.inventory.itemList.Count == 1)
			{
				info.msg.skullTrophy = Pool.Get<ProtoBuf.SkullTrophy>();
				info.msg.skullTrophy.playerName = base.inventory.itemList[0].GetName(new bool?(false));
				info.msg.skullTrophy.streamerName = base.inventory.itemList[0].GetName(new bool?(true));
				return;
			}
			if (info.msg.skullTrophy != null)
			{
				info.msg.skullTrophy.playerName = string.Empty;
				info.msg.skullTrophy.streamerName = string.Empty;
			}
		}
	}

	// Token: 0x04001016 RID: 4118
	public RustText NameText;

	// Token: 0x04001017 RID: 4119
	public TextProOnACircle CircleModifier;

	// Token: 0x04001018 RID: 4120
	public int AngleModifierMinCharCount = 3;

	// Token: 0x04001019 RID: 4121
	public int AngleModifierMaxCharCount = 20;

	// Token: 0x0400101A RID: 4122
	public int AngleModifierMinArcAngle = 20;

	// Token: 0x0400101B RID: 4123
	public int AngleModifierMaxArcAngle = 45;

	// Token: 0x0400101C RID: 4124
	public float SunsetTime = 18f;

	// Token: 0x0400101D RID: 4125
	public float SunriseTime = 5f;

	// Token: 0x0400101E RID: 4126
	public MeshRenderer[] SkullRenderers;

	// Token: 0x0400101F RID: 4127
	public Material[] DaySkull;

	// Token: 0x04001020 RID: 4128
	public Material[] NightSkull;

	// Token: 0x04001021 RID: 4129
	public Material[] NoSkull;
}
