using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapView : MonoBehaviour {
	public Transform playerPos;
	public Image playerSprite;
	public Image mapImage;
	public Image enemy;

	float width = 256;
	float height = 256;

	float ratio = 1.32f;

	Vector2 pos;

	private const int mkAllyTextureSize = 2;

	public Color friendlyColor;
	public Color enemyColor;
	public Camera topDownCamera;
	public Canvas canvas;

	private Image mPanelImage;
	private float mWorldOriginX;
	private float mWorldOriginZ;
	private float mScreenWidth;
	private float mScreenHeight;
	private float mMapImageWidth;
	private float mMapImageHeight;
	private Texture2D mFriendlyFireTeamMemberTexture;
	private Texture2D mEnemyFireTeamMemberTexture;
	private Dictionary<int, Image[]> mImageArrays;
	private TeamList mTeamList;

	// Use this for initialization
	void Start () {
		// Initialize map view size and position variables.
		Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>() as Terrain;
		Vector3 cameraPositionOnScreen= topDownCamera.WorldToScreenPoint(topDownCamera.transform.position); 
		GetWorldWidthAndHeight ();
		mMapImageWidth = Screen.width;
		mMapImageHeight =  Screen.height;
		mWorldOriginX = terrain.terrainData.heightmapWidth / 2;
		mWorldOriginZ = terrain.terrainData.heightmapHeight / 2;

		InitializeTextures ();

		// Initialize the dictionary of Image arryas as well as the team list.
		mImageArrays = new Dictionary<int, Image[]>(); // Used to store created image game objects.
		mTeamList = GameObject.Find("TeamList").GetComponent<TeamList>() as TeamList;
	}
	// TODO
	// For each Unit, spawn a sprite icon at it's relative position.

	// Update is called once per frame
	void Update () {
		if (!GameController.Instance.topDownView)
		{
			canvas.GetComponent<CanvasGroup>().alpha = 0;
			canvas.GetComponent<CanvasGroup>().interactable = false;
		}
		else
		{
			canvas.GetComponent<CanvasGroup>().alpha = 1;
			canvas.GetComponent<CanvasGroup>().interactable = true;
			if (playerPos != null && mapImage != null)
			{
				playerSprite.transform.localPosition = ConvertVector3To2DMapCoordinates(playerPos.position);
			}
			DrawFireTeamMembersOnMap ();
		}
	}

	private void DrawFireTeamMembersOnMap()
	{
		// Draw the positions of the friendly fire teams on the minimap.
		List<FireTeam> friendFireTeams = mTeamList.GetTeamsWithGivenAlignment(FireTeam.Side.Friend);
		DrawMapCoordinatesForFireTeamList (friendFireTeams, mFriendlyFireTeamMemberTexture);

		// Draw the positions of the enemy fire teams on the minimap.
		List<FireTeam> EnemyFireTeams = mTeamList.GetTeamsWithGivenAlignment(FireTeam.Side.Enemy);
		DrawMapCoordinatesForFireTeamList (EnemyFireTeams,  mEnemyFireTeamMemberTexture);
	}

	private void DrawMapCoordinatesForFireTeamList(List<FireTeam> fireTeamList, Texture2D texture)
	{
		foreach (FireTeam fireTeam in fireTeamList) {
			int key = fireTeam.teamNumber; // Use the fire team number as the key.

			// Get the image array in the dictionary of images. If there is no image array
			// for the given key, create one.
			Image[] images = GetImagesWithKey (key);
			if (images == null) {
				images = new Image[FireTeam.kMaxFireTeamMembers];
				mImageArrays.Add (key, images);
			}
			// Only draw the map positions of fire teams with at least one member in them.
			if (fireTeam.MemberCount > 0) {
				// Draw the minimap images for the fire team members in the fire team.
				DrawRemainingFireTeamMembers (images, fireTeam, texture);
				// Remove and destroy the images of fire team members that have been destroyed.
				DetroyImagesInArrayStartingFromIndex(images, fireTeam.MemberCount);
			} else {
				// If the fire team has been destroyed, destroy its image and remove it 
				// from the images dictionary.
				if (images != null) {
					DetroyImagesInArrayStartingFromIndex (images, 0);
					mImageArrays.Remove (key);
				}
			}
		}
	}

	private Image[] GetImagesWithKey(int key)
	{
		Image[] images = null;
		mImageArrays.TryGetValue (key, out images);
		return images;
	}

	private void DrawRemainingFireTeamMembers(Image[] images, FireTeam fireTeam, Texture2D texture)
	{
		for (int i = 0; i < fireTeam.MemberCount; ++i) {
			// Covert 3D coordinates of the fire team ally to minimap coordinates.
			Vector3 allyPosition = fireTeam.GetAllyAtSlotPosition(i).Position;
			Vector2 mapCoordinates = ConvertVector3To2DMapCoordinates (allyPosition);

			Image image = images [i];
			if (image == null) {
				image = CreateImage (texture, mkAllyTextureSize);
				images [i] = image;
			}

			// Move the image object to the new map coordinate position.
			image.transform.localPosition = mapCoordinates;
		}
	}

	private Image CreateImage(Texture2D texture, int textureSize)
	{	
		// Initialize an image for the fire team member.
		// Create the new sprite image object
		GameObject imageObject = new GameObject();
		Image image = imageObject.AddComponent<Image>();
		image.rectTransform.sizeDelta = new Vector2 (textureSize, textureSize);
		Rect rect = new Rect(0, 0, textureSize, textureSize);
		Sprite sprite =	Sprite.Create(texture, rect, new Vector2(0, 0));
		image.sprite = sprite;

		// Add the image object to the transform of this script.
		image.transform.SetParent(transform);

		return image;
	}

	private Vector2 ConvertVector3To2DMapCoordinates(Vector3 vector3ToConvert)
	{
		// Convert from world coordinates to minimap coordinates.
		float newXCoord = Mathf.Floor((vector3ToConvert.x - mWorldOriginX) * mMapImageWidth / mScreenWidth);
		float newYCoord = Mathf.Floor((vector3ToConvert.z - mWorldOriginZ) * mMapImageHeight / mScreenHeight);
		return new Vector2 (newXCoord, newYCoord);
	}

	private void DetroyImagesInArrayStartingFromIndex(Image[] images, int indexToDeleteFrom)
	{

		for (int i = images.Length - 1; i >= indexToDeleteFrom; --i) {
			Image image = images [i];
			//print (i);
			// print (image);
			if (image != null) {
				Destroy (image.gameObject);
				images [i] = null;
			}
		}
	}

	private void GetWorldWidthAndHeight()
	{
		// Get the corners of the world in game coordinates.
		// Subtracting the game coordinates of the corners 
		// gives the size of the screen in game cooridinaes.
		Vector3 bottomLeftCorner = topDownCamera.ViewportToWorldPoint (new Vector3 (0, 0, 
			topDownCamera.nearClipPlane));
		Vector3 bottomRightCorner = topDownCamera.ViewportToWorldPoint (new Vector3 (1, 0, 
			topDownCamera.nearClipPlane));
		Vector3 topLeftCorner = topDownCamera.ViewportToWorldPoint (new Vector3 (0, 1, 
			topDownCamera.nearClipPlane));

		// Calculate the screen width and height in game coordinates using the 
		// game coordinate distance between the screen corners.
		mScreenWidth = bottomRightCorner.x - bottomLeftCorner.x;
		mScreenHeight = topLeftCorner.z - bottomLeftCorner.z;
	}

	private void InitializeTextures()
	{
		// Initialize reusable textures.
		mFriendlyFireTeamMemberTexture = new Texture2D (mkAllyTextureSize, mkAllyTextureSize);
		mEnemyFireTeamMemberTexture = new Texture2D (mkAllyTextureSize, mkAllyTextureSize);
		for (int x = 0; x < mFriendlyFireTeamMemberTexture.width; ++x) {
			for (int y = 0; y < mFriendlyFireTeamMemberTexture.height; ++y) {
				mFriendlyFireTeamMemberTexture.SetPixel (x, y, friendlyColor);
				mEnemyFireTeamMemberTexture.SetPixel (x, y, enemyColor);
			}
		}

		// Apply texture changes
		mFriendlyFireTeamMemberTexture.Apply (); 
		mEnemyFireTeamMemberTexture.Apply (); 
	}
}
