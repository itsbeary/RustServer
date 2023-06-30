using System;
using UnityEngine;

// Token: 0x02000613 RID: 1555
[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	// Token: 0x06002E1A RID: 11802 RVA: 0x001154E0 File Offset: 0x001136E0
	public MaterialEffect.Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		foreach (MaterialEffect.Entry entry in this.Entries)
		{
			if (entry.Material == mat)
			{
				return entry;
			}
		}
		return null;
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x00115518 File Offset: 0x00113718
	public MaterialEffect.Entry GetWaterEntry()
	{
		if (this.waterFootstepIndex == -1)
		{
			for (int i = 0; i < this.Entries.Length; i++)
			{
				if (this.Entries[i].Material.name == "Water")
				{
					this.waterFootstepIndex = i;
					break;
				}
			}
		}
		if (this.waterFootstepIndex != -1)
		{
			return this.Entries[this.waterFootstepIndex];
		}
		Debug.LogWarning("Unable to find water effect for :" + base.name);
		return null;
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x00115598 File Offset: 0x00113798
	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3), float speed = 0f)
	{
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, QueryTriggerInteraction.UseGlobal, null))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward, Effect.Type.Generic);
			if (this.DefaultSoundDefinition != null)
			{
				this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
			}
			return;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(ray.origin, true, false, null, false);
		if (!waterInfo.isValid)
		{
			PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
			MaterialEffect.Entry entryFromMaterial = this.GetEntryFromMaterial(materialAt);
			if (entryFromMaterial == null)
			{
				Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (this.DefaultSoundDefinition != null)
				{
					this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
					return;
				}
			}
			else
			{
				Effect.client.Run(entryFromMaterial.Effect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (entryFromMaterial.SoundDefinition != null)
				{
					this.PlaySound(entryFromMaterial.SoundDefinition, raycastHit.point, speed);
				}
			}
			return;
		}
		Vector3 vector = new Vector3(ray.origin.x, WaterSystem.GetHeight(ray.origin), ray.origin.z);
		MaterialEffect.Entry waterEntry = this.GetWaterEntry();
		if (this.submergedWaterDepth > 0f && waterInfo.currentDepth >= this.submergedWaterDepth)
		{
			waterEntry = this.submergedWaterEntry;
		}
		else if (this.deepWaterDepth > 0f && waterInfo.currentDepth >= this.deepWaterDepth)
		{
			waterEntry = this.deepWaterEntry;
		}
		if (waterEntry == null)
		{
			return;
		}
		Effect.client.Run(waterEntry.Effect.resourcePath, vector, Vector3.up, default(Vector3), Effect.Type.Generic);
		if (waterEntry.SoundDefinition != null)
		{
			this.PlaySound(waterEntry.SoundDefinition, vector, speed);
		}
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlaySound(SoundDefinition definition, Vector3 position, float velocity = 0f)
	{
	}

	// Token: 0x040025CF RID: 9679
	public GameObjectRef DefaultEffect;

	// Token: 0x040025D0 RID: 9680
	public SoundDefinition DefaultSoundDefinition;

	// Token: 0x040025D1 RID: 9681
	public MaterialEffect.Entry[] Entries;

	// Token: 0x040025D2 RID: 9682
	public int waterFootstepIndex = -1;

	// Token: 0x040025D3 RID: 9683
	public MaterialEffect.Entry deepWaterEntry;

	// Token: 0x040025D4 RID: 9684
	public float deepWaterDepth = -1f;

	// Token: 0x040025D5 RID: 9685
	public MaterialEffect.Entry submergedWaterEntry;

	// Token: 0x040025D6 RID: 9686
	public float submergedWaterDepth = -1f;

	// Token: 0x040025D7 RID: 9687
	public bool ScaleVolumeWithSpeed;

	// Token: 0x040025D8 RID: 9688
	public AnimationCurve SpeedGainCurve;

	// Token: 0x02000DA5 RID: 3493
	[Serializable]
	public class Entry
	{
		// Token: 0x040048BC RID: 18620
		public PhysicMaterial Material;

		// Token: 0x040048BD RID: 18621
		public GameObjectRef Effect;

		// Token: 0x040048BE RID: 18622
		public SoundDefinition SoundDefinition;
	}
}
