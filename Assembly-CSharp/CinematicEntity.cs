using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class CinematicEntity : BaseEntity
{
	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06001EB5 RID: 7861 RVA: 0x000D095C File Offset: 0x000CEB5C
	// (set) Token: 0x06001EB6 RID: 7862 RVA: 0x000D0964 File Offset: 0x000CEB64
	[ServerVar(Help = "Hides cinematic light source meshes (keeps lights visible)")]
	public static bool HideObjects
	{
		get
		{
			return CinematicEntity._hideObjects;
		}
		set
		{
			if (value != CinematicEntity._hideObjects)
			{
				CinematicEntity._hideObjects = value;
				foreach (CinematicEntity cinematicEntity in CinematicEntity.serverList)
				{
					cinematicEntity.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity._hideObjects, false, true);
				}
			}
		}
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000D09D0 File Offset: 0x000CEBD0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Add(this);
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity.HideObjects, false, true);
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000D0A02 File Offset: 0x000CEC02
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer && CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Remove(this);
		}
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000D0A2C File Offset: 0x000CEC2C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = !base.HasFlag(BaseEntity.Flags.Reserved1);
		this.ToggleObjects(flag);
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000D0A58 File Offset: 0x000CEC58
	private void ToggleObjects(bool state)
	{
		foreach (GameObject gameObject in this.DisableObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(state);
			}
		}
	}

	// Token: 0x040017B4 RID: 6068
	private const BaseEntity.Flags HideMesh = BaseEntity.Flags.Reserved1;

	// Token: 0x040017B5 RID: 6069
	public GameObject[] DisableObjects;

	// Token: 0x040017B6 RID: 6070
	private static bool _hideObjects = false;

	// Token: 0x040017B7 RID: 6071
	private static List<CinematicEntity> serverList = new List<CinematicEntity>();
}
