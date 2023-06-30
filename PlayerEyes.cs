using System;
using UnityEngine;

// Token: 0x0200044A RID: 1098
public class PlayerEyes : EntityComponent<BasePlayer>
{
	// Token: 0x17000301 RID: 769
	// (get) Token: 0x060024CF RID: 9423 RVA: 0x000E9D98 File Offset: 0x000E7F98
	public Vector3 worldMountedPosition
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return this.worldStandingPosition;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x060024D0 RID: 9424 RVA: 0x000E9DF1 File Offset: 0x000E7FF1
	public Vector3 worldStandingPosition
	{
		get
		{
			return base.transform.position + PlayerEyes.EyeOffset;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x060024D1 RID: 9425 RVA: 0x000E9E08 File Offset: 0x000E8008
	public Vector3 worldCrouchedPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.DuckOffset;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x060024D2 RID: 9426 RVA: 0x000E9E1A File Offset: 0x000E801A
	public Vector3 worldCrawlingPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.CrawlOffset;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x060024D3 RID: 9427 RVA: 0x000E9E2C File Offset: 0x000E802C
	public Vector3 position
	{
		get
		{
			if (!base.baseEntity || !base.baseEntity.isMounted)
			{
				return base.transform.position + base.transform.rotation * (PlayerEyes.EyeOffset + this.viewOffset) + this.BodyLeanOffset;
			}
			Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
			if (vector != Vector3.zero)
			{
				return vector;
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y) + this.BodyLeanOffset;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x060024D4 RID: 9428 RVA: 0x0002C05D File Offset: 0x0002A25D
	private Vector3 BodyLeanOffset
	{
		get
		{
			return Vector3.zero;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x060024D5 RID: 9429 RVA: 0x000E9EFC File Offset: 0x000E80FC
	public Vector3 center
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyeCenterForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y);
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x060024D6 RID: 9430 RVA: 0x000E9F84 File Offset: 0x000E8184
	public Vector3 offset
	{
		get
		{
			return base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x060024D7 RID: 9431 RVA: 0x000E9FAC File Offset: 0x000E81AC
	// (set) Token: 0x060024D8 RID: 9432 RVA: 0x000E9FBF File Offset: 0x000E81BF
	public Quaternion rotation
	{
		get
		{
			return this.parentRotation * this.bodyRotation;
		}
		set
		{
			this.bodyRotation = Quaternion.Inverse(this.parentRotation) * value;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x060024D9 RID: 9433 RVA: 0x000E9FD8 File Offset: 0x000E81D8
	// (set) Token: 0x060024DA RID: 9434 RVA: 0x000E9FE0 File Offset: 0x000E81E0
	public Quaternion bodyRotation { get; set; }

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x060024DB RID: 9435 RVA: 0x000E9FEC File Offset: 0x000E81EC
	public Quaternion parentRotation
	{
		get
		{
			if (base.baseEntity.isMounted || !(base.transform.parent != null))
			{
				return Quaternion.identity;
			}
			return Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000EA04C File Offset: 0x000E824C
	public void NetworkUpdate(Quaternion rot)
	{
		if (base.baseEntity.IsCrawling())
		{
			this.viewOffset = PlayerEyes.CrawlOffset;
		}
		else if (base.baseEntity.IsDucked())
		{
			this.viewOffset = PlayerEyes.DuckOffset;
		}
		else
		{
			this.viewOffset = Vector3.zero;
		}
		this.bodyRotation = rot;
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000EA0A0 File Offset: 0x000E82A0
	public Vector3 MovementForward()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.forward;
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000EA0E0 File Offset: 0x000E82E0
	public Vector3 MovementRight()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.right;
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000EA11E File Offset: 0x000E831E
	public Ray BodyRay()
	{
		return new Ray(this.position, this.BodyForward());
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000EA131 File Offset: 0x000E8331
	public Vector3 BodyForward()
	{
		return this.rotation * Vector3.forward;
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000EA143 File Offset: 0x000E8343
	public Vector3 BodyRight()
	{
		return this.rotation * Vector3.right;
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000EA155 File Offset: 0x000E8355
	public Vector3 BodyUp()
	{
		return this.rotation * Vector3.up;
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000EA167 File Offset: 0x000E8367
	public Ray HeadRay()
	{
		return new Ray(this.position, this.HeadForward());
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x000EA17A File Offset: 0x000E837A
	public Vector3 HeadForward()
	{
		return this.GetLookRotation() * Vector3.forward;
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000EA18C File Offset: 0x000E838C
	public Vector3 HeadRight()
	{
		return this.GetLookRotation() * Vector3.right;
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000EA19E File Offset: 0x000E839E
	public Vector3 HeadUp()
	{
		return this.GetLookRotation() * Vector3.up;
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000EA1B0 File Offset: 0x000E83B0
	public Quaternion GetLookRotation()
	{
		return this.rotation;
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x000EA1B0 File Offset: 0x000E83B0
	public Quaternion GetAimRotation()
	{
		return this.rotation;
	}

	// Token: 0x04001CD1 RID: 7377
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	// Token: 0x04001CD2 RID: 7378
	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	// Token: 0x04001CD3 RID: 7379
	public static readonly Vector3 CrawlOffset = new Vector3(0f, -1.15f, 0.175f);

	// Token: 0x04001CD4 RID: 7380
	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	// Token: 0x04001CD5 RID: 7381
	public LazyAimProperties defaultLazyAim;

	// Token: 0x04001CD6 RID: 7382
	private Vector3 viewOffset = Vector3.zero;
}
