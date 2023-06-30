using System;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x02000963 RID: 2403
public class Poolable : MonoBehaviour, IClientComponent, IPrefabPostProcess
{
	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06003999 RID: 14745 RVA: 0x0015551C File Offset: 0x0015371C
	public int ClientCount
	{
		get
		{
			if (base.GetComponent<LootPanel>() != null)
			{
				return 1;
			}
			if (base.GetComponent<DecorComponent>() != null)
			{
				return 100;
			}
			if (base.GetComponent<BuildingBlock>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Door>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Projectile>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Gib>() != null)
			{
				return 100;
			}
			return 1;
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x0600399A RID: 14746 RVA: 0x00007A44 File Offset: 0x00005C44
	public int ServerCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x0015558F File Offset: 0x0015378F
	public void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.Initialize(StringPool.Get(name));
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x001555A4 File Offset: 0x001537A4
	public void Initialize(uint id)
	{
		this.prefabID = id;
		this.behaviours = base.gameObject.GetComponentsInChildren(typeof(Behaviour), true).OfType<Behaviour>().ToArray<Behaviour>();
		this.rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>(true);
		this.colliders = base.gameObject.GetComponentsInChildren<Collider>(true);
		this.lodgroups = base.gameObject.GetComponentsInChildren<LODGroup>(true);
		this.renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
		this.particles = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		if (this.behaviours.Length == 0)
		{
			this.behaviours = Array.Empty<Behaviour>();
		}
		if (this.rigidbodies.Length == 0)
		{
			this.rigidbodies = Array.Empty<Rigidbody>();
		}
		if (this.colliders.Length == 0)
		{
			this.colliders = Array.Empty<Collider>();
		}
		if (this.lodgroups.Length == 0)
		{
			this.lodgroups = Array.Empty<LODGroup>();
		}
		if (this.renderers.Length == 0)
		{
			this.renderers = Array.Empty<Renderer>();
		}
		if (this.particles.Length == 0)
		{
			this.particles = Array.Empty<ParticleSystem>();
		}
		this.behaviourStates = ArrayEx.New<bool>(this.behaviours.Length);
		this.rigidbodyStates = ArrayEx.New<bool>(this.rigidbodies.Length);
		this.colliderStates = ArrayEx.New<bool>(this.colliders.Length);
		this.lodgroupStates = ArrayEx.New<bool>(this.lodgroups.Length);
		this.rendererStates = ArrayEx.New<bool>(this.renderers.Length);
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x00155710 File Offset: 0x00153910
	public void EnterPool()
	{
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, false);
		}
		if (Pool.mode <= 1)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.SetBehaviourEnabled(false);
			this.SetComponentEnabled(false);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x00155786 File Offset: 0x00153986
	public void LeavePool()
	{
		if (Pool.mode > 1)
		{
			this.SetComponentEnabled(true);
		}
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x00155798 File Offset: 0x00153998
	public void SetBehaviourEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.behaviours.Length; i++)
				{
					Behaviour behaviour = this.behaviours[i];
					this.behaviourStates[i] = behaviour.enabled;
					behaviour.enabled = false;
				}
				for (int j = 0; j < this.particles.Length; j++)
				{
					ParticleSystem particleSystem = this.particles[j];
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
			else
			{
				for (int k = 0; k < this.particles.Length; k++)
				{
					ParticleSystem particleSystem2 = this.particles[k];
					if (particleSystem2.playOnAwake)
					{
						particleSystem2.Play();
					}
				}
				for (int l = 0; l < this.behaviours.Length; l++)
				{
					this.behaviours[l].enabled = this.behaviourStates[l];
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[] { "Pooling error: ", base.name, " (", ex.Message, ")" }));
		}
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x001558B0 File Offset: 0x00153AB0
	public void SetComponentEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Renderer renderer = this.renderers[i];
					this.rendererStates[i] = renderer.enabled;
					renderer.enabled = false;
				}
				for (int j = 0; j < this.lodgroups.Length; j++)
				{
					LODGroup lodgroup = this.lodgroups[j];
					this.lodgroupStates[j] = lodgroup.enabled;
					lodgroup.enabled = false;
				}
				for (int k = 0; k < this.colliders.Length; k++)
				{
					Collider collider = this.colliders[k];
					this.colliderStates[k] = collider.enabled;
					collider.enabled = false;
				}
				for (int l = 0; l < this.rigidbodies.Length; l++)
				{
					Rigidbody rigidbody = this.rigidbodies[l];
					this.rigidbodyStates[l] = rigidbody.isKinematic;
					rigidbody.isKinematic = true;
					rigidbody.detectCollisions = false;
				}
			}
			else
			{
				for (int m = 0; m < this.renderers.Length; m++)
				{
					this.renderers[m].enabled = this.rendererStates[m];
				}
				for (int n = 0; n < this.lodgroups.Length; n++)
				{
					this.lodgroups[n].enabled = this.lodgroupStates[n];
				}
				for (int num = 0; num < this.colliders.Length; num++)
				{
					this.colliders[num].enabled = this.colliderStates[num];
				}
				for (int num2 = 0; num2 < this.rigidbodies.Length; num2++)
				{
					Rigidbody rigidbody2 = this.rigidbodies[num2];
					rigidbody2.isKinematic = this.rigidbodyStates[num2];
					rigidbody2.detectCollisions = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[] { "Pooling error: ", base.name, " (", ex.Message, ")" }));
		}
	}

	// Token: 0x04003418 RID: 13336
	[HideInInspector]
	public uint prefabID;

	// Token: 0x04003419 RID: 13337
	[HideInInspector]
	public Behaviour[] behaviours;

	// Token: 0x0400341A RID: 13338
	[HideInInspector]
	public Rigidbody[] rigidbodies;

	// Token: 0x0400341B RID: 13339
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x0400341C RID: 13340
	[HideInInspector]
	public LODGroup[] lodgroups;

	// Token: 0x0400341D RID: 13341
	[HideInInspector]
	public Renderer[] renderers;

	// Token: 0x0400341E RID: 13342
	[HideInInspector]
	public ParticleSystem[] particles;

	// Token: 0x0400341F RID: 13343
	[HideInInspector]
	public bool[] behaviourStates;

	// Token: 0x04003420 RID: 13344
	[HideInInspector]
	public bool[] rigidbodyStates;

	// Token: 0x04003421 RID: 13345
	[HideInInspector]
	public bool[] colliderStates;

	// Token: 0x04003422 RID: 13346
	[HideInInspector]
	public bool[] lodgroupStates;

	// Token: 0x04003423 RID: 13347
	[HideInInspector]
	public bool[] rendererStates;
}
