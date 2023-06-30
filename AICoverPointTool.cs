using System;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class AICoverPointTool : MonoBehaviour
{
	// Token: 0x0600195C RID: 6492 RVA: 0x000BA238 File Offset: 0x000B8438
	[ContextMenu("Place Cover Points")]
	public void PlaceCoverPoints()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.DestroyImmediate(((Transform)obj).gameObject);
		}
		Vector3 vector = new Vector3(base.transform.position.x - 50f, base.transform.position.y, base.transform.position.z - 50f);
		for (int i = 0; i < 50; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				AICoverPointTool.TestResult testResult = this.TestPoint(vector);
				if (testResult.Valid)
				{
					this.PlacePoint(testResult);
				}
				vector.x += 2f;
			}
			vector.x -= 100f;
			vector.z += 2f;
		}
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x000BA340 File Offset: 0x000B8540
	private AICoverPointTool.TestResult TestPoint(Vector3 pos)
	{
		pos.y += 0.5f;
		AICoverPointTool.TestResult testResult = default(AICoverPointTool.TestResult);
		testResult.Position = pos;
		if (this.HitsCover(new Ray(pos, Vector3.forward), 1218519041, 1f))
		{
			testResult.Forward = true;
			testResult.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.right), 1218519041, 1f))
		{
			testResult.Right = true;
			testResult.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.back), 1218519041, 1f))
		{
			testResult.Backward = true;
			testResult.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.left), 1218519041, 1f))
		{
			testResult.Left = true;
			testResult.Valid = true;
		}
		return testResult;
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x000BA424 File Offset: 0x000B8624
	private void PlacePoint(AICoverPointTool.TestResult result)
	{
		if (result.Forward)
		{
			this.PlacePoint(result.Position, Vector3.forward);
		}
		if (result.Right)
		{
			this.PlacePoint(result.Position, Vector3.right);
		}
		if (result.Backward)
		{
			this.PlacePoint(result.Position, Vector3.back);
		}
		if (result.Left)
		{
			this.PlacePoint(result.Position, Vector3.left);
		}
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000BA495 File Offset: 0x000B8695
	private void PlacePoint(Vector3 pos, Vector3 dir)
	{
		AICoverPoint aicoverPoint = new GameObject("CP").AddComponent<AICoverPoint>();
		aicoverPoint.transform.position = pos;
		aicoverPoint.transform.forward = dir;
		aicoverPoint.transform.SetParent(base.transform);
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x000BA4D0 File Offset: 0x000B86D0
	public bool HitsCover(Ray ray, int layerMask, float maxDistance)
	{
		RaycastHit raycastHit;
		return !ray.origin.IsNaNOrInfinity() && !ray.direction.IsNaNOrInfinity() && !(ray.direction == Vector3.zero) && GamePhysics.Trace(ray, 0f, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null);
	}

	// Token: 0x02000C4D RID: 3149
	private struct TestResult
	{
		// Token: 0x0400434A RID: 17226
		public Vector3 Position;

		// Token: 0x0400434B RID: 17227
		public bool Valid;

		// Token: 0x0400434C RID: 17228
		public bool Forward;

		// Token: 0x0400434D RID: 17229
		public bool Right;

		// Token: 0x0400434E RID: 17230
		public bool Backward;

		// Token: 0x0400434F RID: 17231
		public bool Left;
	}
}
