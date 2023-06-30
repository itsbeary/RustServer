using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

// Token: 0x0200055F RID: 1375
public class PrefabPreProcess : IPrefabProcessor
{
	// Token: 0x06002A77 RID: 10871 RVA: 0x00102A58 File Offset: 0x00100C58
	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		this.isClientside = clientside;
		this.isServerside = serverside;
		this.isBundling = bundling;
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x00102AA8 File Offset: 0x00100CA8
	public GameObject Find(string strPrefab)
	{
		GameObject gameObject;
		if (!this.prefabList.TryGetValue(strPrefab, out gameObject))
		{
			return null;
		}
		if (gameObject == null)
		{
			this.prefabList.Remove(strPrefab);
			return null;
		}
		return gameObject;
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x00102AE0 File Offset: 0x00100CE0
	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (this.HasComponents<IPrefabPreProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IPrefabPostProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IEditorComponent>(go.transform))
		{
			return true;
		}
		if (!this.isClientside)
		{
			if (PrefabPreProcess.clientsideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IClientComponentEx>(go.transform))
			{
				return true;
			}
		}
		if (!this.isServerside)
		{
			if (PrefabPreProcess.serversideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IServerComponentEx>(go.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x00102BC4 File Offset: 0x00100DC4
	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		if (!this.isClientside)
		{
			foreach (Type type in PrefabPreProcess.clientsideOnlyTypes)
			{
				this.DestroyComponents(type, go, this.isClientside, this.isServerside);
			}
			foreach (IClientComponentEx clientComponentEx in this.FindComponents<IClientComponentEx>(go.transform))
			{
				clientComponentEx.PreClientComponentCull(this);
			}
		}
		if (!this.isServerside)
		{
			foreach (Type type2 in PrefabPreProcess.serversideOnlyTypes)
			{
				this.DestroyComponents(type2, go, this.isClientside, this.isServerside);
			}
			foreach (IServerComponentEx serverComponentEx in this.FindComponents<IServerComponentEx>(go.transform))
			{
				serverComponentEx.PreServerComponentCull(this);
			}
		}
		this.DestroyComponents(typeof(IEditorComponent), go, this.isClientside, this.isServerside);
		if (resetLocalTransform)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
		List<Transform> list = this.FindComponents<Transform>(go.transform);
		list.Reverse();
		MeshColliderCookingOptions meshColliderCookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices;
		MeshColliderCookingOptions meshColliderCookingOptions2 = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;
		MeshColliderCookingOptions meshColliderCookingOptions3 = (MeshColliderCookingOptions)(-1);
		foreach (MeshCollider meshCollider in this.FindComponents<MeshCollider>(go.transform))
		{
			if (meshCollider.cookingOptions == meshColliderCookingOptions || meshCollider.cookingOptions == meshColliderCookingOptions3)
			{
				meshCollider.cookingOptions = meshColliderCookingOptions2;
			}
		}
		foreach (IPrefabPreProcess prefabPreProcess in this.FindComponents<IPrefabPreProcess>(go.transform))
		{
			prefabPreProcess.PreProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
		foreach (Transform transform in list)
		{
			if (transform && transform.gameObject)
			{
				if (this.isServerside && transform.gameObject.CompareTag("Server Cull"))
				{
					this.RemoveComponents(transform.gameObject);
					this.NominateForDeletion(transform.gameObject);
				}
				if (this.isClientside)
				{
					bool flag = transform.gameObject.CompareTag("Client Cull");
					bool flag2 = transform != go.transform && transform.gameObject.GetComponent<BaseEntity>() != null;
					if (flag || flag2)
					{
						this.RemoveComponents(transform.gameObject);
						this.NominateForDeletion(transform.gameObject);
					}
				}
			}
		}
		this.RunCleanupQueue();
		foreach (IPrefabPostProcess prefabPostProcess in this.FindComponents<IPrefabPostProcess>(go.transform))
		{
			prefabPostProcess.PostProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x00102F44 File Offset: 0x00101144
	public void Process(string name, GameObject go)
	{
		if (!UnityEngine.Application.isPlaying)
		{
			return;
		}
		if (go.CompareTag("NoPreProcessing"))
		{
			return;
		}
		GameObject hierarchyGroup = this.GetHierarchyGroup();
		GameObject gameObject = go;
		go = Instantiate.GameObject(gameObject, hierarchyGroup.transform);
		go.name = gameObject.name;
		if (this.NeedsProcessing(go))
		{
			this.ProcessObject(name, go, true);
		}
		this.AddPrefab(name, go);
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x00102FA4 File Offset: 0x001011A4
	public void Invalidate(string name)
	{
		GameObject gameObject;
		if (this.prefabList.TryGetValue(name, out gameObject))
		{
			this.prefabList.Remove(name);
			if (gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject, true);
			}
		}
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x00102FDE File Offset: 0x001011DE
	public GameObject GetHierarchyGroup()
	{
		if (this.isClientside && this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
		}
		if (this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x0010301E File Offset: 0x0010121E
	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		this.prefabList.Add(name, go);
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x00103034 File Offset: 0x00101234
	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(go.transform, list, t);
		list.Reverse();
		foreach (Component component in list)
		{
			RealmedRemove component2 = component.GetComponent<RealmedRemove>();
			if (!(component2 != null) || component2.ShouldDelete(component, client, server))
			{
				if (!component.gameObject.CompareTag("persist"))
				{
					this.NominateForDeletion(component.gameObject);
				}
				UnityEngine.Object.DestroyImmediate(component, true);
			}
		}
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x001030D8 File Offset: 0x001012D8
	private bool ShouldExclude(Transform transform)
	{
		return transform.GetComponent<BaseEntity>() != null;
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x001030EC File Offset: 0x001012EC
	private bool HasComponents<T>(Transform transform)
	{
		if (transform.GetComponent<T>() != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents<T>(transform2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x00103164 File Offset: 0x00101364
	private bool HasComponents(Transform transform, Type t)
	{
		if (transform.GetComponent(t) != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents(transform2, t))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x001031DC File Offset: 0x001013DC
	public List<T> FindComponents<T>(Transform transform)
	{
		List<T> list = new List<T>();
		this.FindComponents<T>(transform, list);
		return list;
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x001031F8 File Offset: 0x001013F8
	public void FindComponents<T>(Transform transform, List<T> list)
	{
		list.AddRange(transform.GetComponents<T>());
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents<T>(transform2, list);
			}
		}
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x00103264 File Offset: 0x00101464
	public List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(transform, list, t);
		return list;
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x00103284 File Offset: 0x00101484
	public void FindComponents(Transform transform, List<Component> list, Type t)
	{
		list.AddRange(transform.GetComponents(t));
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents(transform2, list, t);
			}
		}
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x001032F0 File Offset: 0x001014F0
	public void RemoveComponent(Component c)
	{
		if (c == null)
		{
			return;
		}
		this.destroyList.Add(c);
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x00103308 File Offset: 0x00101508
	public void RemoveComponents(GameObject gameObj)
	{
		foreach (Component component in gameObj.GetComponents<Component>())
		{
			if (!(component is Transform))
			{
				this.destroyList.Add(component);
			}
		}
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x00103342 File Offset: 0x00101542
	public void NominateForDeletion(GameObject gameObj)
	{
		this.cleanupList.Add(gameObj);
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x00103350 File Offset: 0x00101550
	private void RunCleanupQueue()
	{
		foreach (Component component in this.destroyList)
		{
			UnityEngine.Object.DestroyImmediate(component, true);
		}
		this.destroyList.Clear();
		foreach (GameObject gameObject in this.cleanupList)
		{
			this.DoCleanup(gameObject);
		}
		this.cleanupList.Clear();
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x001033FC File Offset: 0x001015FC
	private void DoCleanup(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go.GetComponentsInChildren<Component>(true).Length > 1)
		{
			return;
		}
		Transform parent = go.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.name.StartsWith("PrefabPreProcess - "))
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(go, true);
	}

	// Token: 0x040022C2 RID: 8898
	public static Type[] clientsideOnlyTypes = new Type[]
	{
		typeof(IClientComponent),
		typeof(SkeletonSkinLod),
		typeof(ImageEffectLayer),
		typeof(NGSS_Directional),
		typeof(VolumetricDustParticles),
		typeof(VolumetricLightBeam),
		typeof(Cloth),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(AudioLowPassFilter),
		typeof(AudioSource),
		typeof(AudioListener),
		typeof(ParticleSystemRenderer),
		typeof(ParticleSystem),
		typeof(ParticleEmitFromParentObject),
		typeof(ImpostorShadows),
		typeof(Light),
		typeof(LODGroup),
		typeof(Animator),
		typeof(AnimationEvents),
		typeof(PlayerVoiceSpeaker),
		typeof(VoiceProcessor),
		typeof(PlayerVoiceRecorder),
		typeof(ParticleScaler),
		typeof(PostEffectsBase),
		typeof(TOD_ImageEffect),
		typeof(TOD_Scattering),
		typeof(TOD_Rays),
		typeof(Tree),
		typeof(Projector),
		typeof(HttpImage),
		typeof(EventTrigger),
		typeof(StandaloneInputModule),
		typeof(UIBehaviour),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasGroup),
		typeof(GraphicRaycaster)
	};

	// Token: 0x040022C3 RID: 8899
	public static Type[] serversideOnlyTypes = new Type[]
	{
		typeof(IServerComponent),
		typeof(NavMeshObstacle)
	};

	// Token: 0x040022C4 RID: 8900
	public bool isClientside;

	// Token: 0x040022C5 RID: 8901
	public bool isServerside;

	// Token: 0x040022C6 RID: 8902
	public bool isBundling;

	// Token: 0x040022C7 RID: 8903
	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x040022C8 RID: 8904
	private List<Component> destroyList = new List<Component>();

	// Token: 0x040022C9 RID: 8905
	private List<GameObject> cleanupList = new List<GameObject>();
}
