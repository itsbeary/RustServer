using System;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class ExcavatorServerEffects : MonoBehaviour
{
	// Token: 0x060024A0 RID: 9376 RVA: 0x000E8C17 File Offset: 0x000E6E17
	public void Awake()
	{
		ExcavatorServerEffects.instance = this;
		ExcavatorServerEffects.SetMining(false, true);
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000E8C26 File Offset: 0x000E6E26
	public void OnDestroy()
	{
		ExcavatorServerEffects.instance = null;
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x000E8C30 File Offset: 0x000E6E30
	public static void SetMining(bool isMining, bool force = false)
	{
		if (ExcavatorServerEffects.instance == null)
		{
			return;
		}
		foreach (TriggerBase triggerBase in ExcavatorServerEffects.instance.miningTriggers)
		{
			if (!(triggerBase == null))
			{
				triggerBase.gameObject.SetActive(isMining);
			}
		}
	}

	// Token: 0x04001C85 RID: 7301
	public static ExcavatorServerEffects instance;

	// Token: 0x04001C86 RID: 7302
	public TriggerBase[] miningTriggers;
}
