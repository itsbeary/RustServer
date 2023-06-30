using System;
using UnityEngine;

// Token: 0x0200044D RID: 1101
public class PlayerModelCinematicList : PrefabAttribute, IClientComponent
{
	// Token: 0x060024F9 RID: 9465 RVA: 0x000EA503 File Offset: 0x000E8703
	protected override Type GetIndexedType()
	{
		return typeof(PlayerModelCinematicList);
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000EA50F File Offset: 0x000E870F
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	// Token: 0x04001D2D RID: 7469
	public PlayerModelCinematicList.PlayerModelCinematicAnimation[] Animations;

	// Token: 0x02000D02 RID: 3330
	[Serializable]
	public struct PlayerModelCinematicAnimation
	{
		// Token: 0x0400466D RID: 18029
		public string StateName;

		// Token: 0x0400466E RID: 18030
		public string ClipName;

		// Token: 0x0400466F RID: 18031
		public float Length;
	}
}
