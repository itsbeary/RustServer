using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200084A RID: 2122
public class OvenLootPanel : MonoBehaviour
{
	// Token: 0x04002FBC RID: 12220
	public GameObject controlsOn;

	// Token: 0x04002FBD RID: 12221
	public GameObject controlsOff;

	// Token: 0x04002FBE RID: 12222
	public Image TitleBackground;

	// Token: 0x04002FBF RID: 12223
	public RustText TitleText;

	// Token: 0x04002FC0 RID: 12224
	public Color AlertBackgroundColor;

	// Token: 0x04002FC1 RID: 12225
	public Color AlertTextColor;

	// Token: 0x04002FC2 RID: 12226
	public Color OffBackgroundColor;

	// Token: 0x04002FC3 RID: 12227
	public Color OffTextColor;

	// Token: 0x04002FC4 RID: 12228
	public Color OnBackgroundColor;

	// Token: 0x04002FC5 RID: 12229
	public Color OnTextColor;

	// Token: 0x04002FC6 RID: 12230
	private Translate.Phrase OffPhrase = new Translate.Phrase("off", "off");

	// Token: 0x04002FC7 RID: 12231
	private Translate.Phrase OnPhrase = new Translate.Phrase("on", "on");

	// Token: 0x04002FC8 RID: 12232
	private Translate.Phrase NoFuelPhrase = new Translate.Phrase("no_fuel", "No Fuel");

	// Token: 0x04002FC9 RID: 12233
	public GameObject FuelRowPrefab;

	// Token: 0x04002FCA RID: 12234
	public GameObject MaterialRowPrefab;

	// Token: 0x04002FCB RID: 12235
	public GameObject ItemRowPrefab;

	// Token: 0x04002FCC RID: 12236
	public Sprite IconBackground_Wood;

	// Token: 0x04002FCD RID: 12237
	public Sprite IconBackGround_Input;

	// Token: 0x04002FCE RID: 12238
	public LootGrid LootGrid_Wood;

	// Token: 0x04002FCF RID: 12239
	public LootGrid LootGrid_Input;

	// Token: 0x04002FD0 RID: 12240
	public LootGrid LootGrid_Output;

	// Token: 0x04002FD1 RID: 12241
	public GameObject Contents;

	// Token: 0x04002FD2 RID: 12242
	public GameObject[] ElectricDisableRoots = new GameObject[0];
}
