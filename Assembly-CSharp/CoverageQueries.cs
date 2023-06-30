using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200098D RID: 2445
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	// Token: 0x040034C9 RID: 13513
	public bool debug;

	// Token: 0x040034CA RID: 13514
	public float depthBias = -0.1f;

	// Token: 0x02000EE0 RID: 3808
	public class BufferSet
	{
		// Token: 0x06005392 RID: 21394 RVA: 0x001B2B3C File Offset: 0x001B0D3C
		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		// Token: 0x06005393 RID: 21395 RVA: 0x001B2B48 File Offset: 0x001B0D48
		public void Dispose(bool data = true)
		{
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
			}
		}

		// Token: 0x06005394 RID: 21396 RVA: 0x001B2BC4 File Offset: 0x001B0DC4
		public bool CheckResize(int count)
		{
			if (count > this.inputData.Length || (this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				this.width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				this.height = Mathf.CeilToInt((float)count / (float)this.width);
				this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
				this.inputTexture.name = "_Input";
				this.inputTexture.filterMode = FilterMode.Point;
				this.inputTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				this.resultTexture.name = "_Result";
				this.resultTexture.filterMode = FilterMode.Point;
				this.resultTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture.useMipMap = false;
				this.resultTexture.Create();
				int num = this.resultData.Length;
				int num2 = this.width * this.height;
				Array.Resize<Color>(ref this.inputData, num2);
				Array.Resize<Color32>(ref this.resultData, num2);
				Color32 color = new Color32(byte.MaxValue, 0, 0, 0);
				for (int i = num; i < num2; i++)
				{
					this.resultData[i] = color;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005395 RID: 21397 RVA: 0x001B2D20 File Offset: 0x001B0F20
		public void UploadData()
		{
			if (this.inputData.Length != 0)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
			}
		}

		// Token: 0x06005396 RID: 21398 RVA: 0x001B2D48 File Offset: 0x001B0F48
		public void Dispatch(int count)
		{
			if (this.inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				this.coverageMat.SetTexture("_Input", this.inputTexture);
				Graphics.Blit(this.inputTexture, this.resultTexture, this.coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		// Token: 0x06005397 RID: 21399 RVA: 0x001B2D9D File Offset: 0x001B0F9D
		public void IssueRead()
		{
			if (this.asyncRequests.Count < 10)
			{
				this.asyncRequests.Enqueue(AsyncGPUReadback.Request(this.resultTexture, 0, null));
			}
		}

		// Token: 0x06005398 RID: 21400 RVA: 0x001B2DC8 File Offset: 0x001B0FC8
		public void GetResults()
		{
			if (this.resultData.Length != 0)
			{
				while (this.asyncRequests.Count > 0)
				{
					AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
					if (asyncGPUReadbackRequest.hasError)
					{
						this.asyncRequests.Dequeue();
					}
					else
					{
						if (!asyncGPUReadbackRequest.done)
						{
							break;
						}
						NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
						for (int i = 0; i < data.Length; i++)
						{
							this.resultData[i] = data[i];
						}
						this.asyncRequests.Dequeue();
					}
				}
			}
		}

		// Token: 0x04004DAA RID: 19882
		public int width;

		// Token: 0x04004DAB RID: 19883
		public int height;

		// Token: 0x04004DAC RID: 19884
		public Texture2D inputTexture;

		// Token: 0x04004DAD RID: 19885
		public RenderTexture resultTexture;

		// Token: 0x04004DAE RID: 19886
		public Color[] inputData = new Color[0];

		// Token: 0x04004DAF RID: 19887
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004DB0 RID: 19888
		private Material coverageMat;

		// Token: 0x04004DB1 RID: 19889
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004DB2 RID: 19890
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();
	}

	// Token: 0x02000EE1 RID: 3809
	public enum RadiusSpace
	{
		// Token: 0x04004DB4 RID: 19892
		ScreenNormalized,
		// Token: 0x04004DB5 RID: 19893
		World
	}

	// Token: 0x02000EE2 RID: 3810
	public class Query
	{
		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x0600539A RID: 21402 RVA: 0x001B2E80 File Offset: 0x001B1080
		public bool IsRegistered
		{
			get
			{
				return this.intern.id >= 0;
			}
		}

		// Token: 0x04004DB6 RID: 19894
		public CoverageQueries.Query.Input input;

		// Token: 0x04004DB7 RID: 19895
		public CoverageQueries.Query.Internal intern;

		// Token: 0x04004DB8 RID: 19896
		public CoverageQueries.Query.Result result;

		// Token: 0x02000FEA RID: 4074
		public struct Input
		{
			// Token: 0x040051AC RID: 20908
			public Vector3 position;

			// Token: 0x040051AD RID: 20909
			public CoverageQueries.RadiusSpace radiusSpace;

			// Token: 0x040051AE RID: 20910
			public float radius;

			// Token: 0x040051AF RID: 20911
			public int sampleCount;

			// Token: 0x040051B0 RID: 20912
			public float smoothingSpeed;
		}

		// Token: 0x02000FEB RID: 4075
		public struct Internal
		{
			// Token: 0x060055E1 RID: 21985 RVA: 0x001BB2A0 File Offset: 0x001B94A0
			public void Reset()
			{
				this.id = -1;
			}

			// Token: 0x040051B1 RID: 20913
			public int id;
		}

		// Token: 0x02000FEC RID: 4076
		public struct Result
		{
			// Token: 0x060055E2 RID: 21986 RVA: 0x001BB2AC File Offset: 0x001B94AC
			public void Reset()
			{
				this.passed = 0;
				this.coverage = 0f;
				this.smoothCoverage = 0f;
				this.weightedCoverage = 0f;
				this.weightedSmoothCoverage = 0f;
				this.originOccluded = true;
				this.frame = -1;
				this.originVisibility = 0f;
				this.originSmoothVisibility = 0f;
			}

			// Token: 0x040051B2 RID: 20914
			public int passed;

			// Token: 0x040051B3 RID: 20915
			public float coverage;

			// Token: 0x040051B4 RID: 20916
			public float smoothCoverage;

			// Token: 0x040051B5 RID: 20917
			public float weightedCoverage;

			// Token: 0x040051B6 RID: 20918
			public float weightedSmoothCoverage;

			// Token: 0x040051B7 RID: 20919
			public bool originOccluded;

			// Token: 0x040051B8 RID: 20920
			public int frame;

			// Token: 0x040051B9 RID: 20921
			public float originVisibility;

			// Token: 0x040051BA RID: 20922
			public float originSmoothVisibility;
		}
	}
}
