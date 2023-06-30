using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000575 RID: 1397
public class GenericSpawnPoint : BaseSpawnPoint
{
	// Token: 0x06002AEB RID: 10987 RVA: 0x001059A8 File Offset: 0x00103BA8
	public Quaternion GetRandomRotation()
	{
		if (!this.randomRot)
		{
			return Quaternion.identity;
		}
		int num = Mathf.FloorToInt(360f / this.randomRotSnapDegrees);
		int num2 = UnityEngine.Random.Range(0, num);
		return Quaternion.Euler(0f, (float)num2 * this.randomRotSnapDegrees, 0f);
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x001059F8 File Offset: 0x00103BF8
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position;
		if (this.randomRot)
		{
			rot = base.transform.rotation * this.GetRandomRotation();
		}
		else
		{
			rot = base.transform.rotation;
		}
		if (this.dropToGround)
		{
			base.DropToGround(ref pos, ref rot);
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x00105A60 File Offset: 0x00103C60
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		if (this.spawnEffect.isValid)
		{
			Effect.server.Run(this.spawnEffect.resourcePath, instance.GetComponent<BaseEntity>(), 0U, Vector3.zero, Vector3.up, null, false);
		}
		this.OnObjectSpawnedEvent.Invoke();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x00105AB4 File Offset: 0x00103CB4
	public override void ObjectRetired(SpawnPointInstance instance)
	{
		this.OnObjectRetiredEvent.Invoke();
		base.gameObject.SetActive(true);
	}

	// Token: 0x04002311 RID: 8977
	public bool dropToGround = true;

	// Token: 0x04002312 RID: 8978
	public bool randomRot;

	// Token: 0x04002313 RID: 8979
	[Range(1f, 180f)]
	public float randomRotSnapDegrees = 1f;

	// Token: 0x04002314 RID: 8980
	public GameObjectRef spawnEffect;

	// Token: 0x04002315 RID: 8981
	public UnityEvent OnObjectSpawnedEvent = new UnityEvent();

	// Token: 0x04002316 RID: 8982
	public UnityEvent OnObjectRetiredEvent = new UnityEvent();
}
