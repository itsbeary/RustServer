using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200080A RID: 2058
public class UIBuffs : SingletonComponent<UIBuffs>
{
	// Token: 0x060035B8 RID: 13752 RVA: 0x00147368 File Offset: 0x00145568
	public void Refresh(PlayerModifiers modifiers)
	{
		if (!this.Enabled)
		{
			return;
		}
		this.RemoveAll();
		if (modifiers == null)
		{
			return;
		}
		using (List<Modifier>.Enumerator enumerator = modifiers.All.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					UnityEngine.Object.Instantiate<Transform>(this.PrefabBuffIcon).SetParent(base.transform);
				}
			}
		}
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x001473E4 File Offset: 0x001455E4
	private void RemoveAll()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x04002E6A RID: 11882
	public bool Enabled = true;

	// Token: 0x04002E6B RID: 11883
	public Transform PrefabBuffIcon;
}
