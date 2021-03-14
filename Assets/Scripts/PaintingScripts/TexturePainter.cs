using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Collections;
using System;

public enum Painter_BrushMode{PAINT,DECAL};
public class TexturePainter : MonoBehaviour {

	public GameObject brushCursor,brushContainer; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera,canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
	public Sprite cursorPaint,cursorDecal; // Cursor for the differen functions 
	public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes

	double  coloredarea = 0.0f; // Coloread Area Percantage
	private Texture2D tex; // Colored Are Texture
	public TMPro.TextMeshProUGUI percText; // UI Colored Are Text 

	public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)

	Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
	float brushSize=1.0f; //The size of our brush
	Color brushColor; //The selected color
	int brushCounter=0,MAX_BRUSH_COUNT=1000; //To avoid having millions of brushes
	bool saving=false; //Flag to check if we are saving the texture

	
	void Update () {
		brushColor = ColorSelector.GetColor ();	//Updates our painted color with the selected color
		if (Input.GetMouseButton(0)) {
			DoAction();
		}
		UpdateBrushCursor ();
		double value = Math.Round(coloredarea, 2);
		percText.text = value + "%";
	}

	//The main action, instantiates a brush or decal entity at the clicked position on the UV map
	void DoAction(){	
		if (saving)
			return;
		Vector3 uvWorldPosition=Vector3.zero;		
		if(HitTestUVPosition(ref uvWorldPosition)){
			GameObject brushObj;
			if(mode==Painter_BrushMode.PAINT){

				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); //Paint a brush
				brushObj.GetComponent<SpriteRenderer>().color=brushColor; //Set the brush color
			}
			else{
				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity")); //Paint a decal
			}
			brushColor.a=brushSize*2.0f; // Brushes have alpha to have a merging effect when painted over.
			brushObj.transform.parent=brushContainer.transform; //Add the brush to our container to be wiped later
			brushObj.transform.localPosition=uvWorldPosition; //The position of the brush (in the UVMap)
			brushObj.transform.localScale=Vector3.one*brushSize;//The size of the brush
		}
		brushCounter++; //Add to the max brushes
		if (brushCounter >= MAX_BRUSH_COUNT) { //If we reach the max brushes available, flatten the texture and clear the brushes
			brushCursor.SetActive (false);
			saving=true;
			Invoke("SaveTexture",0.1f);
			
		}
	}
	//To update at realtime the painting cursor on the mesh
	void UpdateBrushCursor(){
		Vector3 uvWorldPosition=Vector3.zero;
		if (HitTestUVPosition (ref uvWorldPosition) && !saving) {
			brushCursor.SetActive(true);
			brushCursor.transform.position =uvWorldPosition+brushContainer.transform.position;									
		} else {
			brushCursor.SetActive(false);
		}		
	}

	//Returns the position on the texuremap according to a hit in the mesh collider
	bool HitTestUVPosition(ref Vector3 uvWorldPosition){
		RaycastHit hit;
		Vector3 cursorPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f);
		Ray cursorRay=sceneCamera.ScreenPointToRay (cursorPos);
		if (Physics.Raycast(cursorRay,out hit,200)){
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
				return false;			
			Vector2 pixelUV  = new Vector2(hit.textureCoord.x,hit.textureCoord.y);
			uvWorldPosition.x=pixelUV.x-canvasCam.orthographicSize;//To center the UV on X
			uvWorldPosition.y=pixelUV.y-canvasCam.orthographicSize;//To center the UV on Y
			uvWorldPosition.z=0.0f;
			return true;
		}
		else{		
			return false;
		}
		
	}
	//Sets the base material with a our canvas texture, then removes all our brushes
	public void SaveTexture(){		
		brushCounter=0;
		System.DateTime date = System.DateTime.Now;
		RenderTexture.active = canvasTexture;
		tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);		
		tex.ReadPixels (new Rect (0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
		tex.Apply ();
		ColorChecker();
		RenderTexture.active = null;
		baseMaterial.mainTexture =tex;	//Put the painted texture as the base
		foreach (Transform child in brushContainer.transform) { //Clear brushes
			Destroy(child.gameObject);
		}

		StartCoroutine ("SaveTextureToFile",tex); //Do you want to save the texture? This is your method!
		Invoke ("ShowCursor", 0.1f);
	}


	public void ColorChecker()
	{
		int count = 0;
		for (int x = 0; x < tex.width; x++)
			for (int y = 0; y < tex.height; y++)
				if (!tex.GetPixel(x, y).Equals(new Color(0, 0, 0, 1)) && !tex.GetPixel(x, y).Equals(new Color(1, 1, 1, 1)))
					count++;
		double area = tex.width * tex.height;
		coloredarea = 100 * (count / area);
		print(coloredarea);
	}

	Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}

	private int CompareTextures(Texture2D texture1, Texture2D texture2)
	{
		int correctPixels = 0;
		var tex1 = texture1.GetPixels();
		var tex2 = texture2.GetPixels();

		if (tex1.Length != tex2.Length)
		{
			return 0;
		}
		else
		{
			for (int i = 0; i < tex1.Length; i++)
			{
				if (tex1[i] != tex2[i])
				{
					correctPixels++;
				}
			}

			int percentage = (100 * correctPixels) / tex1.Length;
			return percentage;
		}

	}

	IEnumerator SaveTextureToFile(Texture2D savedTexture)
	{
		string fullPath = System.IO.Directory.GetCurrentDirectory();
		System.DateTime date = System.DateTime.Now;
		string fileName = "CanvasTexture.png";
		if (!System.IO.Directory.Exists(fullPath))
			System.IO.Directory.CreateDirectory(fullPath);
		var bytes = savedTexture.EncodeToPNG();
		System.IO.File.WriteAllBytes(fullPath + fileName, bytes);
		yield return null;
	}

	//Show again the user cursor (To avoid saving it to the texture)
	void ShowCursor(){	
		saving = false;
	}

	////////////////// PUBLIC METHODS //////////////////

	public void SetBrushMode(Painter_BrushMode brushMode){ //Sets if we are painting or placing decals
		mode = brushMode;
		brushCursor.GetComponent<SpriteRenderer> ().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
	}
	public void SetBrushSize(float newBrushSize){ //Sets the size of the cursor brush or decal
		brushSize = newBrushSize;
		brushCursor.transform.localScale = Vector3.one * brushSize;
	}

}
