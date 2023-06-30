using System;
using Facepunch;
using UnityEngine;

// Token: 0x02000898 RID: 2200
public class ServerBrowserTagGroup : MonoBehaviour
{
	// Token: 0x060036D6 RID: 14038 RVA: 0x0014A115 File Offset: 0x00148315
	private void Initialize()
	{
		if (this.tags == null)
		{
			this.tags = base.GetComponentsInChildren<ServerBrowserTag>(true);
		}
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x0014A12C File Offset: 0x0014832C
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x0014A134 File Offset: 0x00148334
	public bool AnyActive()
	{
		ServerBrowserTag[] array = this.tags;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsActive)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x0014A164 File Offset: 0x00148364
	public void Refresh(in ServerInfo server, ref int tagsEnabled, int maxTags)
	{
		this.Initialize();
		bool flag = false;
		foreach (ServerBrowserTag serverBrowserTag in this.tags)
		{
			if ((!this.isExclusive || !flag) && tagsEnabled <= maxTags && server.Tags.Contains(serverBrowserTag.serverTag))
			{
				serverBrowserTag.SetActive(true);
				tagsEnabled++;
				flag = true;
			}
			else
			{
				serverBrowserTag.SetActive(false);
			}
		}
		base.gameObject.SetActive(flag);
	}

	// Token: 0x0400319C RID: 12700
	[Tooltip("If set then queries will filter out servers matching unselected tags in the group")]
	public bool isExclusive;

	// Token: 0x0400319D RID: 12701
	[NonSerialized]
	public ServerBrowserTag[] tags;
}
