using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200070E RID: 1806
public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x060032E7 RID: 13031 RVA: 0x00138EB5 File Offset: 0x001370B5
	public static void Reset()
	{
		WaterVisibilityTrigger.ticks = 1L;
		WaterVisibilityTrigger.tracker.Clear();
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x00138EC8 File Offset: 0x001370C8
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ToggleVisibility()
	{
	}

	// Token: 0x060032EA RID: 13034 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ResetVisibility()
	{
	}

	// Token: 0x060032EB RID: 13035 RVA: 0x00138EE3 File Offset: 0x001370E3
	private void ToggleCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, true);
		}
	}

	// Token: 0x060032EC RID: 13036 RVA: 0x00138F11 File Offset: 0x00137111
	private void ResetCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, false);
		}
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x00138F40 File Offset: 0x00137140
	protected void OnTriggerEnter(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && !WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			long num = WaterVisibilityTrigger.ticks;
			WaterVisibilityTrigger.ticks = num + 1L;
			this.enteredTick = num;
			WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
			this.ToggleVisibility();
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ToggleCollision(other);
		}
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x00138FC0 File Offset: 0x001371C0
	protected void OnTriggerExit(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
			if (WaterVisibilityTrigger.tracker.Count > 0)
			{
				WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
			}
			else
			{
				this.ResetVisibility();
			}
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ResetCollision(other);
		}
	}

	// Token: 0x040029A2 RID: 10658
	public bool togglePhysics = true;

	// Token: 0x040029A3 RID: 10659
	public bool toggleVisuals = true;

	// Token: 0x040029A4 RID: 10660
	private long enteredTick;

	// Token: 0x040029A5 RID: 10661
	private static long ticks = 1L;

	// Token: 0x040029A6 RID: 10662
	private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();
}
