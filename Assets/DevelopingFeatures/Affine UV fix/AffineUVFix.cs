using UnityEngine;

public class AffineUVFix : MonoBehaviour {
	[SerializeField] Vector3[] vertices;

	void Start () {
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4];
		mesh.triangles = new int[] {0,1,2, 0,2,3};		
		GetComponent<MeshFilter>().mesh = mesh;
	}

	void Update () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = vertices;
			
		//NEW TEST
		var shiftedPositions = new Vector3[] {
			Vector3.zero,
			new Vector3(0, 1, 0),
			new Vector3(vertices[2].x - vertices[1].x, 1, 0),
			new Vector3(vertices[3].x - vertices[0].x, 0, 0)
		};
		shiftedPositions[0].z = shiftedPositions[3].z = shiftedPositions[3].x;
		shiftedPositions[1].z = shiftedPositions[2].z = shiftedPositions[2].x;
		mesh.SetUVs(0, shiftedPositions);
	}
}