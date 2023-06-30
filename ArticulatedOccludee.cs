using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class ArticulatedOccludee : BaseMonoBehaviour
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x000D007A File Offset: 0x000CE27A
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000D0082 File Offset: 0x000CE282
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.UnregisterFromCulling();
		this.ClearVisibility();
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x000D0098 File Offset: 0x000CE298
	private void ClearVisibility()
	{
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
			this.lodGroup = null;
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		this.localOccludee = new OccludeeSphere(-1);
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000D00F4 File Offset: 0x000CE2F4
	public void ProcessVisibility(LODGroup lod)
	{
		this.lodGroup = lod;
		if (lod != null)
		{
			this.renderers = new List<Renderer>(16);
			LOD[] lods = lod.GetLODs();
			for (int i = 0; i < lods.Length; i++)
			{
				foreach (Renderer renderer in lods[i].renderers)
				{
					if (renderer != null)
					{
						this.renderers.Add(renderer);
					}
				}
			}
		}
		this.UpdateCullingBounds();
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x000D0170 File Offset: 0x000CE370
	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x000D0202 File Offset: 0x000CE402
	private void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.Invalidate();
		}
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000D022C File Offset: 0x000CE42C
	public void UpdateCullingBounds()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		bool flag = false;
		int num = ((this.renderers != null) ? this.renderers.Count : 0);
		int num2 = ((this.colliders != null) ? this.colliders.Count : 0);
		if (num > 0 && (num2 == 0 || num < num2))
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i].isVisible)
				{
					Bounds bounds = this.renderers[i].bounds;
					Vector3 min = bounds.min;
					Vector3 max = bounds.max;
					if (!flag)
					{
						vector = min;
						vector2 = max;
						flag = true;
					}
					else
					{
						vector.x = ((vector.x < min.x) ? vector.x : min.x);
						vector.y = ((vector.y < min.y) ? vector.y : min.y);
						vector.z = ((vector.z < min.z) ? vector.z : min.z);
						vector2.x = ((vector2.x > max.x) ? vector2.x : max.x);
						vector2.y = ((vector2.y > max.y) ? vector2.y : max.y);
						vector2.z = ((vector2.z > max.z) ? vector2.z : max.z);
					}
				}
			}
		}
		if (!flag && num2 > 0)
		{
			flag = true;
			vector = this.colliders[0].bounds.min;
			vector2 = this.colliders[0].bounds.max;
			for (int j = 1; j < this.colliders.Count; j++)
			{
				Bounds bounds2 = this.colliders[j].bounds;
				Vector3 min2 = bounds2.min;
				Vector3 max2 = bounds2.max;
				vector.x = ((vector.x < min2.x) ? vector.x : min2.x);
				vector.y = ((vector.y < min2.y) ? vector.y : min2.y);
				vector.z = ((vector.z < min2.z) ? vector.z : min2.z);
				vector2.x = ((vector2.x > max2.x) ? vector2.x : max2.x);
				vector2.y = ((vector2.y > max2.y) ? vector2.y : max2.y);
				vector2.z = ((vector2.z > max2.z) ? vector2.z : max2.z);
			}
		}
		if (flag)
		{
			Vector3 vector3 = vector2 - vector;
			Vector3 vector4 = vector + vector3 * 0.5f;
			float num3 = Mathf.Max(Mathf.Max(vector3.x, vector3.y), vector3.z) * 0.5f;
			OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(vector4, num3);
			if (this.localOccludee.IsRegistered)
			{
				OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
				this.localOccludee.sphere = sphere;
				return;
			}
			bool flag2 = true;
			if (this.lodGroup != null)
			{
				flag2 = this.lodGroup.enabled;
			}
			this.RegisterForCulling(sphere, flag2);
		}
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000D05F4 File Offset: 0x000CE7F4
	protected virtual bool CheckVisibility()
	{
		return this.localOccludee.state == null || this.localOccludee.state.isVisible;
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x000D0618 File Offset: 0x000CE818
	private void ApplyVisibility(bool vis)
	{
		if (this.lodGroup != null)
		{
			float num = (float)(vis ? 0 : 100000);
			if (num != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000D0668 File Offset: 0x000CE868
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (MainCamera.mainCamera != null && this.localOccludee.IsRegistered)
		{
			float num = Vector3.Distance(MainCamera.position, base.transform.position);
			this.VisUpdateUsingCulling(num, visible);
			this.ApplyVisibility(this.isVisible);
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateVisibility(float delay)
	{
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000063A5 File Offset: 0x000045A5
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000D06BC File Offset: 0x000CE8BC
	public virtual void TriggerUpdateVisibilityBounds()
	{
		if (base.enabled)
		{
			float sqrMagnitude = (base.transform.position - MainCamera.position).sqrMagnitude;
			float num = 400f;
			float num2;
			if (sqrMagnitude < num)
			{
				num2 = 1f / UnityEngine.Random.Range(5f, 25f);
			}
			else
			{
				float num3 = Mathf.Clamp01((Mathf.Sqrt(sqrMagnitude) - 20f) * 0.001f);
				float num4 = Mathf.Lerp(0.06666667f, 2f, num3);
				num2 = UnityEngine.Random.Range(num4, num4 + 0.06666667f);
			}
			this.UpdateVisibility(num2);
			this.ApplyVisibility(this.isVisible);
			if (this.TriggerUpdateVisibilityBoundsDelegate == null)
			{
				this.TriggerUpdateVisibilityBoundsDelegate = new Action(this.TriggerUpdateVisibilityBounds);
			}
			base.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, num2);
		}
	}

	// Token: 0x040017AA RID: 6058
	private const float UpdateBoundsFadeStart = 20f;

	// Token: 0x040017AB RID: 6059
	private const float UpdateBoundsFadeLength = 1000f;

	// Token: 0x040017AC RID: 6060
	private const float UpdateBoundsMaxFrequency = 15f;

	// Token: 0x040017AD RID: 6061
	private const float UpdateBoundsMinFrequency = 0.5f;

	// Token: 0x040017AE RID: 6062
	private LODGroup lodGroup;

	// Token: 0x040017AF RID: 6063
	public List<Collider> colliders = new List<Collider>();

	// Token: 0x040017B0 RID: 6064
	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x040017B1 RID: 6065
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x040017B2 RID: 6066
	private bool isVisible = true;

	// Token: 0x040017B3 RID: 6067
	private Action TriggerUpdateVisibilityBoundsDelegate;
}
