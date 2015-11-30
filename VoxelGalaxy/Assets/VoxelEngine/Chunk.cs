using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
	public static int CHUNK_SIZE = 16;
	public bool update = false;
	public bool rendered = false;
	
	public Galaxy world;
	public WorldPos pos;
	
	public Block[,,] blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
	
	MeshFilter filter;
	MeshCollider coll;
	
	void Start()
	{
		filter = gameObject.GetComponent<MeshFilter>();
		coll = gameObject.GetComponent<MeshCollider>();
	}
	
	void Update()
	{
		if (update)
		{
			update = false;
			UpdateChunk();
		}
	}
	
	void UpdateChunk()
	{
		rendered = true;
		MeshData meshData = new MeshData();
		
		for (int x = 0; x < CHUNK_SIZE; x++)
		{
			for (int y = 0; y < CHUNK_SIZE; y++)
			{
				for (int z = 0; z < CHUNK_SIZE; z++)
				{
					meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
				}
			}
		}
		
		RenderMesh(meshData);
	}
	
	void RenderMesh(MeshData meshData)
	{
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.ToArray();
		filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.RecalculateNormals();
		
		coll.sharedMesh = null;
		Mesh mesh = new Mesh();
		mesh.vertices = meshData.colVertices.ToArray();
		mesh.triangles = meshData.colTriangles.ToArray();
		mesh.RecalculateNormals();
		coll.sharedMesh = mesh;
	}
	
	public void SetBlockUnmodified()
	{
		foreach (Block block in blocks)
		{
			block.changed = false;
		}
	}
	
	public static bool InRange(int index)
	{
		if (index < 0 || index >= CHUNK_SIZE)
		{
			return false;
		}
		
		return true;
	}
	
	public Block GetBlock(int x, int y, int z)
	{
		if (InRange(x) && InRange(y) && InRange(z)) 
		{
			return blocks [x, y, z];
		}
		
		return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
	}
	
	public void SetBlock(int x, int y, int z, Block block)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			blocks[x, y, z] = block;
		}
		else
		{
			world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
		}
	}
}
