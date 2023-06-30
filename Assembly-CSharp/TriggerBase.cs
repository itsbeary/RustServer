using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class TriggerBase : BaseMonoBehaviour
{
	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002B72 RID: 11122 RVA: 0x00108185 File Offset: 0x00106385
	public bool HasAnyContents
	{
		get
		{
			return !this.contents.IsNullOrEmpty<GameObject>();
		}
	}

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002B73 RID: 11123 RVA: 0x00108195 File Offset: 0x00106395
	public bool HasAnyEntityContents
	{
		get
		{
			return !this.entityContents.IsNullOrEmpty<BaseEntity>();
		}
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x001081A8 File Offset: 0x001063A8
	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << obj.layer;
		if ((this.interestLayers.value & num) != num)
		{
			return null;
		}
		return obj;
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x001081D4 File Offset: 0x001063D4
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (this.contents == null)
		{
			return;
		}
		foreach (GameObject gameObject in this.contents.ToArray<GameObject>())
		{
			this.OnTriggerExit(gameObject);
		}
		this.contents = null;
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x0010821E File Offset: 0x0010641E
	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			this.entityContents = new HashSet<BaseEntity>();
		}
		this.entityContents.Add(ent);
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x0010824A File Offset: 0x0010644A
	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (this.entityContents == null)
		{
			return;
		}
		this.entityContents.Remove(ent);
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x00108264 File Offset: 0x00106464
	internal virtual void OnObjectAdded(GameObject obj, Collider col)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.EnterTrigger(this);
			this.OnEntityEnter(baseEntity);
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x0010829C File Offset: 0x0010649C
	internal virtual void OnObjectRemoved(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			bool flag = false;
			foreach (GameObject gameObject in this.contents)
			{
				if (gameObject == null)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains null object.");
				}
				else if (gameObject.ToBaseEntity() == baseEntity)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				baseEntity.LeaveTrigger(this);
				this.OnEntityLeave(baseEntity);
			}
		}
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x0010834C File Offset: 0x0010654C
	internal void RemoveInvalidEntities()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		List<BaseEntity> list = null;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity == null)
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains destroyed entity.");
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
			else if (!bounds.Contains(baseEntity.ClosestPoint(base.transform.position)))
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains entity that is too far away: " + baseEntity.ToString());
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
		}
		if (list != null)
		{
			foreach (BaseEntity baseEntity2 in list)
			{
				this.RemoveEntity(baseEntity2);
			}
			Facepunch.Pool.FreeList<BaseEntity>(ref list);
		}
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x001084B0 File Offset: 0x001066B0
	internal bool CheckEntity(BaseEntity ent)
	{
		if (ent == null)
		{
			return true;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return true;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		return bounds.Contains(ent.ClosestPoint(base.transform.position));
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x000063A5 File Offset: 0x000045A5
	internal virtual void OnObjects()
	{
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x00108505 File Offset: 0x00106705
	internal virtual void OnEmpty()
	{
		this.contents = null;
		this.entityContents = null;
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x00108518 File Offset: 0x00106718
	public void RemoveObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		this.OnTriggerExit(component);
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x00108548 File Offset: 0x00106748
	public void RemoveEntity(BaseEntity ent)
	{
		if (this == null || this.contents == null || ent == null)
		{
			return;
		}
		List<GameObject> list = Facepunch.Pool.GetList<GameObject>();
		foreach (GameObject gameObject in this.contents)
		{
			if (gameObject != null && gameObject.GetComponentInParent<BaseEntity>() == ent)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject gameObject2 in list)
		{
			this.OnTriggerExit(gameObject2);
		}
		Facepunch.Pool.FreeList<GameObject>(ref list);
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x0010861C File Offset: 0x0010681C
	public void OnTriggerEnter(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (!base.enabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerBase.OnTriggerEnter", 0))
		{
			GameObject gameObject = this.InterestedInObject(collider.gameObject);
			if (gameObject == null)
			{
				return;
			}
			if (this.contents == null)
			{
				this.contents = new HashSet<GameObject>();
			}
			if (this.contents.Contains(gameObject))
			{
				return;
			}
			bool count = this.contents.Count != 0;
			this.contents.Add(gameObject);
			this.OnObjectAdded(gameObject, collider);
			if (!count && this.contents.Count == 1)
			{
				this.OnObjects();
			}
		}
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x00007A44 File Offset: 0x00005C44
	internal virtual bool SkipOnTriggerExit(Collider collider)
	{
		return false;
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x001086E4 File Offset: 0x001068E4
	public void OnTriggerExit(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (collider == null)
		{
			return;
		}
		if (this.SkipOnTriggerExit(collider))
		{
			return;
		}
		GameObject gameObject = this.InterestedInObject(collider.gameObject);
		if (gameObject == null)
		{
			return;
		}
		this.OnTriggerExit(gameObject);
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x0010873C File Offset: 0x0010693C
	private void OnTriggerExit(GameObject targetObj)
	{
		if (this.contents == null)
		{
			return;
		}
		if (!this.contents.Contains(targetObj))
		{
			return;
		}
		this.contents.Remove(targetObj);
		this.OnObjectRemoved(targetObj);
		if (this.contents == null || this.contents.Count == 0)
		{
			this.OnEmpty();
		}
	}

	// Token: 0x0400236C RID: 9068
	public LayerMask interestLayers;

	// Token: 0x0400236D RID: 9069
	[NonSerialized]
	public HashSet<GameObject> contents;

	// Token: 0x0400236E RID: 9070
	[NonSerialized]
	public HashSet<BaseEntity> entityContents;
}
