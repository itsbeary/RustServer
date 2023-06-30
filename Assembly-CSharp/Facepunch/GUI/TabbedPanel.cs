using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000B06 RID: 2822
	internal class TabbedPanel
	{
		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x060044C4 RID: 17604 RVA: 0x00192FC5 File Offset: 0x001911C5
		public TabbedPanel.Tab selectedTab
		{
			get
			{
				return this.tabs[this.selectedTabID];
			}
		}

		// Token: 0x060044C5 RID: 17605 RVA: 0x00192FD8 File Offset: 0x001911D8
		public void Add(TabbedPanel.Tab tab)
		{
			this.tabs.Add(tab);
		}

		// Token: 0x060044C6 RID: 17606 RVA: 0x00192FE8 File Offset: 0x001911E8
		internal void DrawVertical(float width)
		{
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(width),
				GUILayout.ExpandHeight(true)
			});
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (GUILayout.Toggle(this.selectedTabID == i, this.tabs[i].name, new GUIStyle("devtab"), Array.Empty<GUILayoutOption>()))
				{
					this.selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle("devtab"), new GUILayoutOption[] { GUILayout.ExpandHeight(true) }))
			{
				this.selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		// Token: 0x060044C7 RID: 17607 RVA: 0x0019309C File Offset: 0x0019129C
		internal void DrawContents()
		{
			if (this.selectedTabID < 0)
			{
				return;
			}
			TabbedPanel.Tab selectedTab = this.selectedTab;
			GUILayout.BeginVertical(new GUIStyle("devtabcontents"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			if (selectedTab.drawFunc != null)
			{
				selectedTab.drawFunc();
			}
			GUILayout.EndVertical();
		}

		// Token: 0x04003D40 RID: 15680
		private int selectedTabID;

		// Token: 0x04003D41 RID: 15681
		private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();

		// Token: 0x02000F95 RID: 3989
		public struct Tab
		{
			// Token: 0x0400509D RID: 20637
			public string name;

			// Token: 0x0400509E RID: 20638
			public Action drawFunc;
		}
	}
}
