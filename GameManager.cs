using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000514 RID: 1300
public class GameManager
{
	// Token: 0x0600299C RID: 10652 RVA: 0x000FF4C9 File Offset: 0x000FD6C9
	public void Reset()
	{
		this.pool.Clear(null);
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x000FF4D7 File Offset: 0x000FD6D7
	public GameManager(bool clientside, bool serverside)
	{
		this.Clientside = clientside;
		this.Serverside = serverside;
		this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
		this.pool = new PrefabPoolCollection();
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x000FF508 File Offset: 0x000FD708
	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return this.FindPrefab(text);
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000FF52D File Offset: 0x000FD72D
	public GameObject FindPrefab(BaseEntity ent)
	{
		if (ent == null)
		{
			return null;
		}
		return this.FindPrefab(ent.PrefabName);
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000FF548 File Offset: 0x000FD748
	public GameObject FindPrefab(string strPrefab)
	{
		GameObject gameObject = this.preProcessed.Find(strPrefab);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = FileSystem.LoadPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		this.preProcessed.Process(strPrefab, gameObject);
		GameObject gameObject2 = this.preProcessed.Find(strPrefab);
		if (!(gameObject2 != null))
		{
			return gameObject;
		}
		return gameObject2;
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x000FF5A8 File Offset: 0x000FD7A8
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject)
		{
			gameObject.transform.localScale = scale;
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x000FF5E0 File Offset: 0x000FD7E0
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000FF60C File Offset: 0x000FD80C
	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000FF640 File Offset: 0x000FD840
	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, parent.position, parent.rotation);
		if (gameObject)
		{
			gameObject.transform.SetParent(parent, false);
			gameObject.Identity();
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000FF688 File Offset: 0x000FD888
	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject gameObject = this.CreatePrefab(strPrefab, pos, rot, startActive);
		if (gameObject == null)
		{
			return null;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (component == null)
		{
			Debug.LogError("CreateEntity called on a prefab that isn't an entity! " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		if (component.CompareTag("CannotBeCreated"))
		{
			Debug.LogWarning("CreateEntity called on a prefab that has the CannotBeCreated tag set. " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		return component;
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000FF704 File Offset: 0x000FD904
	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		if (!strPrefab.IsLower())
		{
			Debug.LogWarning("Converting prefab name to lowercase: " + strPrefab);
			strPrefab = strPrefab.ToLower();
		}
		GameObject gameObject = this.FindPrefab(strPrefab);
		if (!gameObject)
		{
			Debug.LogError("Couldn't find prefab \"" + strPrefab + "\"");
			return null;
		}
		GameObject gameObject2 = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if (gameObject2 == null)
		{
			gameObject2 = Facepunch.Instantiate.GameObject(gameObject, pos, rot);
			gameObject2.name = strPrefab;
		}
		else
		{
			gameObject2.transform.localScale = gameObject.transform.localScale;
		}
		if (!this.Clientside && this.Serverside && gameObject2.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject2, Rust.Server.EntityScene);
		}
		return gameObject2;
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000FF7CC File Offset: 0x000FD9CC
	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.Destroy(component, delay);
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000FF7F7 File Offset: 0x000FD9F7
	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (!instance)
		{
			return;
		}
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.Destroy(instance, delay);
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000FF82B File Offset: 0x000FDA2B
	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.DestroyImmediate(component, allowDestroyingAssets);
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000FF856 File Offset: 0x000FDA56
	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.DestroyImmediate(instance, allowDestroyingAssets);
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000FF884 File Offset: 0x000FDA84
	public void Retire(GameObject instance)
	{
		if (!instance)
		{
			return;
		}
		using (TimeWarning.New("GameManager.Retire", 0))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError("Trying to retire an entity without killing it first: " + instance.name);
			}
			if (!Rust.Application.isQuitting && ConVar.Pool.enabled && instance.SupportsPooling())
			{
				this.pool.Push(instance);
			}
			else
			{
				UnityEngine.Object.Destroy(instance);
			}
		}
	}

	// Token: 0x040021BF RID: 8639
	public static GameManager server = new GameManager(false, true);

	// Token: 0x040021C0 RID: 8640
	internal PrefabPreProcess preProcessed;

	// Token: 0x040021C1 RID: 8641
	internal PrefabPoolCollection pool;

	// Token: 0x040021C2 RID: 8642
	private bool Clientside;

	// Token: 0x040021C3 RID: 8643
	private bool Serverside;
}
