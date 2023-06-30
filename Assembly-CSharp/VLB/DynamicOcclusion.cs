using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B3 RID: 2483
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	public class DynamicOcclusion : MonoBehaviour
	{
		// Token: 0x06003B2E RID: 15150 RVA: 0x0015E0E5 File Offset: 0x0015C2E5
		private void OnValidate()
		{
			this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0f);
			this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
		}

		// Token: 0x06003B2F RID: 15151 RVA: 0x0015E111 File Offset: 0x0015C311
		private void OnEnable()
		{
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x0015E12F File Offset: 0x0015C32F
		private void OnDisable()
		{
			this.SetHitNull();
		}

		// Token: 0x06003B31 RID: 15153 RVA: 0x0015E138 File Offset: 0x0015C338
		private void Start()
		{
			if (Application.isPlaying)
			{
				TriggerZone component = base.GetComponent<TriggerZone>();
				if (component)
				{
					this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x0015E171 File Offset: 0x0015C371
		private void LateUpdate()
		{
			if (this.m_FrameCountToWait <= 0)
			{
				this.ProcessRaycasts();
				this.m_FrameCountToWait = this.waitFrameCount;
			}
			this.m_FrameCountToWait--;
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x0015E19C File Offset: 0x0015C39C
		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			float num = angleDiff * 0.5f;
			return Quaternion.Euler(UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num)) * direction;
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x0015E1D4 File Offset: 0x0015C3D4
		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, this.layerMask.value);
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].collider.isTrigger && array[i].collider.bounds.GetMaxArea2D() >= this.minOccluderArea && array[i].distance < num2)
				{
					num2 = array[i].distance;
					num = i;
				}
			}
			if (num != -1)
			{
				return array[num];
			}
			return default(RaycastHit);
		}

		// Token: 0x06003B35 RID: 15157 RVA: 0x0015E284 File Offset: 0x0015C484
		private Vector3 GetDirection(uint dirInt)
		{
			dirInt %= (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length;
			switch (dirInt)
			{
			case 0U:
				return base.transform.up;
			case 1U:
				return base.transform.right;
			case 2U:
				return -base.transform.up;
			case 3U:
				return -base.transform.right;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06003B36 RID: 15158 RVA: 0x0015E300 File Offset: 0x0015C500
		private bool IsHitValid(RaycastHit hit)
		{
			return hit.collider && Vector3.Dot(hit.normal, -base.transform.forward) >= this.maxSurfaceDot;
		}

		// Token: 0x06003B37 RID: 15159 RVA: 0x0015E33C File Offset: 0x0015C53C
		private void ProcessRaycasts()
		{
			RaycastHit raycastHit = this.GetBestHit(base.transform.position, base.transform.forward);
			if (this.IsHitValid(raycastHit))
			{
				if (this.minSurfaceRatio > 0.5f)
				{
					for (uint num = 0U; num < (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length; num += 1U)
					{
						Vector3 direction = this.GetDirection(num + this.m_PrevNonSubHitDirectionId);
						Vector3 vector = base.transform.position + direction * this.m_Master.coneRadiusStart * (this.minSurfaceRatio * 2f - 1f);
						Vector3 vector2 = base.transform.position + base.transform.forward * this.m_Master.fadeEnd + direction * this.m_Master.coneRadiusEnd * (this.minSurfaceRatio * 2f - 1f);
						RaycastHit bestHit = this.GetBestHit(vector, vector2 - vector);
						if (!this.IsHitValid(bestHit))
						{
							this.m_PrevNonSubHitDirectionId = num;
							this.SetHitNull();
							return;
						}
						if (bestHit.distance > raycastHit.distance)
						{
							raycastHit = bestHit;
						}
					}
				}
				this.SetHit(raycastHit);
				return;
			}
			this.SetHitNull();
		}

		// Token: 0x06003B38 RID: 15160 RVA: 0x0015E498 File Offset: 0x0015C698
		private void SetHit(RaycastHit hit)
		{
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment != PlaneAlignment.Surface && planeAlignment == PlaneAlignment.Beam)
			{
				this.SetClippingPlane(new Plane(-base.transform.forward, hit.point));
				return;
			}
			this.SetClippingPlane(new Plane(hit.normal, hit.point));
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x0015E4EF File Offset: 0x0015C6EF
		private void SetHitNull()
		{
			this.SetClippingPlaneOff();
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x0015E4F7 File Offset: 0x0015C6F7
		private void SetClippingPlane(Plane planeWS)
		{
			planeWS = planeWS.TranslateCustom(planeWS.normal * this.planeOffset);
			this.m_Master.SetClippingPlane(planeWS);
		}

		// Token: 0x06003B3B RID: 15163 RVA: 0x0015E51F File Offset: 0x0015C71F
		private void SetClippingPlaneOff()
		{
			this.m_Master.SetClippingPlaneOff();
		}

		// Token: 0x04003623 RID: 13859
		public LayerMask layerMask = -1;

		// Token: 0x04003624 RID: 13860
		public float minOccluderArea;

		// Token: 0x04003625 RID: 13861
		public int waitFrameCount = 3;

		// Token: 0x04003626 RID: 13862
		public float minSurfaceRatio = 0.5f;

		// Token: 0x04003627 RID: 13863
		public float maxSurfaceDot = 0.25f;

		// Token: 0x04003628 RID: 13864
		public PlaneAlignment planeAlignment;

		// Token: 0x04003629 RID: 13865
		public float planeOffset = 0.1f;

		// Token: 0x0400362A RID: 13866
		private VolumetricLightBeam m_Master;

		// Token: 0x0400362B RID: 13867
		private int m_FrameCountToWait;

		// Token: 0x0400362C RID: 13868
		private float m_RangeMultiplier = 1f;

		// Token: 0x0400362D RID: 13869
		private uint m_PrevNonSubHitDirectionId;

		// Token: 0x02000EF3 RID: 3827
		private enum Direction
		{
			// Token: 0x04004E13 RID: 19987
			Up,
			// Token: 0x04004E14 RID: 19988
			Right,
			// Token: 0x04004E15 RID: 19989
			Down,
			// Token: 0x04004E16 RID: 19990
			Left
		}
	}
}
