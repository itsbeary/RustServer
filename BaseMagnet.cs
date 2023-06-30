using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class BaseMagnet : MonoBehaviour
{
	// Token: 0x06002695 RID: 9877 RVA: 0x000F303C File Offset: 0x000F123C
	public bool HasConnectedObject()
	{
		return this.fixedJoint.connectedBody != null && this.isMagnetOn;
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000F305C File Offset: 0x000F125C
	public OBB GetConnectedOBB(float scale = 1f)
	{
		if (this.fixedJoint.connectedBody == null)
		{
			Debug.LogError("BaseMagnet returning fake OBB because no connected body!");
			return new OBB(Vector3.zero, Vector3.one, Quaternion.identity);
		}
		BaseEntity component = this.fixedJoint.connectedBody.gameObject.GetComponent<BaseEntity>();
		Bounds bounds = component.bounds;
		bounds.extents *= scale;
		return new OBB(component.transform.position, component.transform.rotation, bounds);
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000F30E8 File Offset: 0x000F12E8
	public void SetCollisionsEnabled(GameObject other, bool wants)
	{
		Collider[] componentsInChildren = other.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = this.colliderSource.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			foreach (Collider collider2 in componentsInChildren2)
			{
				Physics.IgnoreCollision(collider, collider2, !wants);
			}
		}
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000F3144 File Offset: 0x000F1344
	public virtual void SetMagnetEnabled(bool wantsOn, BasePlayer forPlayer)
	{
		if (this.isMagnetOn == wantsOn)
		{
			return;
		}
		this.associatedPlayer = forPlayer;
		this.isMagnetOn = wantsOn;
		if (this.isMagnetOn)
		{
			this.OnMagnetEnabled();
		}
		else
		{
			this.OnMagnetDisabled();
		}
		if (this.entityOwner != null)
		{
			this.entityOwner.SetFlag(this.magnetFlag, this.isMagnetOn, false, true);
		}
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnMagnetEnabled()
	{
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000F31A8 File Offset: 0x000F13A8
	public virtual void OnMagnetDisabled()
	{
		if (this.fixedJoint.connectedBody)
		{
			this.SetCollisionsEnabled(this.fixedJoint.connectedBody.gameObject, true);
			Rigidbody connectedBody = this.fixedJoint.connectedBody;
			this.fixedJoint.connectedBody = null;
			connectedBody.WakeUp();
		}
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000F31FA File Offset: 0x000F13FA
	public bool IsMagnetOn()
	{
		return this.isMagnetOn;
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000F3204 File Offset: 0x000F1404
	public void MagnetThink(float delta)
	{
		if (!this.isMagnetOn)
		{
			return;
		}
		Vector3 position = this.magnetTrigger.transform.position;
		if (this.magnetTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.magnetTrigger.entityContents)
			{
				if (baseEntity.syncPosition)
				{
					Rigidbody component = baseEntity.GetComponent<Rigidbody>();
					if (!(component == null) && !component.isKinematic && !baseEntity.isClient)
					{
						OBB obb = new OBB(baseEntity.transform.position, baseEntity.transform.rotation, baseEntity.bounds);
						if (obb.Contains(this.attachDepthPoint.position))
						{
							baseEntity.GetComponent<MagnetLiftable>().SetMagnetized(true, this, this.associatedPlayer);
							if (this.fixedJoint.connectedBody == null)
							{
								Effect.server.Run(this.attachEffect.resourcePath, this.attachDepthPoint.position, -this.attachDepthPoint.up, null, false);
								this.fixedJoint.connectedBody = component;
								this.SetCollisionsEnabled(component.gameObject, false);
								continue;
							}
						}
						if (this.fixedJoint.connectedBody == null)
						{
							Vector3 position2 = baseEntity.transform.position;
							float num = Vector3.Distance(position2, position);
							Vector3 vector = Vector3Ex.Direction(position, position2);
							float num2 = 1f / Mathf.Max(1f, num);
							component.AddForce(vector * this.magnetForce * num2, ForceMode.Acceleration);
						}
					}
				}
			}
		}
	}

	// Token: 0x04001F00 RID: 7936
	public BaseEntity entityOwner;

	// Token: 0x04001F01 RID: 7937
	public BaseEntity.Flags magnetFlag = BaseEntity.Flags.Reserved6;

	// Token: 0x04001F02 RID: 7938
	public TriggerMagnet magnetTrigger;

	// Token: 0x04001F03 RID: 7939
	public FixedJoint fixedJoint;

	// Token: 0x04001F04 RID: 7940
	public Rigidbody kinematicAttachmentBody;

	// Token: 0x04001F05 RID: 7941
	public float magnetForce;

	// Token: 0x04001F06 RID: 7942
	public Transform attachDepthPoint;

	// Token: 0x04001F07 RID: 7943
	public GameObjectRef attachEffect;

	// Token: 0x04001F08 RID: 7944
	public bool isMagnetOn;

	// Token: 0x04001F09 RID: 7945
	public GameObject colliderSource;

	// Token: 0x04001F0A RID: 7946
	private BasePlayer associatedPlayer;
}
