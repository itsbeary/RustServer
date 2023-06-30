using System;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class ChippyArcadeGame : BaseArcadeGame
{
	// Token: 0x04000FAA RID: 4010
	public ChippyMainCharacter mainChar;

	// Token: 0x04000FAB RID: 4011
	public SpriteArcadeEntity mainCharAim;

	// Token: 0x04000FAC RID: 4012
	public ChippyBoss currentBoss;

	// Token: 0x04000FAD RID: 4013
	public ChippyBoss[] bossPrefabs;

	// Token: 0x04000FAE RID: 4014
	public SpriteArcadeEntity mainMenuLogo;

	// Token: 0x04000FAF RID: 4015
	public Transform respawnPoint;

	// Token: 0x04000FB0 RID: 4016
	public Vector2 mouseAim = new Vector2(0f, 1f);

	// Token: 0x04000FB1 RID: 4017
	public TextArcadeEntity levelIndicator;

	// Token: 0x04000FB2 RID: 4018
	public TextArcadeEntity gameOverIndicator;

	// Token: 0x04000FB3 RID: 4019
	public TextArcadeEntity playGameButton;

	// Token: 0x04000FB4 RID: 4020
	public TextArcadeEntity highScoresButton;

	// Token: 0x04000FB5 RID: 4021
	public bool OnMainMenu;

	// Token: 0x04000FB6 RID: 4022
	public bool GameActive;

	// Token: 0x04000FB7 RID: 4023
	public int level;

	// Token: 0x04000FB8 RID: 4024
	public TextArcadeEntity[] scoreDisplays;

	// Token: 0x04000FB9 RID: 4025
	public MenuButtonArcadeEntity[] mainMenuButtons;

	// Token: 0x04000FBA RID: 4026
	public int selectedButtonIndex;

	// Token: 0x04000FBB RID: 4027
	public bool OnHighScores;
}
