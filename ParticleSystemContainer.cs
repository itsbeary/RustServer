using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000910 RID: 2320
public class ParticleSystemContainer : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06003800 RID: 14336 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Play()
	{
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Pause()
	{
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Stop()
	{
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Clear()
	{
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x0014DF64 File Offset: 0x0014C164
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.precached && clientside)
		{
			List<ParticleSystemContainer.ParticleSystemGroup> list = new List<ParticleSystemContainer.ParticleSystemGroup>();
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				LODComponentParticleSystem[] components = particleSystem.GetComponents<LODComponentParticleSystem>();
				ParticleSystemContainer.ParticleSystemGroup particleSystemGroup = new ParticleSystemContainer.ParticleSystemGroup
				{
					system = particleSystem,
					lodComponents = components
				};
				list.Add(particleSystemGroup);
			}
			this.particleGroups = list.ToArray();
		}
	}

	// Token: 0x04003362 RID: 13154
	public bool precached;

	// Token: 0x04003363 RID: 13155
	[HideInInspector]
	public ParticleSystemContainer.ParticleSystemGroup[] particleGroups;

	// Token: 0x02000EC7 RID: 3783
	[Serializable]
	public struct ParticleSystemGroup
	{
		// Token: 0x04004D55 RID: 19797
		public ParticleSystem system;

		// Token: 0x04004D56 RID: 19798
		public LODComponentParticleSystem[] lodComponents;
	}
}
