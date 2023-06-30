using System;
using UnityEngine;

// Token: 0x02000463 RID: 1123
public class TreeMarkerData : PrefabAttribute, IServerComponent
{
	// Token: 0x0600253F RID: 9535 RVA: 0x000EB632 File Offset: 0x000E9832
	protected override Type GetIndexedType()
	{
		return typeof(TreeMarkerData);
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000EB640 File Offset: 0x000E9840
	public Vector3 GetNearbyPoint(Vector3 point, ref int ignoreIndex, out Vector3 normal)
	{
		int num = this.Markers.Length;
		if (ignoreIndex != -1 && this.ProcessAngleChecks)
		{
			ignoreIndex++;
			if (ignoreIndex >= num)
			{
				ignoreIndex = 0;
			}
			normal = this.Markers[ignoreIndex].LocalNormal;
			return this.Markers[ignoreIndex].LocalPosition;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		float num3 = float.MaxValue;
		int num4 = -1;
		for (int i = 0; i < num; i++)
		{
			if (ignoreIndex != num2)
			{
				TreeMarkerData.MarkerLocation markerLocation = this.Markers[num2];
				if (markerLocation.LocalPosition.y >= this.MinY)
				{
					Vector3 localPosition = markerLocation.LocalPosition;
					localPosition.y = Mathf.Lerp(localPosition.y, point.y, 0.5f);
					float num5 = (localPosition - point).sqrMagnitude;
					num5 *= UnityEngine.Random.Range(0.95f, 1.05f);
					if (num5 < num3)
					{
						num3 = num5;
						num4 = num2;
					}
					num2++;
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
			}
		}
		if (num4 > -1)
		{
			normal = this.Markers[num4].LocalNormal;
			ignoreIndex = num4;
			return this.Markers[num4].LocalPosition;
		}
		normal = this.Markers[0].LocalNormal;
		return this.Markers[0].LocalPosition;
	}

	// Token: 0x04001D73 RID: 7539
	public TreeMarkerData.GenerationArc[] GenerationArcs;

	// Token: 0x04001D74 RID: 7540
	public TreeMarkerData.MarkerLocation[] Markers;

	// Token: 0x04001D75 RID: 7541
	public Vector3 GenerationStartPoint = Vector3.up * 2f;

	// Token: 0x04001D76 RID: 7542
	public float GenerationRadius = 2f;

	// Token: 0x04001D77 RID: 7543
	public float MaxY = 1.7f;

	// Token: 0x04001D78 RID: 7544
	public float MinY = 0.2f;

	// Token: 0x04001D79 RID: 7545
	public bool ProcessAngleChecks;

	// Token: 0x02000D06 RID: 3334
	[Serializable]
	public struct MarkerLocation
	{
		// Token: 0x04004687 RID: 18055
		public Vector3 LocalPosition;

		// Token: 0x04004688 RID: 18056
		public Vector3 LocalNormal;
	}

	// Token: 0x02000D07 RID: 3335
	[Serializable]
	public struct GenerationArc
	{
		// Token: 0x04004689 RID: 18057
		public Vector3 CentrePoint;

		// Token: 0x0400468A RID: 18058
		public float Radius;

		// Token: 0x0400468B RID: 18059
		public Vector3 Rotation;

		// Token: 0x0400468C RID: 18060
		public int OverrideCount;
	}
}
