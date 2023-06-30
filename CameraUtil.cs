using System;
using UnityEngine;

// Token: 0x02000930 RID: 2352
public static class CameraUtil
{
	// Token: 0x06003860 RID: 14432 RVA: 0x0014F674 File Offset: 0x0014D874
	public static void NormalizePlane(ref Plane plane)
	{
		float num = 1f / plane.normal.magnitude;
		plane.normal *= num;
		plane.distance *= num;
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x0014F6B8 File Offset: 0x0014D8B8
	public static void ExtractPlanes(Camera camera, ref Plane[] planes)
	{
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		CameraUtil.ExtractPlanes(GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * worldToCameraMatrix, ref planes);
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x0014F6E4 File Offset: 0x0014D8E4
	public static void ExtractPlanes(Matrix4x4 viewProjMatrix, ref Plane[] planes)
	{
		planes[0].normal = new Vector3(viewProjMatrix.m30 + viewProjMatrix.m00, viewProjMatrix.m31 + viewProjMatrix.m01, viewProjMatrix.m32 + viewProjMatrix.m02);
		planes[0].distance = viewProjMatrix.m33 + viewProjMatrix.m03;
		CameraUtil.NormalizePlane(ref planes[0]);
		planes[1].normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m00, viewProjMatrix.m31 - viewProjMatrix.m01, viewProjMatrix.m32 - viewProjMatrix.m02);
		planes[1].distance = viewProjMatrix.m33 - viewProjMatrix.m03;
		CameraUtil.NormalizePlane(ref planes[1]);
		planes[2].normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m10, viewProjMatrix.m31 - viewProjMatrix.m11, viewProjMatrix.m32 - viewProjMatrix.m12);
		planes[2].distance = viewProjMatrix.m33 - viewProjMatrix.m13;
		CameraUtil.NormalizePlane(ref planes[2]);
		planes[3].normal = new Vector3(viewProjMatrix.m30 + viewProjMatrix.m10, viewProjMatrix.m31 + viewProjMatrix.m11, viewProjMatrix.m32 + viewProjMatrix.m12);
		planes[3].distance = viewProjMatrix.m33 + viewProjMatrix.m13;
		CameraUtil.NormalizePlane(ref planes[3]);
		planes[4].normal = new Vector3(viewProjMatrix.m20, viewProjMatrix.m21, viewProjMatrix.m22);
		planes[4].distance = viewProjMatrix.m23;
		CameraUtil.NormalizePlane(ref planes[4]);
		planes[5].normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m20, viewProjMatrix.m31 - viewProjMatrix.m21, viewProjMatrix.m32 - viewProjMatrix.m22);
		planes[5].distance = viewProjMatrix.m33 - viewProjMatrix.m23;
		CameraUtil.NormalizePlane(ref planes[5]);
	}
}
