using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class TennisArcadeGame : BaseArcadeGame
{
	// Token: 0x04000FD6 RID: 4054
	public ArcadeEntity paddle1;

	// Token: 0x04000FD7 RID: 4055
	public ArcadeEntity paddle2;

	// Token: 0x04000FD8 RID: 4056
	public ArcadeEntity ball;

	// Token: 0x04000FD9 RID: 4057
	public Transform paddle1Origin;

	// Token: 0x04000FDA RID: 4058
	public Transform paddle2Origin;

	// Token: 0x04000FDB RID: 4059
	public Transform paddle1Goal;

	// Token: 0x04000FDC RID: 4060
	public Transform paddle2Goal;

	// Token: 0x04000FDD RID: 4061
	public Transform ballSpawn;

	// Token: 0x04000FDE RID: 4062
	public float maxScore = 5f;

	// Token: 0x04000FDF RID: 4063
	public ArcadeEntity[] paddle1ScoreNodes;

	// Token: 0x04000FE0 RID: 4064
	public ArcadeEntity[] paddle2ScoreNodes;

	// Token: 0x04000FE1 RID: 4065
	public int paddle1Score;

	// Token: 0x04000FE2 RID: 4066
	public int paddle2Score;

	// Token: 0x04000FE3 RID: 4067
	public float sensitivity = 1f;

	// Token: 0x04000FE4 RID: 4068
	public ArcadeEntity logo;

	// Token: 0x04000FE5 RID: 4069
	public bool OnMainMenu;

	// Token: 0x04000FE6 RID: 4070
	public bool GameActive;
}
