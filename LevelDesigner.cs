using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour {
	public int designNumber = 0;
	public float size = 1;
	public float gridStep = 10;
	public float maxNoise = 1;
	public float A = 100;
	public float B = 100;
	public int maxSatellitesCount = 10;
	public GameObject block;
	public Material crystalMaterial;
	public GameObject[] crystalPoints;
	Light mainSun;

	Transform crystalSun;
	GameObject[] satellites;
	Vector3 mainCrystalRotation = Vector3.zero;
	Vector3 satellitesRotation = Vector3.zero;

	void Awake() {
		switch (designNumber) {
		case 0: //       STANDART SYSTEM
			GameObject mainCrystal = new GameObject("main crystal");
			mainCrystal.layer = 8; //Space Objects layer
			MeshFilter mf = mainCrystal.AddComponent<MeshFilter>();
			MeshRenderer mr = mainCrystal.AddComponent<MeshRenderer>();
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mr.receiveShadows = false;
			Color mainColor = Random.ColorHSV();
			float h, s,v; 
			Color.RGBToHSV(mainColor, out h, out s, out v);
			Color sunColor = Color.HSVToRGB(h,s,1);
			crystalMaterial.color = mainColor;
			crystalMaterial.SetColor("_EmissionColor", sunColor);
			crystalMaterial.SetFloat("_Emission", 1f);

			mf.mesh = CreateCrystal(size, gridStep, maxNoise);
			mr.material = crystalMaterial;
			mainCrystal.transform.position = Vector3.forward * 250;
			mainSun = mainCrystal.AddComponent<Light>();
			mainSun.type = LightType.Directional;
			mainSun.transform.forward = -mainCrystal.transform.position;
			mainSun.color = sunColor;

			crystalSun = mainCrystal.transform;
			mainCrystalRotation = Random.onUnitSphere / 4.0f;
			mainCrystal.transform.localScale = new Vector3(Random.value*0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f);

			satellites = new GameObject[(int)(Random.value * maxSatellitesCount)];
			Vector3 satSize = size * mainCrystal.transform.localScale / 100;
			for (int i =0; i <satellites.Length;i ++) {
				GameObject t = new GameObject("satellite"+i.ToString());
				mf = t.AddComponent<MeshFilter>();
				mf.mesh = CreateCrystal(size/10 * (0.7f * Random.value + 0.3f), gridStep/10, maxNoise/20);
				mr = t.AddComponent<MeshRenderer>();
				mr.material = crystalMaterial;
				mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				mr.receiveShadows = false;

				t.transform.position = Random.onUnitSphere * size * 1.2f;
				t.transform.localScale = new Vector3(t.transform.localScale.x, 1.5f * t.transform.localScale.y, t.transform.localScale.z);
				satellites[i] = t;
			}
			satellitesRotation = Random.onUnitSphere;
			break;
		case 1:
			if (crystalPoints.Length == 0) {print ("no crystal points, desu"); return;}
			foreach (GameObject g in crystalPoints) {
				CreateCrystalOnObject(4+Random.value*8, g);
			}
			break;
		}
			
	}

	void Update () {
		if (GameMaster.IsPaused() || designNumber != 0) return;
		crystalSun.Rotate(mainCrystalRotation * Time.deltaTime);
		if (satellites.Length!=0) {
			foreach (GameObject t in satellites) {
				t.transform.RotateAround(crystalSun.position, t.transform.TransformDirection(satellitesRotation), 0.25f);
			}
		}
	}

	Mesh CreateCrystal( float size ) {
		Mesh m = new Mesh();
		List<Vector3> verts = new List<Vector3>();
		float diagEdge = 1.414f * size / 2;
		verts.Add(new Vector3(0, size, 0));
		verts.Add(new Vector3(diagEdge, 0, diagEdge));
		verts.Add(new Vector3(-diagEdge, 0, diagEdge));
		verts.Add(new Vector3(-diagEdge, 0, -diagEdge));
		verts.Add(new Vector3(diagEdge, 0, -diagEdge));
		verts.Add(new Vector3(0, -size, 0));
		int[] tris = {0,2,1, 0,3,2, 0,4,3, 0,1,4, 4,1,5, 1,2,5, 2,3,5, 3,4,5};
		m.SetVertices(verts);
		m.triangles = tris;
		Vector2[] uvs = {new Vector2 (0.5f, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0,1), new Vector2(1,0), new Vector2(0.5f, 0)	};
		m.uv = uvs;
		return m;
	}

	Mesh CreateCrystal(float size, float step,  float maxGap ) {
		Mesh m = new Mesh();

		int levels = (int)(size / step );
		if (levels < 2) {
			return CreateCrystal(size);
		}
		int count = (int)(6 * (Mathf.Pow(2, levels-1) - 1) + 2 + 3 * Mathf.Pow(2, levels-1));
		float trisCount = 0;
		Vector3[] verts = new Vector3[count];
		Vector2[] uvs = new Vector2[count];
		int k = 0;
		verts[k++] = Vector3.up * size; // equal to verts[k] = x; k++;
		for (int i = 1; i<= levels; i++) {
			float height = size - i * step;
			float r = size * (1f - height / size);
			int vertsCount =(int) (3 * Mathf.Pow(2, i - 1));
			trisCount += vertsCount * 1.5f;
			float angle = 2* Mathf.PI / vertsCount;
			float t = 0;
			for (int j = 0; j < vertsCount; j++) {
				verts[k++] = new Vector3( r * Mathf.Cos( t ), height, r * Mathf.Sin( t) ) * (1 + Random.value * maxGap); 
				t += angle;
			}
		}
		for (int i = levels-1; i > 0; i--) {
			float height = size - i * step;
			float r = size * (1f - height / size);
			int vertsCount =(int) (3 * Mathf.Pow(2, i - 1));
			trisCount += vertsCount * 1.5f;
			float angle = 2* Mathf.PI / vertsCount;
			float t = 0;
			for (int j = 0; j < vertsCount; j++) {
				verts[k++] = new Vector3( r * Mathf.Cos( t ), -height, r * Mathf.Sin( t) ) * (1+Random.value * maxGap);
				t += angle;
			}
		}
		verts[k] = Vector3.down * size;
		m.vertices = verts;

		trisCount = trisCount*2 - 3;
		int[] tris = new int[(int)(6 * trisCount)];

		tris[0] = 0; tris[1] = 2; tris[2] = 1;
		tris[3] = 0; tris[4] = 3; tris[5] = 2;
		tris[6] = 0; tris[7] = 1; tris[8] = 3;

		int upIndex = 1, downIndex = 4, trisIndex = 9;
		for (int i=1; i < levels; i++) {
			int upCount = (int)(3 * Mathf.Pow(2, i-1)); // two rows of vertices
			int endIndex = upIndex + upCount;
			downIndex = endIndex ;
			int startDownIndex = downIndex, startUpIndex = upIndex;
			for (upIndex = upIndex; upIndex < endIndex -1; upIndex ++) {
				tris[trisIndex++] = upIndex; 
				tris[trisIndex++] = downIndex+1; 
				tris[trisIndex++] = downIndex; 

				tris[trisIndex++] = upIndex; 
				tris[trisIndex++] = upIndex+1; 
				tris[trisIndex++] = downIndex + 1;

				tris[trisIndex++] = upIndex+1;
				tris[trisIndex++] = downIndex + 2;
				tris[trisIndex++] = downIndex + 1;

				downIndex+=2;
			}

			tris[trisIndex++] = upIndex; 
			tris[trisIndex++] = downIndex+1; 
			tris[trisIndex++] = downIndex; 

			tris[trisIndex++] = upIndex;
			tris[trisIndex++] = startUpIndex;
			tris[trisIndex++] = downIndex + 1;

			tris[trisIndex++] = startUpIndex;
			tris[trisIndex++] = startDownIndex;
			tris[trisIndex++] = downIndex + 1;

			upIndex ++;
		}
		//bottom part

		upIndex = downIndex + 2;
		for (int i = levels-1; i > 0; i--) {
			int upCount = (int)(3 * Mathf.Pow(2, i-1)); 
			int endIndex = upIndex + upCount;
			downIndex = upIndex - 2*upCount ;
			int startDownIndex = downIndex, startUpIndex = upIndex;
			for (upIndex = upIndex; upIndex < endIndex -1; upIndex ++) {
				tris[trisIndex++] = upIndex ; 
				tris[trisIndex++] = downIndex; 
				tris[trisIndex++] = downIndex + 1; 

				tris[trisIndex++] = upIndex +1;
				tris[trisIndex++] = upIndex;
				tris[trisIndex++] = downIndex + 1;

				tris[trisIndex++] = upIndex +1;
				tris[trisIndex++] = downIndex + 1;
				tris[trisIndex++] = downIndex + 2;

				downIndex+=2;
			}

			tris[trisIndex++] = upIndex;
			tris[trisIndex++] = downIndex;
			tris[trisIndex++] = downIndex + 1;

			tris[trisIndex++] =downIndex + 1;
			tris[trisIndex++] = startUpIndex;
			tris[trisIndex++] = upIndex;

			tris[trisIndex++] = downIndex + 1;
			tris[trisIndex++] = startDownIndex;
			tris[trisIndex++] = startUpIndex;

			upIndex ++;
		}
		tris[trisIndex++] = upIndex; tris[trisIndex++] = upIndex -2; tris[trisIndex++] = upIndex - 1;
		tris[trisIndex++] = upIndex; tris[trisIndex++] = upIndex -3; tris[trisIndex++] = upIndex - 2;
		tris[trisIndex++] = upIndex; tris[trisIndex++] = upIndex - 1; tris[trisIndex++] = upIndex - 3;
		m.triangles = tris;


		return m;
	}


	public void CreateCrystalOnObject (float size, GameObject g) 
	{
		MeshRenderer mr = g.AddComponent<MeshRenderer>();
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;
		MeshFilter mf= g.AddComponent<MeshFilter>();
		mf.mesh = CreateCrystal(size, size/8f, size/20f);
		mr.material = crystalMaterial;
	}

	GameObject CreateRing() {
		float radiusA = A;
		float radiusB = B;
		float len= 4 * (Mathf.PI * radiusA * radiusB + (radiusA - radiusB) * (radiusA - radiusB)) / (radiusA + radiusB);
		float step = size / len;
		Vector3 startPos = Random.onUnitSphere * 1000;
		GameObject ring  = new GameObject("ring");
		ring.transform.position = startPos;
		ring.transform.rotation = Random.rotation;

		for (float i = 0; i <= 1; i+=step) {
			float t = i * 2 * Mathf.PI;
			Vector3 pos = new Vector3( radiusA * Mathf.Cos(t), 0, radiusB * Mathf. Sin(t));
			GameObject g = Instantiate(block, ring.transform.position, Random.rotation) as GameObject;
			g.transform.localScale = Vector3.one * size ; 
			g.transform.parent = ring.transform;
			g.transform.localPosition = pos;
		}

		return ring;
	}
}
