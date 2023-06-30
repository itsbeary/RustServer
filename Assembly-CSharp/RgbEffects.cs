using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x0200073D RID: 1853
public class RgbEffects : SingletonComponent<RgbEffects>
{
	// Token: 0x0600336D RID: 13165 RVA: 0x000063A5 File Offset: 0x000045A5
	[ClientVar(Name = "static")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Static(ConsoleSystem.Arg args)
	{
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x000063A5 File Offset: 0x000045A5
	[ClientVar(Name = "pulse")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Pulse(ConsoleSystem.Arg args)
	{
	}

	// Token: 0x04002A46 RID: 10822
	[ClientVar(Help = "Enables RGB lighting effects (supports SteelSeries and Razer)", Saved = true)]
	public static bool Enabled = true;

	// Token: 0x04002A47 RID: 10823
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on SteelSeries devices", Saved = true)]
	public static Vector3 ColorCorrection_SteelSeries = new Vector3(1.5f, 1.5f, 1.5f);

	// Token: 0x04002A48 RID: 10824
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on Razer devices", Saved = true)]
	public static Vector3 ColorCorrection_Razer = new Vector3(3f, 3f, 3f);

	// Token: 0x04002A49 RID: 10825
	[ClientVar(Help = "Brightness of colors, from 0 to 1 (note: may affect color accuracy)", Saved = true)]
	public static float Brightness = 1f;

	// Token: 0x04002A4A RID: 10826
	public Color defaultColor;

	// Token: 0x04002A4B RID: 10827
	public Color buildingPrivilegeColor;

	// Token: 0x04002A4C RID: 10828
	public Color coldColor;

	// Token: 0x04002A4D RID: 10829
	public Color hotColor;

	// Token: 0x04002A4E RID: 10830
	public Color hurtColor;

	// Token: 0x04002A4F RID: 10831
	public Color healedColor;

	// Token: 0x04002A50 RID: 10832
	public Color irradiatedColor;

	// Token: 0x04002A51 RID: 10833
	public Color comfortedColor;
}
