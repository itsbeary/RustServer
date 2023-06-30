using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Rust;
using Rust.Water5;
using UnityEngine;

// Token: 0x0200070C RID: 1804
[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	// Token: 0x060032BD RID: 12989 RVA: 0x000063A5 File Offset: 0x000045A5
	private void EditorInitialize()
	{
	}

	// Token: 0x060032BE RID: 12990 RVA: 0x000063A5 File Offset: 0x000045A5
	private void EditorShutdown()
	{
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x060032BF RID: 12991 RVA: 0x001388BC File Offset: 0x00136ABC
	// (set) Token: 0x060032C0 RID: 12992 RVA: 0x001388C3 File Offset: 0x00136AC3
	public static WaterSystem Instance { get; private set; }

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x060032C1 RID: 12993 RVA: 0x001388CB File Offset: 0x00136ACB
	// (set) Token: 0x060032C2 RID: 12994 RVA: 0x001388D2 File Offset: 0x00136AD2
	public static WaterCollision Collision { get; private set; }

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x060032C3 RID: 12995 RVA: 0x001388DA File Offset: 0x00136ADA
	// (set) Token: 0x060032C4 RID: 12996 RVA: 0x001388E1 File Offset: 0x00136AE1
	public static WaterBody Ocean { get; private set; }

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x060032C5 RID: 12997 RVA: 0x001388E9 File Offset: 0x00136AE9
	public static Material OceanMaterial
	{
		get
		{
			WaterSystem instance = WaterSystem.Instance;
			if (instance == null)
			{
				return null;
			}
			return instance.oceanMaterial;
		}
	}

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x060032C6 RID: 12998 RVA: 0x001388FB File Offset: 0x00136AFB
	public static ListHashSet<WaterCamera> WaterCameras { get; } = new ListHashSet<WaterCamera>(8);

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x060032C7 RID: 12999 RVA: 0x00138902 File Offset: 0x00136B02
	public static HashSet<WaterBody> WaterBodies { get; } = new HashSet<WaterBody>();

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x060032C8 RID: 13000 RVA: 0x00138909 File Offset: 0x00136B09
	public static HashSet<WaterDepthMask> DepthMasks { get; } = new HashSet<WaterDepthMask>();

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x060032C9 RID: 13001 RVA: 0x00138910 File Offset: 0x00136B10
	// (set) Token: 0x060032CA RID: 13002 RVA: 0x00138917 File Offset: 0x00136B17
	public static float WaveTime { get; private set; }

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x060032CB RID: 13003 RVA: 0x0013891F File Offset: 0x00136B1F
	// (set) Token: 0x060032CC RID: 13004 RVA: 0x00138926 File Offset: 0x00136B26
	public static float OceanLevel
	{
		get
		{
			return WaterSystem.oceanLevel;
		}
		set
		{
			value = Mathf.Max(value, 0f);
			if (!Mathf.Approximately(WaterSystem.oceanLevel, value))
			{
				WaterSystem.oceanLevel = value;
				WaterSystem.UpdateOceanLevel();
			}
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x060032CD RID: 13005 RVA: 0x00138950 File Offset: 0x00136B50
	// (set) Token: 0x060032CE RID: 13006 RVA: 0x00138958 File Offset: 0x00136B58
	public bool IsInitialized { get; private set; }

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x060032CF RID: 13007 RVA: 0x00138961 File Offset: 0x00136B61
	public int Layer
	{
		get
		{
			return base.gameObject.layer;
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x060032D0 RID: 13008 RVA: 0x0013896E File Offset: 0x00136B6E
	public int Reflections
	{
		get
		{
			return Water.reflections;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x060032D1 RID: 13009 RVA: 0x00138975 File Offset: 0x00136B75
	public float WindowDirection
	{
		get
		{
			return this.oceanSettings.windDirection;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x060032D2 RID: 13010 RVA: 0x00138982 File Offset: 0x00136B82
	public float[] OctaveScales
	{
		get
		{
			return this.oceanSettings.octaveScales;
		}
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x0013898F File Offset: 0x00136B8F
	private void CheckInstance()
	{
		WaterSystem.Instance = ((WaterSystem.Instance != null) ? WaterSystem.Instance : this);
		WaterSystem.Collision = ((WaterSystem.Collision != null) ? WaterSystem.Collision : base.GetComponent<WaterCollision>());
	}

	// Token: 0x060032D4 RID: 13012 RVA: 0x001389CA File Offset: 0x00136BCA
	private void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x060032D5 RID: 13013 RVA: 0x001389D2 File Offset: 0x00136BD2
	private void OnEnable()
	{
		this.CheckInstance();
		this.oceanSimulation = new OceanSimulation(this.oceanSettings);
		this.IsInitialized = true;
	}

	// Token: 0x060032D6 RID: 13014 RVA: 0x001389F2 File Offset: 0x00136BF2
	private void OnDisable()
	{
		if (UnityEngine.Application.isPlaying && Rust.Application.isQuitting)
		{
			return;
		}
		this.oceanSimulation.Dispose();
		this.oceanSimulation = null;
		this.IsInitialized = false;
		WaterSystem.Instance = null;
	}

	// Token: 0x060032D7 RID: 13015 RVA: 0x00138A24 File Offset: 0x00136C24
	private void Update()
	{
		using (TimeWarning.New("UpdateWaves", 0))
		{
			this.UpdateOceanSimulation();
		}
	}

	// Token: 0x060032D8 RID: 13016 RVA: 0x00138A60 File Offset: 0x00136C60
	public static bool Trace(Ray ray, out Vector3 position, float maxDist = 100f)
	{
		if (WaterSystem.Instance == null)
		{
			position = Vector3.zero;
			return false;
		}
		return WaterSystem.Instance.oceanSimulation.Trace(ray, maxDist, out position);
	}

	// Token: 0x060032D9 RID: 13017 RVA: 0x00138A90 File Offset: 0x00136C90
	public static bool Trace(Ray ray, out Vector3 position, out Vector3 normal, float maxDist = 100f)
	{
		if (WaterSystem.Instance == null)
		{
			position = Vector3.zero;
			normal = Vector3.zero;
			return false;
		}
		normal = Vector3.up;
		return WaterSystem.Instance.oceanSimulation.Trace(ray, maxDist, out position);
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x00138AE0 File Offset: 0x00136CE0
	public static void GetHeightArray_Managed(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		if (TerrainTexturing.Instance != null)
		{
			for (int i = 0; i < posUV.Length; i++)
			{
				shore[i] = ((TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.GetCoarseVectorToShore(posUV[i]) : Vector3.zero);
			}
		}
		for (int j = 0; j < pos.Length; j++)
		{
			terrainHeight[j] = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(posUV[j]) : 0f);
			if (WaterSystem.Instance != null)
			{
				waterHeight[j] = WaterSystem.Instance.oceanSimulation.GetHeight(pos[j].XZ3D());
			}
		}
		float num = WaterSystem.OceanLevel;
		for (int k = 0; k < posUV.Length; k++)
		{
			Vector2 vector = posUV[k];
			float num2 = ((TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(vector) : 0f);
			if (WaterSystem.Instance != null && (double)num2 <= (double)num + 0.01)
			{
				waterHeight[k] = num + waterHeight[k];
			}
			else
			{
				waterHeight[k] = num2;
			}
		}
	}

	// Token: 0x060032DB RID: 13019 RVA: 0x00138C05 File Offset: 0x00136E05
	public static void GetHeightArray(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		WaterSystem.GetHeightArray_Managed(pos, posUV, shore, terrainHeight, waterHeight);
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x00138C12 File Offset: 0x00136E12
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetHeight(Vector3 pos)
	{
		if (WaterSystem.Instance == null)
		{
			return WaterSystem.OceanLevel;
		}
		return WaterSystem.Instance.oceanSimulation.GetHeight(pos) + WaterSystem.OceanLevel;
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x00138C3D File Offset: 0x00136E3D
	public static Vector3 GetNormal(Vector3 pos)
	{
		return Vector3.up;
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x00138C44 File Offset: 0x00136E44
	public static float MinLevel()
	{
		if (WaterSystem.Instance == null)
		{
			return WaterSystem.OceanLevel;
		}
		return WaterSystem.Instance.oceanSimulation.MinLevel() + WaterSystem.OceanLevel;
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x00138C6E File Offset: 0x00136E6E
	public static float MaxLevel()
	{
		if (WaterSystem.Instance == null)
		{
			return WaterSystem.OceanLevel;
		}
		return WaterSystem.Instance.oceanSimulation.MaxLevel() + WaterSystem.OceanLevel;
	}

	// Token: 0x060032E0 RID: 13024 RVA: 0x00138C98 File Offset: 0x00136E98
	public static void RegisterBody(WaterBody body)
	{
		if (body.Type == WaterBodyType.Ocean)
		{
			if (WaterSystem.Ocean == null)
			{
				WaterSystem.Ocean = body;
				body.Transform.position = body.Transform.position.WithY(WaterSystem.OceanLevel);
			}
			else if (WaterSystem.Ocean != body)
			{
				Debug.LogWarning("[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterSystem.WaterBodies.Add(body);
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x00138D07 File Offset: 0x00136F07
	public static void UnregisterBody(WaterBody body)
	{
		if (body == WaterSystem.Ocean)
		{
			WaterSystem.Ocean = null;
		}
		WaterSystem.WaterBodies.Remove(body);
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x00138D28 File Offset: 0x00136F28
	private static void UpdateOceanLevel()
	{
		if (WaterSystem.Ocean != null)
		{
			WaterSystem.Ocean.Transform.position = WaterSystem.Ocean.Transform.position.WithY(WaterSystem.OceanLevel);
		}
		foreach (WaterBody waterBody in WaterSystem.WaterBodies)
		{
			waterBody.OnOceanLevelChanged(WaterSystem.OceanLevel);
		}
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x00138DB4 File Offset: 0x00136FB4
	private void UpdateOceanSimulation()
	{
		if (Water.scaled_time)
		{
			WaterSystem.WaveTime += UnityEngine.Time.deltaTime;
		}
		else
		{
			WaterSystem.WaveTime = UnityEngine.Time.realtimeSinceStartup;
		}
		if (Weather.ocean_time >= 0f)
		{
			WaterSystem.WaveTime = Weather.ocean_time;
		}
		float num = (SingletonComponent<Climate>.Instance ? SingletonComponent<Climate>.Instance.WeatherState.OceanScale : 4f);
		OceanSimulation oceanSimulation = this.oceanSimulation;
		if (oceanSimulation == null)
		{
			return;
		}
		oceanSimulation.Update(WaterSystem.WaveTime, UnityEngine.Time.deltaTime, num);
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x00138E38 File Offset: 0x00137038
	public void Refresh()
	{
		this.oceanSimulation.Dispose();
		this.oceanSimulation = new OceanSimulation(this.oceanSettings);
	}

	// Token: 0x0400298D RID: 10637
	private static float oceanLevel = 0f;

	// Token: 0x04002995 RID: 10645
	[Header("Ocean Settings")]
	public OceanSettings oceanSettings;

	// Token: 0x04002996 RID: 10646
	public OceanSimulation oceanSimulation;

	// Token: 0x04002997 RID: 10647
	public WaterQuality Quality = WaterQuality.High;

	// Token: 0x04002998 RID: 10648
	public Material oceanMaterial;

	// Token: 0x04002999 RID: 10649
	public WaterSystem.RenderingSettings Rendering = new WaterSystem.RenderingSettings();

	// Token: 0x0400299A RID: 10650
	public int patchSize = 100;

	// Token: 0x0400299B RID: 10651
	public int patchCount = 4;

	// Token: 0x0400299C RID: 10652
	public float patchScale = 1f;

	// Token: 0x02000E4A RID: 3658
	[Serializable]
	public class RenderingSettings
	{
		// Token: 0x04004B53 RID: 19283
		public Vector4[] TessellationQuality;

		// Token: 0x04004B54 RID: 19284
		public WaterSystem.RenderingSettings.SkyProbe SkyReflections;

		// Token: 0x04004B55 RID: 19285
		public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;

		// Token: 0x02000FE4 RID: 4068
		[Serializable]
		public class SkyProbe
		{
			// Token: 0x04005198 RID: 20888
			public float ProbeUpdateInterval = 1f;

			// Token: 0x04005199 RID: 20889
			public bool TimeSlicing = true;
		}

		// Token: 0x02000FE5 RID: 4069
		[Serializable]
		public class SSR
		{
			// Token: 0x0400519A RID: 20890
			public float FresnelCutoff = 0.02f;

			// Token: 0x0400519B RID: 20891
			public float ThicknessMin = 1f;

			// Token: 0x0400519C RID: 20892
			public float ThicknessMax = 20f;

			// Token: 0x0400519D RID: 20893
			public float ThicknessStartDist = 40f;

			// Token: 0x0400519E RID: 20894
			public float ThicknessEndDist = 100f;
		}
	}
}
