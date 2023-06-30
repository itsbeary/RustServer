using System;
using UnityEngine;

// Token: 0x0200047A RID: 1146
public class TugboatSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E05 RID: 7685
	[SerializeField]
	private Tugboat tugboat;

	// Token: 0x04001E06 RID: 7686
	[SerializeField]
	private float roughHalfWidth = 5f;

	// Token: 0x04001E07 RID: 7687
	[SerializeField]
	private float roughHalfLength = 10f;

	// Token: 0x04001E08 RID: 7688
	private float soundCullDistanceSq;

	// Token: 0x04001E09 RID: 7689
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineLoopDef;

	// Token: 0x04001E0A RID: 7690
	private Sound engineLoop;

	// Token: 0x04001E0B RID: 7691
	private SoundModulation.Modulator engineGainMod;

	// Token: 0x04001E0C RID: 7692
	private SoundModulation.Modulator enginePitchMod;

	// Token: 0x04001E0D RID: 7693
	[SerializeField]
	private SoundDefinition engineStartDef;

	// Token: 0x04001E0E RID: 7694
	[SerializeField]
	private SoundDefinition engineStartBridgeDef;

	// Token: 0x04001E0F RID: 7695
	[SerializeField]
	private SoundDefinition engineStopDef;

	// Token: 0x04001E10 RID: 7696
	[SerializeField]
	private SoundDefinition engineStopBridgeDef;

	// Token: 0x04001E11 RID: 7697
	[SerializeField]
	private float engineGainChangeRate = 1f;

	// Token: 0x04001E12 RID: 7698
	[SerializeField]
	private float enginePitchChangeRate = 0.5f;

	// Token: 0x04001E13 RID: 7699
	[SerializeField]
	private Transform engineTransform;

	// Token: 0x04001E14 RID: 7700
	[SerializeField]
	private Transform bridgeControlsTransform;

	// Token: 0x04001E15 RID: 7701
	[Header("Water")]
	[SerializeField]
	private SoundDefinition waterIdleDef;

	// Token: 0x04001E16 RID: 7702
	[SerializeField]
	private SoundDefinition waterSideMovementSlowDef;

	// Token: 0x04001E17 RID: 7703
	[SerializeField]
	private SoundDefinition waterSideMovementFastDef;

	// Token: 0x04001E18 RID: 7704
	[SerializeField]
	private SoundDefinition waterSternMovementDef;

	// Token: 0x04001E19 RID: 7705
	[SerializeField]
	private SoundDefinition waterInteriorIdleDef;

	// Token: 0x04001E1A RID: 7706
	[SerializeField]
	private SoundDefinition waterInteriorDef;

	// Token: 0x04001E1B RID: 7707
	[SerializeField]
	private AnimationCurve waterMovementGainCurve;

	// Token: 0x04001E1C RID: 7708
	[SerializeField]
	private float waterMovementGainChangeRate = 0.5f;

	// Token: 0x04001E1D RID: 7709
	[SerializeField]
	private AnimationCurve waterDistanceGainCurve;

	// Token: 0x04001E1E RID: 7710
	private Sound leftWaterSound;

	// Token: 0x04001E1F RID: 7711
	private SoundModulation.Modulator leftWaterGainMod;

	// Token: 0x04001E20 RID: 7712
	private Sound rightWaterSound;

	// Token: 0x04001E21 RID: 7713
	private SoundModulation.Modulator rightWaterGainMod;

	// Token: 0x04001E22 RID: 7714
	private Sound sternWaterSound;

	// Token: 0x04001E23 RID: 7715
	private SoundModulation.Modulator sternWaterGainMod;

	// Token: 0x04001E24 RID: 7716
	[SerializeField]
	private Transform wakeTransform;

	// Token: 0x04001E25 RID: 7717
	[SerializeField]
	private Vector3 sideSoundLineStern;

	// Token: 0x04001E26 RID: 7718
	[SerializeField]
	private Vector3 sideSoundLineBow;

	// Token: 0x04001E27 RID: 7719
	[Header("Ambient")]
	private Sound ambientIdleSound;

	// Token: 0x04001E28 RID: 7720
	[SerializeField]
	private SoundDefinition ambientActiveLoopDef;

	// Token: 0x04001E29 RID: 7721
	private Sound ambientActiveSound;

	// Token: 0x04001E2A RID: 7722
	[SerializeField]
	private SoundDefinition hullGroanDef;

	// Token: 0x04001E2B RID: 7723
	[SerializeField]
	private float hullGroanCooldown = 1f;

	// Token: 0x04001E2C RID: 7724
	private float lastHullGroan;

	// Token: 0x04001E2D RID: 7725
	[SerializeField]
	private SoundDefinition chainRattleDef;

	// Token: 0x04001E2E RID: 7726
	[SerializeField]
	private float chainRattleCooldown = 1f;

	// Token: 0x04001E2F RID: 7727
	[SerializeField]
	private Transform[] chainRattleLocations;

	// Token: 0x04001E30 RID: 7728
	[SerializeField]
	private float chainRattleAngleDeltaThreshold = 1f;

	// Token: 0x04001E31 RID: 7729
	private float lastChainRattle;

	// Token: 0x04001E32 RID: 7730
	private Line leftSoundLine;

	// Token: 0x04001E33 RID: 7731
	private Line rightSoundLine;

	// Token: 0x04001E34 RID: 7732
	[Header("Runtime")]
	public bool engineOn;

	// Token: 0x04001E35 RID: 7733
	public bool throttleOn;

	// Token: 0x04001E36 RID: 7734
	public bool inWater = true;
}
