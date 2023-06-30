using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A06 RID: 2566
	public class Map : BaseHandler<AppEmpty>
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06003D35 RID: 15669 RVA: 0x001668CC File Offset: 0x00164ACC
		protected override double TokenCost
		{
			get
			{
				return 5.0;
			}
		}

		// Token: 0x06003D36 RID: 15670 RVA: 0x001668D8 File Offset: 0x00164AD8
		public override void Execute()
		{
			if (Map._imageData == null)
			{
				base.SendError("no_map");
				return;
			}
			AppMap appMap = Pool.Get<AppMap>();
			appMap.width = (uint)Map._width;
			appMap.height = (uint)Map._height;
			appMap.oceanMargin = 500;
			appMap.jpgImage = Map._imageData;
			appMap.background = Map._background;
			appMap.monuments = Pool.GetList<AppMap.Monument>();
			if (TerrainMeta.Path != null && TerrainMeta.Path.Landmarks != null)
			{
				foreach (LandmarkInfo landmarkInfo in TerrainMeta.Path.Landmarks)
				{
					if (landmarkInfo.shouldDisplayOnMap)
					{
						Vector2 vector = Util.WorldToMap(landmarkInfo.transform.position);
						AppMap.Monument monument = Pool.Get<AppMap.Monument>();
						monument.token = (landmarkInfo.displayPhrase.IsValid() ? landmarkInfo.displayPhrase.token : landmarkInfo.transform.root.name);
						monument.x = vector.x;
						monument.y = vector.y;
						appMap.monuments.Add(monument);
					}
				}
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.map = appMap;
			base.Send(appResponse);
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x00166A38 File Offset: 0x00164C38
		public static void PopulateCache()
		{
			Map.RenderToCache();
		}

		// Token: 0x06003D38 RID: 15672 RVA: 0x00166A40 File Offset: 0x00164C40
		private static void RenderToCache()
		{
			Map._imageData = null;
			Map._width = 0;
			Map._height = 0;
			try
			{
				Color color;
				Map._imageData = MapImageRenderer.Render(out Map._width, out Map._height, out color, 0.5f, true);
				Map._background = "#" + ColorUtility.ToHtmlStringRGB(color);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Exception thrown when rendering map for the app: {0}", ex));
			}
			if (Map._imageData == null)
			{
				Debug.LogError("Map image is null! App users will not be able to see the map.");
			}
		}

		// Token: 0x04003758 RID: 14168
		private static int _width;

		// Token: 0x04003759 RID: 14169
		private static int _height;

		// Token: 0x0400375A RID: 14170
		private static byte[] _imageData;

		// Token: 0x0400375B RID: 14171
		private static string _background;
	}
}
