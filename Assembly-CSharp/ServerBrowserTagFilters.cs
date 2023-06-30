using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000897 RID: 2199
public class ServerBrowserTagFilters : MonoBehaviour
{
	// Token: 0x060036D1 RID: 14033 RVA: 0x00149F04 File Offset: 0x00148104
	public void Start()
	{
		this._groups = base.gameObject.GetComponentsInChildren<ServerBrowserTagGroup>();
		UnityAction unityAction = delegate
		{
			UnityEvent tagFiltersChanged = this.TagFiltersChanged;
			if (tagFiltersChanged == null)
			{
				return;
			}
			tagFiltersChanged.Invoke();
		};
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			foreach (ServerBrowserTag serverBrowserTag in groups[i].tags)
			{
				serverBrowserTag.button.OnPressed.AddListener(unityAction);
				serverBrowserTag.button.OnReleased.AddListener(unityAction);
			}
		}
	}

	// Token: 0x060036D2 RID: 14034 RVA: 0x00149F84 File Offset: 0x00148184
	public void DeselectAll()
	{
		if (this._groups == null)
		{
			return;
		}
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.tags != null)
			{
				ServerBrowserTag[] tags = serverBrowserTagGroup.tags;
				for (int j = 0; j < tags.Length; j++)
				{
					tags[j].button.SetToggleFalse();
				}
			}
		}
	}

	// Token: 0x060036D3 RID: 14035 RVA: 0x00149FE4 File Offset: 0x001481E4
	public void GetTags(out List<HashSet<string>> searchTagGroups, out HashSet<string> excludeTags)
	{
		searchTagGroups = new List<HashSet<string>>();
		excludeTags = new HashSet<string>();
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.AnyActive())
			{
				if (serverBrowserTagGroup.isExclusive)
				{
					HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (ServerBrowserTag serverBrowserTag in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag.IsActive)
						{
							hashSet.Add(serverBrowserTag.serverTag);
						}
						else if (serverBrowserTagGroup.isExclusive)
						{
							excludeTags.Add(serverBrowserTag.serverTag);
						}
					}
					if (hashSet.Count > 0)
					{
						searchTagGroups.Add(hashSet);
					}
				}
				else
				{
					foreach (ServerBrowserTag serverBrowserTag2 in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag2.IsActive)
						{
							HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
							hashSet2.Add(serverBrowserTag2.serverTag);
							searchTagGroups.Add(hashSet2);
						}
					}
				}
			}
		}
	}

	// Token: 0x04003199 RID: 12697
	public UnityEvent TagFiltersChanged = new UnityEvent();

	// Token: 0x0400319A RID: 12698
	private ServerBrowserTagGroup[] _groups;

	// Token: 0x0400319B RID: 12699
	private List<bool> _previousState;
}
