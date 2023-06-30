using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008B9 RID: 2233
public class Scoreboard : MonoBehaviour, IClientComponent
{
	// Token: 0x0400324F RID: 12879
	public static Scoreboard instance;

	// Token: 0x04003250 RID: 12880
	public RustText scoreboardTitle;

	// Token: 0x04003251 RID: 12881
	public RectTransform scoreboardRootContents;

	// Token: 0x04003252 RID: 12882
	public RustText scoreLimitText;

	// Token: 0x04003253 RID: 12883
	public GameObject teamPrefab;

	// Token: 0x04003254 RID: 12884
	public GameObject columnPrefab;

	// Token: 0x04003255 RID: 12885
	public GameObject dividerPrefab;

	// Token: 0x04003256 RID: 12886
	public Color localPlayerColor;

	// Token: 0x04003257 RID: 12887
	public Color otherPlayerColor;

	// Token: 0x04003258 RID: 12888
	public Scoreboard.TeamColumn[] teamColumns;

	// Token: 0x04003259 RID: 12889
	public GameObject[] TeamPanels;

	// Token: 0x02000EB5 RID: 3765
	public class TeamColumn
	{
		// Token: 0x04004D0D RID: 19725
		public GameObject nameColumn;

		// Token: 0x04004D0E RID: 19726
		public GameObject[] activeColumns;
	}
}
