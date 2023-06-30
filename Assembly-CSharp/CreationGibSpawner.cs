using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public class CreationGibSpawner : BaseMonoBehaviour
{
	// Token: 0x06002934 RID: 10548 RVA: 0x000FDB14 File Offset: 0x000FBD14
	public GameObjectRef GetEffectForMaterial(PhysicMaterial mat)
	{
		foreach (CreationGibSpawner.EffectMaterialPair effectMaterialPair in this.effectLookup)
		{
			if (effectMaterialPair.material == mat)
			{
				return effectMaterialPair.effect;
			}
		}
		return this.effectLookup[0].effect;
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x000FDB5C File Offset: 0x000FBD5C
	public void SetDelay(float newDelay)
	{
		this.startDelay = newDelay;
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000FDB65 File Offset: 0x000FBD65
	public void FinishSpawn()
	{
		if (this.startDelay == 0f)
		{
			this.Init();
			return;
		}
		base.Invoke(new Action(this.Init), this.startDelay);
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000FDB93 File Offset: 0x000FBD93
	public float GetProgress(float delay)
	{
		if (!this.started)
		{
			return 0f;
		}
		if (this.duration == 0f)
		{
			return 1f;
		}
		return Mathf.Clamp01((Time.time - (this.startTime + delay)) / this.duration);
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000FDBD0 File Offset: 0x000FBDD0
	public void AddConditionalGibSource(GameObject cGibSource, Vector3 pos, Quaternion rot)
	{
		Debug.Log("Adding conditional gib source");
		CreationGibSpawner.ConditionalGibSource conditionalGibSource;
		conditionalGibSource.source = cGibSource;
		conditionalGibSource.pos = pos;
		conditionalGibSource.rot = rot;
		this.conditionalGibSources.Add(conditionalGibSource);
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000FDC0C File Offset: 0x000FBE0C
	public void SetGibSource(GameObject newGibSource)
	{
		GameObject gameObject = newGibSource;
		for (int i = 0; i < this.GibReplacements.Length; i++)
		{
			if (this.GibReplacements[i].oldGib == newGibSource)
			{
				gameObject = this.GibReplacements[i].newGib;
				break;
			}
		}
		this.gibSource = gameObject;
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000FDC5C File Offset: 0x000FBE5C
	private int SortsGibs(Transform a, Transform b)
	{
		MeshRenderer component = a.GetComponent<MeshRenderer>();
		MeshRenderer component2 = b.GetComponent<MeshRenderer>();
		if (!this.invert)
		{
			float num = ((component == null) ? a.localPosition.y : component.bounds.center.y);
			float num2 = ((component2 == null) ? b.localPosition.y : component2.bounds.center.y);
			return num.CompareTo(num2);
		}
		float num3 = ((component == null) ? a.localPosition.y : component.bounds.center.y);
		return ((component2 == null) ? b.localPosition.y : component2.bounds.center.y).CompareTo(num3);
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000FDD40 File Offset: 0x000FBF40
	public void Init()
	{
		this.started = true;
		this.startTime = Time.time;
		this.gibsInstance = UnityEngine.Object.Instantiate<GameObject>(this.gibSource, base.transform.position, base.transform.rotation);
		List<Transform> list = this.gibsInstance.GetComponentsInChildren<Transform>().ToList<Transform>();
		list.Remove(this.gibsInstance.transform);
		list.Sort(new Comparison<Transform>(this.SortsGibs));
		this.gibs = list;
		this.spawnPositions = new Vector3[this.gibs.Count];
		this.gibProgress = new float[this.gibs.Count];
		this.particles = new GameObject[this.gibs.Count];
		for (int i = 0; i < this.gibs.Count; i++)
		{
			Transform transform = this.gibs[i];
			this.spawnPositions[i] = transform.localPosition;
			this.gibProgress[i] = 0f;
			this.particles[i] = null;
			transform.localScale = Vector3.one * this.scaleCurve.Evaluate(0f);
			float x = this.spawnPositions[i].x;
			transform.transform.position += base.transform.right * this.GetPushDir(this.spawnPositions[i], transform) * this.buildCurve.Evaluate(0f) * this.buildDirection.x;
			transform.transform.position += base.transform.up * this.yCurve.Evaluate(0f);
			transform.transform.position += base.transform.forward * this.zCurve.Evaluate(0f);
		}
		base.Invoke(new Action(this.DestroyMe), this.duration + 0.05f);
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000FDF75 File Offset: 0x000FC175
	public float GetPushDir(Vector3 spawnPos, Transform theGib)
	{
		if (spawnPos.x < 0f)
		{
			return 1f;
		}
		return -1f;
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000FDF8F File Offset: 0x000FC18F
	public void DestroyMe()
	{
		UnityEngine.Object.Destroy(this.gibsInstance);
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x00029EBC File Offset: 0x000280BC
	public float GetStartDelay(Transform gib)
	{
		return 0f;
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x000FDF9C File Offset: 0x000FC19C
	public void Update()
	{
		if (!this.started)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = Mathf.CeilToInt((float)this.gibs.Count / 10f);
		for (int i = 0; i < this.gibs.Count; i++)
		{
			Transform transform = this.gibs[i];
			if (!(transform == base.transform))
			{
				if (deltaTime <= 0f)
				{
					break;
				}
				float num2 = 0.33f;
				float num3 = num2 / ((float)this.gibs.Count * num2) * (this.duration - num2);
				float num4 = (float)i * num3;
				if (Time.time - this.startTime >= num4)
				{
					MeshFilter component = transform.GetComponent<MeshFilter>();
					int seed = UnityEngine.Random.seed;
					UnityEngine.Random.seed = i + this.gibs.Count;
					bool flag = num <= 1 || UnityEngine.Random.Range(0, num) == 0;
					UnityEngine.Random.seed = seed;
					if (flag && this.particles[i] == null && component != null && component.sharedMesh != null)
					{
						if (component.sharedMesh.bounds.size.magnitude == 0f)
						{
							goto IL_3B8;
						}
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.smokeEffect);
						gameObject.transform.SetParent(transform);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localRotation = Quaternion.identity;
						ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
						MeshRenderer component3 = component.GetComponent<MeshRenderer>();
						ParticleSystem.ShapeModule shape = component2.shape;
						shape.shapeType = ParticleSystemShapeType.Box;
						shape.boxThickness = component3.bounds.extents;
						this.particles[i] = gameObject;
					}
					float num5 = Mathf.Clamp01(this.gibProgress[i] / num2);
					float num6 = Mathf.Clamp01((num5 + Time.deltaTime) / num2);
					this.gibProgress[i] = this.gibProgress[i] + Time.deltaTime;
					float num7 = this.scaleCurve.Evaluate(num6);
					transform.transform.localScale = new Vector3(num7, num7, num7);
					transform.transform.localScale += this.buildDirection * this.buildScaleCurve.Evaluate(num6) * this.buildScaleAdditionalAmount;
					transform.transform.localPosition = this.spawnPositions[i];
					transform.transform.position += base.transform.right * this.GetPushDir(this.spawnPositions[i], transform) * this.buildCurve.Evaluate(num6) * this.buildDirection.x;
					transform.transform.position += base.transform.up * this.buildCurve.Evaluate(num6) * this.buildDirection.y;
					transform.transform.position += base.transform.forward * this.buildCurve.Evaluate(num6) * this.buildDirection.z;
					if (num6 >= 1f && num6 > num5 && Time.time > this.nextEffectTime)
					{
						this.nextEffectTime = Time.time + this.effectSpacing;
						if (this.particles[i] != null)
						{
							this.particles[i].GetComponent<ParticleSystem>();
							this.particles[i].transform.SetParent(null, true);
							this.particles[i].BroadcastOnParentDestroying();
						}
					}
				}
			}
			IL_3B8:;
		}
	}

	// Token: 0x0400214B RID: 8523
	private GameObject gibSource;

	// Token: 0x0400214C RID: 8524
	public GameObject gibsInstance;

	// Token: 0x0400214D RID: 8525
	public float startTime;

	// Token: 0x0400214E RID: 8526
	public float duration = 1f;

	// Token: 0x0400214F RID: 8527
	public float buildScaleAdditionalAmount = 0.5f;

	// Token: 0x04002150 RID: 8528
	[Tooltip("Entire object will be scaled on xyz during duration by this curve")]
	public AnimationCurve scaleCurve;

	// Token: 0x04002151 RID: 8529
	[Tooltip("Object will be pushed out along transform.forward/right/up based on build direction by this amount")]
	public AnimationCurve buildCurve;

	// Token: 0x04002152 RID: 8530
	[Tooltip("Additional scaling to apply to object based on build direction")]
	public AnimationCurve buildScaleCurve;

	// Token: 0x04002153 RID: 8531
	public AnimationCurve xCurve;

	// Token: 0x04002154 RID: 8532
	public AnimationCurve yCurve;

	// Token: 0x04002155 RID: 8533
	public AnimationCurve zCurve;

	// Token: 0x04002156 RID: 8534
	public Vector3[] spawnPositions;

	// Token: 0x04002157 RID: 8535
	public GameObject[] particles;

	// Token: 0x04002158 RID: 8536
	public float[] gibProgress;

	// Token: 0x04002159 RID: 8537
	public PhysicMaterial physMaterial;

	// Token: 0x0400215A RID: 8538
	public List<Transform> gibs;

	// Token: 0x0400215B RID: 8539
	public bool started;

	// Token: 0x0400215C RID: 8540
	public GameObjectRef placeEffect;

	// Token: 0x0400215D RID: 8541
	public GameObject smokeEffect;

	// Token: 0x0400215E RID: 8542
	public float effectSpacing = 0.2f;

	// Token: 0x0400215F RID: 8543
	public bool invert;

	// Token: 0x04002160 RID: 8544
	public Vector3 buildDirection;

	// Token: 0x04002161 RID: 8545
	[Horizontal(1, 0)]
	public CreationGibSpawner.GibReplacement[] GibReplacements;

	// Token: 0x04002162 RID: 8546
	public CreationGibSpawner.EffectMaterialPair[] effectLookup;

	// Token: 0x04002163 RID: 8547
	private float startDelay;

	// Token: 0x04002164 RID: 8548
	public List<CreationGibSpawner.ConditionalGibSource> conditionalGibSources = new List<CreationGibSpawner.ConditionalGibSource>();

	// Token: 0x04002165 RID: 8549
	private float nextEffectTime = float.NegativeInfinity;

	// Token: 0x02000D48 RID: 3400
	[Serializable]
	public class GibReplacement
	{
		// Token: 0x0400475A RID: 18266
		public GameObject oldGib;

		// Token: 0x0400475B RID: 18267
		public GameObject newGib;
	}

	// Token: 0x02000D49 RID: 3401
	[Serializable]
	public class EffectMaterialPair
	{
		// Token: 0x0400475C RID: 18268
		public PhysicMaterial material;

		// Token: 0x0400475D RID: 18269
		public GameObjectRef effect;
	}

	// Token: 0x02000D4A RID: 3402
	[Serializable]
	public struct ConditionalGibSource
	{
		// Token: 0x0400475E RID: 18270
		public GameObject source;

		// Token: 0x0400475F RID: 18271
		public Vector3 pos;

		// Token: 0x04004760 RID: 18272
		public Quaternion rot;
	}
}
