using System;
using UnityEngine;

// Token: 0x02000922 RID: 2338
public static class BoundsEx
{
	// Token: 0x06003838 RID: 14392 RVA: 0x0014E634 File Offset: 0x0014C834
	public static Bounds XZ3D(this Bounds bounds)
	{
		return new Bounds(bounds.center.XZ3D(), bounds.size.XZ3D());
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x0014E654 File Offset: 0x0014C854
	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		Vector3 vector = matrix.MultiplyPoint3x4(bounds.center);
		Vector3 extents = bounds.extents;
		Vector3 vector2 = matrix.MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 vector3 = matrix.MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 vector4 = matrix.MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = Mathf.Abs(vector2.x) + Mathf.Abs(vector3.x) + Mathf.Abs(vector4.x);
		extents.y = Mathf.Abs(vector2.y) + Mathf.Abs(vector3.y) + Mathf.Abs(vector4.y);
		extents.z = Mathf.Abs(vector2.z) + Mathf.Abs(vector3.z) + Mathf.Abs(vector4.z);
		return new Bounds
		{
			center = vector,
			extents = extents
		};
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x0014E76C File Offset: 0x0014C96C
	public static Rect ToScreenRect(this Bounds b, Camera cam)
	{
		Rect rect;
		using (TimeWarning.New("Bounds.ToScreenRect", 0))
		{
			BoundsEx.pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
			Vector3 vector = BoundsEx.pts[0];
			Vector3 vector2 = BoundsEx.pts[0];
			for (int i = 1; i < BoundsEx.pts.Length; i++)
			{
				vector = Vector3.Min(vector, BoundsEx.pts[i]);
				vector2 = Vector3.Max(vector2, BoundsEx.pts[i]);
			}
			rect = Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
		}
		return rect;
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x0014EB24 File Offset: 0x0014CD24
	public static Rect ToCanvasRect(this Bounds b, RectTransform target, Camera cam)
	{
		Rect rect = b.ToScreenRect(cam);
		rect.min = rect.min.ToCanvas(target, null);
		rect.max = rect.max.ToCanvas(target, null);
		return rect;
	}

	// Token: 0x04003398 RID: 13208
	private static Vector3[] pts = new Vector3[8];
}
