using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class PlacementTest : MonoBehaviour
{
	// Token: 0x060017B7 RID: 6071 RVA: 0x000B38F8 File Offset: 0x000B1AF8
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 vector = new Vector3(insideUnitCircle.x * degreesOffset, UnityEngine.Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		return (input + vector).normalized;
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x000B395C File Offset: 0x000B1B5C
	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 normalized = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y).normalized;
		return new Vector3(normalized.x * distance, UnityEngine.Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x000B39AC File Offset: 0x000B1BAC
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec.normalized;
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x000B3A40 File Offset: 0x000B1C40
	private void Update()
	{
		if (Time.realtimeSinceStartup < this.nextTest)
		{
			return;
		}
		this.nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 vector = this.RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		vector = base.transform.TransformPoint(vector);
		this.testTransform.transform.position = vector;
		if (this.testTransform != null && this.visualTest != null)
		{
			Vector3 vector2 = base.transform.position;
			RaycastHit raycastHit;
			if (this.myMeshCollider.Raycast(new Ray(this.testTransform.position, (base.transform.position - this.testTransform.position).normalized), out raycastHit, 5f))
			{
				vector2 = raycastHit.point;
			}
			else
			{
				Debug.LogError("Missed");
			}
			this.visualTest.transform.position = vector2;
		}
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmos()
	{
	}

	// Token: 0x04001080 RID: 4224
	public MeshCollider myMeshCollider;

	// Token: 0x04001081 RID: 4225
	public Transform testTransform;

	// Token: 0x04001082 RID: 4226
	public Transform visualTest;

	// Token: 0x04001083 RID: 4227
	public float hemisphere = 45f;

	// Token: 0x04001084 RID: 4228
	public float clampTest = 45f;

	// Token: 0x04001085 RID: 4229
	public float testDist = 2f;

	// Token: 0x04001086 RID: 4230
	private float nextTest;
}
