using System;
using ConVar;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public class TriggerTemperature : TriggerBase
{
	// Token: 0x06002BFA RID: 11258 RVA: 0x0010A4CC File Offset: 0x001086CC
	private void OnValidate()
	{
		if (base.GetComponent<SphereCollider>() != null)
		{
			this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
			return;
		}
		Vector3 vector = Vector3.Scale(base.GetComponent<BoxCollider>().size, base.transform.localScale);
		this.triggerSize = vector.Max() * 0.5f;
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x0010A538 File Offset: 0x00108738
	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		if (this.sunlightBlocker)
		{
			float time = Env.time;
			if (time >= this.blockMinHour && time <= this.blockMaxHour)
			{
				Vector3 position2 = TOD_Sky.Instance.Components.SunTransform.position;
				if (!GamePhysics.LineOfSight(position, position2, 256, null))
				{
					return oldTemperature - this.sunlightBlockAmount;
				}
			}
			return oldTemperature;
		}
		float num = Vector3.Distance(base.gameObject.transform.position, position);
		float num2 = Mathf.InverseLerp(this.triggerSize, this.minSize, num);
		return Mathf.Lerp(oldTemperature, this.Temperature, num2);
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x0010A5CC File Offset: 0x001087CC
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x040023BC RID: 9148
	public float Temperature = 50f;

	// Token: 0x040023BD RID: 9149
	public float triggerSize;

	// Token: 0x040023BE RID: 9150
	public float minSize;

	// Token: 0x040023BF RID: 9151
	public bool sunlightBlocker;

	// Token: 0x040023C0 RID: 9152
	public float sunlightBlockAmount;

	// Token: 0x040023C1 RID: 9153
	[Range(0f, 24f)]
	public float blockMinHour = 8.5f;

	// Token: 0x040023C2 RID: 9154
	[Range(0f, 24f)]
	public float blockMaxHour = 18.5f;
}
