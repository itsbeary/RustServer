using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public class TriggerTrainCollisions : TriggerBase
{
	// Token: 0x1700035A RID: 858
	// (get) Token: 0x060027CF RID: 10191 RVA: 0x000F8945 File Offset: 0x000F6B45
	public bool HasAnyStaticContents
	{
		get
		{
			return this.staticContents.Count > 0;
		}
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x060027D0 RID: 10192 RVA: 0x000F8955 File Offset: 0x000F6B55
	public bool HasAnyTrainContents
	{
		get
		{
			return this.trainContents.Count > 0;
		}
	}

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x060027D1 RID: 10193 RVA: 0x000F8965 File Offset: 0x000F6B65
	public bool HasAnyOtherRigidbodyContents
	{
		get
		{
			return this.otherRigidbodyContents.Count > 0;
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x060027D2 RID: 10194 RVA: 0x000F8975 File Offset: 0x000F6B75
	public bool HasAnyNonStaticContents
	{
		get
		{
			return this.HasAnyTrainContents || this.HasAnyOtherRigidbodyContents;
		}
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000F8988 File Offset: 0x000F6B88
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		base.OnObjectAdded(obj, col);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			if (baseEntity != null)
			{
				Vector3 vector = baseEntity.transform.position + baseEntity.transform.rotation * Vector3.Scale(obj.transform.lossyScale, baseEntity.bounds.center);
				Vector3 center = this.triggerCollider.bounds.center;
				Vector3 vector2 = vector - center;
				bool flag = Vector3.Dot(this.owner.transform.forward, vector2) > 0f;
				if (this.location == TriggerTrainCollisions.Location.Front && !flag)
				{
					return;
				}
				if (this.location == TriggerTrainCollisions.Location.Rear && flag)
				{
					return;
				}
			}
		}
		if (obj != null)
		{
			Rigidbody componentInParent = obj.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				TrainCar componentInParent2 = obj.GetComponentInParent<TrainCar>();
				if (componentInParent2 != null)
				{
					this.trainContents.Add(componentInParent2);
					if (this.owner.coupling != null)
					{
						this.owner.coupling.TryCouple(componentInParent2, this.location);
					}
					base.InvokeRepeating(new Action(this.TrainContentsTick), 0.2f, 0.2f);
				}
				else
				{
					this.otherRigidbodyContents.Add(componentInParent);
				}
			}
			else
			{
				ITrainCollidable componentInParent3 = obj.GetComponentInParent<ITrainCollidable>();
				if (componentInParent3 == null)
				{
					if (!obj.CompareTag("Railway"))
					{
						this.staticContents.Add(obj);
					}
				}
				else if (!componentInParent3.EqualNetID(this.owner) && !componentInParent3.CustomCollision(this.owner, this))
				{
					this.staticContents.Add(obj);
				}
			}
		}
		if (col != null)
		{
			this.colliderContents.Add(col);
		}
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000F8B58 File Offset: 0x000F6D58
	internal override void OnObjectRemoved(GameObject obj)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		if (obj == null)
		{
			return;
		}
		foreach (Collider collider in obj.GetComponents<Collider>())
		{
			this.colliderContents.Remove(collider);
		}
		if (!this.staticContents.Remove(obj))
		{
			TrainCar componentInParent = obj.GetComponentInParent<TrainCar>();
			if (componentInParent != null)
			{
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<TrainCar>(componentInParent))
				{
					this.trainContents.Remove(componentInParent);
					if (this.trainContents == null || this.trainContents.Count == 0)
					{
						base.CancelInvoke(new Action(this.TrainContentsTick));
					}
				}
			}
			else
			{
				Rigidbody componentInParent2 = obj.GetComponentInParent<Rigidbody>();
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<Rigidbody>(componentInParent2))
				{
					this.otherRigidbodyContents.Remove(componentInParent2);
				}
			}
		}
		base.OnObjectRemoved(obj);
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000F8C28 File Offset: 0x000F6E28
	private void TrainContentsTick()
	{
		if (this.trainContents == null)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainContents)
		{
			if (trainCar.IsValid() && !trainCar.IsDestroyed && this.owner.coupling != null)
			{
				this.owner.coupling.TryCouple(trainCar, this.location);
			}
		}
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000F8CE8 File Offset: 0x000F6EE8
	[CompilerGenerated]
	private bool <OnObjectRemoved>g__HasAnotherColliderFor|18_0<T>(T component) where T : Component
	{
		foreach (Collider collider in this.colliderContents)
		{
			if (collider != null && collider.GetComponentInParent<T>() == component)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400206F RID: 8303
	public Collider triggerCollider;

	// Token: 0x04002070 RID: 8304
	public TriggerTrainCollisions.Location location;

	// Token: 0x04002071 RID: 8305
	public TrainCar owner;

	// Token: 0x04002072 RID: 8306
	[NonSerialized]
	public HashSet<GameObject> staticContents = new HashSet<GameObject>();

	// Token: 0x04002073 RID: 8307
	[NonSerialized]
	public HashSet<TrainCar> trainContents = new HashSet<TrainCar>();

	// Token: 0x04002074 RID: 8308
	[NonSerialized]
	public HashSet<Rigidbody> otherRigidbodyContents = new HashSet<Rigidbody>();

	// Token: 0x04002075 RID: 8309
	[NonSerialized]
	public HashSet<Collider> colliderContents = new HashSet<Collider>();

	// Token: 0x04002076 RID: 8310
	private const float TICK_RATE = 0.2f;

	// Token: 0x02000D31 RID: 3377
	public enum Location
	{
		// Token: 0x0400470D RID: 18189
		Front,
		// Token: 0x0400470E RID: 18190
		Rear
	}
}
