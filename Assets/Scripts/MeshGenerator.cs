using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour {

	MeshRenderer mr;
	MeshFilter mf;
	MeshCollider mc;

	public int width;
	public int height;

	Vector3 [] deformation;

	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
		mf.mesh = new Mesh();

		deformation = new Vector3[width * height];

		GenerateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray, out hit, 100))
			{
				Vector3 contact = hit.point;

				for(int x=0; x<width; x++)
				{
					for(int y=0; y<height; y++)
					{
						int i = Index2Dto1D(x,y);
						float distance = Vector3.Distance(mf.mesh.vertices[i], contact);
						float effect =  MathUtils.Map(distance, 0f, 1.5f, 2f, 0f);
						effect = Mathf.Clamp(effect, 0f, 2f);
						deformation[i] += new Vector3(0f, 0f, effect);
					}
				}

				GenerateMesh();
			}
		}
	}

	void GenerateMesh()
	{
		mf.mesh.Clear();

		Vector3 [] verts = new Vector3[width * height];
		Vector3 [] norms = new Vector3[width * height];
		int[] triangles = new int[width * height * 6];
		Vector2 [] uvs = new Vector2[width * height];
		Vector2 uvFactors = new Vector2(1.0f/(width-1), 1.0f/(height-1));

		for(int x=0; x<width; x++)
		{
			for(int y=0; y<height; y++)
			{
				int i = Index2Dto1D(x,y);
				verts[i] = new Vector3(x, y, 0f) + deformation[i];
				uvs[i] = new Vector2(x*uvFactors.x, y*uvFactors.y);
			}
		}

		int index = 0;
		for(int y = 0; y< height-1; y++){
			for(int x = 0; x<width-1; x++ )
			{
				triangles[index] = Index2Dto1D(x,y);
				triangles[index+1] = Index2Dto1D(x,y+1);
				triangles[index+2] = Index2Dto1D(x+1,y);

				triangles[index+3] = Index2Dto1D(x,y+1);
				triangles[index+4] = Index2Dto1D(x+1,y+1);
				triangles[index+5] = Index2Dto1D(x+1,y);
				index += 6;
			}
		}

		mf.mesh.vertices = verts;
		mf.mesh.triangles = triangles;
		mf.mesh.uv = uvs;
		mf.mesh.RecalculateNormals();

		DestroyImmediate(GetComponent<MeshCollider>());
		MeshCollider mc = gameObject.AddComponent<MeshCollider>();
		mc.sharedMesh = mf.mesh;
		mc.convex = true;
	}

	int Index2Dto1D(int x, int y)
	{
		return x+y*width;
	}
}
