using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007F7 RID: 2039
public class MapLayerRenderer : SingletonComponent<MapLayerRenderer>
{
	// Token: 0x0600358E RID: 13710 RVA: 0x00146660 File Offset: 0x00144860
	private void RenderDungeonsLayer()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = MapLayerRenderer.FindDungeon(MainCamera.isValid ? MainCamera.position : Vector3.zero, 200f);
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		MapLayer mapLayer = MapLayer.Dungeons;
		if ((currentlyRenderedLayer.GetValueOrDefault() == mapLayer) & (currentlyRenderedLayer != null))
		{
			NetworkableId? currentlyRenderedDungeon = this._currentlyRenderedDungeon;
			NetworkableId? networkableId;
			if (proceduralDynamicDungeon == null)
			{
				networkableId = null;
			}
			else
			{
				Networkable net = proceduralDynamicDungeon.net;
				networkableId = ((net != null) ? new NetworkableId?(net.ID) : null);
			}
			if (currentlyRenderedDungeon == networkableId)
			{
				return;
			}
		}
		this._currentlyRenderedLayer = new MapLayer?(MapLayer.Dungeons);
		NetworkableId? networkableId2;
		if (proceduralDynamicDungeon == null)
		{
			networkableId2 = null;
		}
		else
		{
			Networkable net2 = proceduralDynamicDungeon.net;
			networkableId2 = ((net2 != null) ? new NetworkableId?(net2.ID) : null);
		}
		this._currentlyRenderedDungeon = networkableId2;
		using (CommandBuffer commandBuffer = this.BuildCommandBufferDungeons(proceduralDynamicDungeon))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x00146788 File Offset: 0x00144988
	private CommandBuffer BuildCommandBufferDungeons(ProceduralDynamicDungeon closest)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "DungeonsLayer Render"
		};
		if (closest != null && closest.spawnedCells != null)
		{
			Matrix4x4 matrix4x = Matrix4x4.Translate(closest.mapOffset);
			foreach (ProceduralDungeonCell proceduralDungeonCell in closest.spawnedCells)
			{
				if (!(proceduralDungeonCell == null) && proceduralDungeonCell.mapRenderers != null && proceduralDungeonCell.mapRenderers.Length != 0)
				{
					foreach (MeshRenderer meshRenderer in proceduralDungeonCell.mapRenderers)
					{
						MeshFilter meshFilter;
						if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
						{
							Mesh sharedMesh = meshFilter.sharedMesh;
							int subMeshCount = sharedMesh.subMeshCount;
							Matrix4x4 matrix4x2 = matrix4x * meshRenderer.transform.localToWorldMatrix;
							for (int j = 0; j < subMeshCount; j++)
							{
								commandBuffer.DrawMesh(sharedMesh, matrix4x2, this.renderMaterial, j);
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x001468B4 File Offset: 0x00144AB4
	public static ProceduralDynamicDungeon FindDungeon(Vector3 position, float maxDist = 200f)
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = null;
		float num = 100000f;
		foreach (ProceduralDynamicDungeon proceduralDynamicDungeon2 in ProceduralDynamicDungeon.dungeons)
		{
			if (!(proceduralDynamicDungeon2 == null) && proceduralDynamicDungeon2.isClient)
			{
				float num2 = Vector3.Distance(position, proceduralDynamicDungeon2.transform.position);
				if (num2 <= maxDist && num2 <= num)
				{
					proceduralDynamicDungeon = proceduralDynamicDungeon2;
					num = num2;
				}
			}
		}
		return proceduralDynamicDungeon;
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x0014693C File Offset: 0x00144B3C
	private void RenderTrainLayer()
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferTrainTunnels())
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x00146974 File Offset: 0x00144B74
	private CommandBuffer BuildCommandBufferTrainTunnels()
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "TrainLayer Render"
		};
		foreach (DungeonGridCell dungeonGridCell in TerrainMeta.Path.DungeonGridCells)
		{
			if (dungeonGridCell.MapRenderers != null && dungeonGridCell.MapRenderers.Length != 0)
			{
				foreach (MeshRenderer meshRenderer in dungeonGridCell.MapRenderers)
				{
					MeshFilter meshFilter;
					if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
					{
						Mesh sharedMesh = meshFilter.sharedMesh;
						int subMeshCount = sharedMesh.subMeshCount;
						Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
						for (int j = 0; j < subMeshCount; j++)
						{
							commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x00146A68 File Offset: 0x00144C68
	private void RenderUnderwaterLabs(int floor)
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferUnderwaterLabs(floor))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00146AA0 File Offset: 0x00144CA0
	public int GetUnderwaterLabFloorCount()
	{
		if (this._underwaterLabFloorCount != null)
		{
			return this._underwaterLabFloorCount.Value;
		}
		List<DungeonBaseInfo> dungeonBaseEntrances = TerrainMeta.Path.DungeonBaseEntrances;
		int num;
		if (dungeonBaseEntrances == null || dungeonBaseEntrances.Count <= 0)
		{
			num = 0;
		}
		else
		{
			num = dungeonBaseEntrances.Max((DungeonBaseInfo l) => l.Floors.Count);
		}
		this._underwaterLabFloorCount = new int?(num);
		return this._underwaterLabFloorCount.Value;
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x00146B1C File Offset: 0x00144D1C
	private CommandBuffer BuildCommandBufferUnderwaterLabs(int floor)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "UnderwaterLabLayer Render"
		};
		foreach (DungeonBaseInfo dungeonBaseInfo in TerrainMeta.Path.DungeonBaseEntrances)
		{
			if (dungeonBaseInfo.Floors.Count > floor)
			{
				foreach (DungeonBaseLink dungeonBaseLink in dungeonBaseInfo.Floors[floor].Links)
				{
					if (dungeonBaseLink.MapRenderers != null && dungeonBaseLink.MapRenderers.Length != 0)
					{
						foreach (MeshRenderer meshRenderer in dungeonBaseLink.MapRenderers)
						{
							MeshFilter meshFilter;
							if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
							{
								Mesh sharedMesh = meshFilter.sharedMesh;
								int subMeshCount = sharedMesh.subMeshCount;
								Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
								for (int j = 0; j < subMeshCount; j++)
								{
									commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
								}
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x00146C74 File Offset: 0x00144E74
	public void Render(MapLayer layer)
	{
		if (layer < MapLayer.TrainTunnels)
		{
			return;
		}
		if (layer == MapLayer.Dungeons)
		{
			this.RenderDungeonsLayer();
			return;
		}
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		if ((layer == currentlyRenderedLayer.GetValueOrDefault()) & (currentlyRenderedLayer != null))
		{
			return;
		}
		this._currentlyRenderedLayer = new MapLayer?(layer);
		if (layer == MapLayer.TrainTunnels)
		{
			this.RenderTrainLayer();
			return;
		}
		if (layer >= MapLayer.Underwater1 && layer <= MapLayer.Underwater8)
		{
			this.RenderUnderwaterLabs(layer - MapLayer.Underwater1);
		}
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x00146CD8 File Offset: 0x00144ED8
	private void RenderImpl(CommandBuffer cb)
	{
		double num = World.Size * 1.5;
		this.renderCamera.orthographicSize = (float)num / 2f;
		this.renderCamera.RemoveAllCommandBuffers();
		this.renderCamera.AddCommandBuffer(this.cameraEvent, cb);
		this.renderCamera.Render();
		this.renderCamera.RemoveAllCommandBuffers();
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x00146D3D File Offset: 0x00144F3D
	public static MapLayerRenderer GetOrCreate()
	{
		if (SingletonComponent<MapLayerRenderer>.Instance != null)
		{
			return SingletonComponent<MapLayerRenderer>.Instance;
		}
		return GameManager.server.CreatePrefab("assets/prefabs/engine/maplayerrenderer.prefab", Vector3.zero, Quaternion.identity, true).GetComponent<MapLayerRenderer>();
	}

	// Token: 0x04002DD1 RID: 11729
	private NetworkableId? _currentlyRenderedDungeon;

	// Token: 0x04002DD2 RID: 11730
	private int? _underwaterLabFloorCount;

	// Token: 0x04002DD3 RID: 11731
	public Camera renderCamera;

	// Token: 0x04002DD4 RID: 11732
	public CameraEvent cameraEvent;

	// Token: 0x04002DD5 RID: 11733
	public Material renderMaterial;

	// Token: 0x04002DD6 RID: 11734
	private MapLayer? _currentlyRenderedLayer;
}
