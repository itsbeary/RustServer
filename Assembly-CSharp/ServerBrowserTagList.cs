using System;
using UnityEngine;

// Token: 0x02000899 RID: 2201
public class ServerBrowserTagList : MonoBehaviour
{
	// Token: 0x060036DB RID: 14043 RVA: 0x0014A1DC File Offset: 0x001483DC
	private void Initialize()
	{
		if (this._groups == null)
		{
			this._groups = base.GetComponentsInChildren<ServerBrowserTagGroup>(true);
		}
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x0014A1F3 File Offset: 0x001483F3
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x0014A1FC File Offset: 0x001483FC
	public bool Refresh(in ServerInfo server)
	{
		this.Initialize();
		int num = 0;
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			groups[i].Refresh(server, ref num, this.maxTagsToShow);
		}
		return num > 0;
	}

	// Token: 0x0400319E RID: 12702
	public int maxTagsToShow = 3;

	// Token: 0x0400319F RID: 12703
	private ServerBrowserTagGroup[] _groups;
}
