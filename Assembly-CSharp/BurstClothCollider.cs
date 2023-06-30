using System;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class BurstClothCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x060015A3 RID: 5539 RVA: 0x000AB028 File Offset: 0x000A9228
	public CapsuleParams GetParams()
	{
		Vector3 position = base.transform.position;
		float num = this.Height / 2f;
		Vector3 vector = base.transform.rotation * Vector3.up;
		Vector3 vector2 = position + vector * num;
		Vector3 vector3 = position - vector * num;
		return new CapsuleParams
		{
			Transform = base.transform,
			PointA = base.transform.InverseTransformPoint(vector2),
			PointB = base.transform.InverseTransformPoint(vector3),
			Radius = this.Radius
		};
	}

	// Token: 0x04000DD5 RID: 3541
	public float Height;

	// Token: 0x04000DD6 RID: 3542
	public float Radius;
}
