using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200095A RID: 2394
public class WorldSpline : MonoBehaviour
{
	// Token: 0x06003964 RID: 14692 RVA: 0x00154608 File Offset: 0x00152808
	public WorldSplineData GetData()
	{
		WorldSplineData worldSplineData;
		if (WorldSplineSharedData.TryGetDataFor(this, out worldSplineData))
		{
			return worldSplineData;
		}
		if (Application.isPlaying && this.privateData == null)
		{
			this.privateData = new WorldSplineData(this);
		}
		return this.privateData;
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x00154642 File Offset: 0x00152842
	public void SetAll(Vector3[] points, Vector3[] tangents, float lutInterval)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = lutInterval;
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x0015465C File Offset: 0x0015285C
	public void CheckValidity()
	{
		this.lutInterval = Mathf.Clamp(this.lutInterval, 0.05f, 100f);
		if (this.points == null || this.points.Length < 2)
		{
			this.points = new Vector3[2];
			this.points[0] = Vector3.zero;
			this.points[1] = Vector3.zero;
		}
		if (this.tangents == null || this.points.Length != this.tangents.Length)
		{
			Vector3[] array = new Vector3[this.points.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (this.tangents != null && i < this.tangents.Length)
				{
					array[i] = this.tangents[i];
				}
				else
				{
					array[i] = Vector3.forward;
				}
			}
			this.tangents = array;
		}
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x00154736 File Offset: 0x00152936
	protected virtual void OnDrawGizmosSelected()
	{
		if (this.showGizmos)
		{
			WorldSpline.DrawSplineGizmo(this, Color.magenta);
		}
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x0015474C File Offset: 0x0015294C
	protected static void DrawSplineGizmo(WorldSpline ws, Color splineColour)
	{
		if (ws == null)
		{
			return;
		}
		WorldSplineData data = ws.GetData();
		if (data == null)
		{
			return;
		}
		if (ws.points.Length < 2 || ws.points.Length != ws.tangents.Length)
		{
			return;
		}
		Vector3[] pointsWorld = ws.GetPointsWorld();
		Vector3[] tangentsWorld = ws.GetTangentsWorld();
		for (int i = 0; i < pointsWorld.Length; i++)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(pointsWorld[i], 0.25f);
			if (tangentsWorld[i].magnitude > 0f)
			{
				Gizmos.color = Color.cyan;
				Vector3 vector = pointsWorld[i] + tangentsWorld[i] + Vector3.up * 0.1f;
				Gizmos.DrawLine(pointsWorld[i] + Vector3.up * 0.1f, vector);
			}
		}
		Gizmos.color = splineColour;
		Vector3[] visualSpline = WorldSpline.GetVisualSpline(ws, data, 1f);
		for (int j = 0; j < visualSpline.Length - 1; j++)
		{
			Gizmos.color = Color.Lerp(Color.white, splineColour, (float)j / (float)(visualSpline.Length - 1));
			Gizmos.DrawLine(visualSpline[j], visualSpline[j + 1]);
			Gizmos.DrawLine(visualSpline[j], visualSpline[j] + Vector3.up * 0.25f);
		}
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x001548BC File Offset: 0x00152ABC
	private static Vector3[] GetVisualSpline(WorldSpline ws, WorldSplineData data, float distBetweenPoints)
	{
		WorldSpline.visualSplineList.Clear();
		if (ws != null && ws.points.Length > 1)
		{
			Vector3 startPointWorld = ws.GetStartPointWorld();
			Vector3 endPointWorld = ws.GetEndPointWorld();
			WorldSpline.visualSplineList.Add(startPointWorld);
			for (float num = distBetweenPoints; num <= data.Length - distBetweenPoints; num += distBetweenPoints)
			{
				WorldSpline.visualSplineList.Add(ws.GetPointCubicHermiteWorld(num, data));
			}
			WorldSpline.visualSplineList.Add(endPointWorld);
		}
		return WorldSpline.visualSplineList.ToArray();
	}

	// Token: 0x0600396A RID: 14698 RVA: 0x0015493A File Offset: 0x00152B3A
	public Vector3 GetStartPointWorld()
	{
		return base.transform.TransformPoint(this.points[0]);
	}

	// Token: 0x0600396B RID: 14699 RVA: 0x00154953 File Offset: 0x00152B53
	public Vector3 GetEndPointWorld()
	{
		return base.transform.TransformPoint(this.points[this.points.Length - 1]);
	}

	// Token: 0x0600396C RID: 14700 RVA: 0x00154975 File Offset: 0x00152B75
	public Vector3 GetStartTangentWorld()
	{
		return Vector3.Scale(base.transform.rotation * this.tangents[0], base.transform.localScale);
	}

	// Token: 0x0600396D RID: 14701 RVA: 0x001549A3 File Offset: 0x00152BA3
	public Vector3 GetEndTangentWorld()
	{
		return Vector3.Scale(base.transform.rotation * this.tangents[this.tangents.Length - 1], base.transform.localScale);
	}

	// Token: 0x0600396E RID: 14702 RVA: 0x001549DA File Offset: 0x00152BDA
	public Vector3 GetTangentCubicHermiteWorld(float distance)
	{
		return Vector3.Scale(base.transform.rotation * this.GetData().GetTangentCubicHermite(distance), base.transform.localScale);
	}

	// Token: 0x0600396F RID: 14703 RVA: 0x00154A08 File Offset: 0x00152C08
	public Vector3 GetTangentCubicHermiteWorld(float distance, WorldSplineData data)
	{
		return Vector3.Scale(base.transform.rotation * data.GetTangentCubicHermite(distance), base.transform.localScale);
	}

	// Token: 0x06003970 RID: 14704 RVA: 0x00154A31 File Offset: 0x00152C31
	public Vector3 GetPointCubicHermiteWorld(float distance)
	{
		return base.transform.TransformPoint(this.GetData().GetPointCubicHermite(distance));
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x00154A4A File Offset: 0x00152C4A
	public Vector3 GetPointCubicHermiteWorld(float distance, WorldSplineData data)
	{
		return base.transform.TransformPoint(data.GetPointCubicHermite(distance));
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x00154A60 File Offset: 0x00152C60
	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermite = this.GetData().GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = base.transform.TransformVector(tangent);
		return base.transform.TransformPoint(pointAndTangentCubicHermite);
	}

	// Token: 0x06003973 RID: 14707 RVA: 0x00154AA0 File Offset: 0x00152CA0
	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, WorldSplineData data, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermite = data.GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = base.transform.TransformVector(tangent);
		return base.transform.TransformPoint(pointAndTangentCubicHermite);
	}

	// Token: 0x06003974 RID: 14708 RVA: 0x00154AD9 File Offset: 0x00152CD9
	public Vector3[] GetPointsWorld()
	{
		return WorldSpline.PointsToWorld(this.points, base.transform);
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x00154AEC File Offset: 0x00152CEC
	public Vector3[] GetTangentsWorld()
	{
		return WorldSpline.TangentsToWorld(this.tangents, base.transform);
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x00154B00 File Offset: 0x00152D00
	private static Vector3[] PointsToWorld(Vector3[] points, Transform tr)
	{
		Vector3[] array = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = tr.TransformPoint(points[i]);
		}
		return array;
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x00154B3C File Offset: 0x00152D3C
	private static Vector3[] TangentsToWorld(Vector3[] tangents, Transform tr)
	{
		Vector3[] array = new Vector3[tangents.Length];
		for (int i = 0; i < tangents.Length; i++)
		{
			array[i] = Vector3.Scale(tr.rotation * tangents[i], tr.localScale);
		}
		return array;
	}

	// Token: 0x040033EE RID: 13294
	public int dataIndex = -1;

	// Token: 0x040033EF RID: 13295
	public Vector3[] points;

	// Token: 0x040033F0 RID: 13296
	public Vector3[] tangents;

	// Token: 0x040033F1 RID: 13297
	[Range(0.05f, 100f)]
	public float lutInterval = 0.25f;

	// Token: 0x040033F2 RID: 13298
	[SerializeField]
	private bool showGizmos = true;

	// Token: 0x040033F3 RID: 13299
	private static List<Vector3> visualSplineList = new List<Vector3>();

	// Token: 0x040033F4 RID: 13300
	private WorldSplineData privateData;
}
