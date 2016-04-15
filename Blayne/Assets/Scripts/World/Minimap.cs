using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	private const int mkAllyTextureSize = 2;
	private const int mkCameraBorderSize = 16;

	public Color friendlyColor;
	public Color enemyColor;
	public Color cameraBorderColor;
	public RectTransform minimapPanel;
	public Camera playerPositionCamera;

	private float mWorldOriginX;
	private float mWorldOriginZ;
	private float mWorldWidth;
	private float mWorldHeight;
	private float mPanelWidth;
	private float mPanelHeight;
	private Texture2D mFriendlyFireTeamMemberTexture;
	private Texture2D mEnemyFireTeamMemberTexture;
	private Texture2D mCameraBorderTexture;
	private Dictionary<int, Image[]> mImageArrays;
	private Image mCameraImage;
	private TeamList mTeamList;
	private Canvas mCanvas;

	public void Start()
	{
		// Initialize panel size and position variables.
		GameObject worldGameObject = GameObject.Find("Water"); // "Water" sets the world bounds.
		mWorldWidth = worldGameObject.transform.localScale.x * 10; // Unit plane is 10x10.
		mWorldHeight = worldGameObject.transform.localScale.z * 10; // Unit plane is 10x10.
		mPanelWidth = minimapPanel.rect.width;
		mPanelHeight = minimapPanel.rect.height;
		mWorldOriginX = worldGameObject.transform.position.x;
		mWorldOriginZ = worldGameObject.transform.position.z;

		InitializeTextures ();
	
		// Initialize the dictionary of Image arryas as well as the team list.
		mImageArrays = new Dictionary<int, Image[]>(); // Used to store created image game objects.
		mTeamList = GameObject.Find("TeamList").GetComponent<TeamList>() as TeamList;

		// Get a reference to the minimap's canvas.
		mCanvas = transform.parent.GetComponent<Canvas>();
    }

	public void Update()
	{
		if (!GameController.Instance.topDownView) {
			mCanvas.enabled = true;
			// Draw the positions of the friendly fire teams on the minimap.
			List<FireTeam> friendFireTeams = mTeamList.GetTeamsWithGivenAlignment (FireTeam.Side.Friend);
			DrawMapCoordinatesForFireTeamList (friendFireTeams, mFriendlyFireTeamMemberTexture);

			// Draw the positions of the enemy fire teams on the minimap.
			List<FireTeam> EnemyFireTeams = mTeamList.GetTeamsWithGivenAlignment (FireTeam.Side.Enemy);
			DrawMapCoordinatesForFireTeamList (EnemyFireTeams, mEnemyFireTeamMemberTexture);

			// Move the camera border image to the current position of the camera.
			Vector2 cameraMapCoordinates = 
				ConvertVector3To2DMapCoordinates (playerPositionCamera.transform.position);
			mCameraImage.transform.localPosition = cameraMapCoordinates;
		} else {
			// Disable the minimap while in top down view mode.
			mCanvas.enabled = false;
		}
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
		image.transform.parent = transform;

		return image;
	}

	private Vector2 ConvertVector3To2DMapCoordinates(Vector3 vector3ToConvert)
	{
		// Convert from world coordinates to minimap coordinates.
		float newXCoord = Mathf.Floor((vector3ToConvert.x - mWorldOriginX) * mPanelWidth / mWorldWidth);
		float newYCoord = Mathf.Floor((vector3ToConvert.z - mWorldOriginZ) * mPanelHeight / mWorldHeight);
		return new Vector2 (newXCoord, newYCoord);
	}

	private void DetroyImagesInArrayStartingFromIndex(Image[] images, int indexToDeleteFrom)
	{
		
		for (int i = images.Length - 1; i >= indexToDeleteFrom; --i) {
			Image image = images [i];
			//print (i);
			// print (image);
			if (image != null) {
				// print ("destroy");
				Destroy (image.gameObject);
				images [i] = null;
			}
		}
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
		// Set borders of the camera texture.
		mCameraBorderTexture = new Texture2D (mkCameraBorderSize, mkCameraBorderSize);
		// Blank the camera border texture.
		for (int x = 0; x <  mCameraBorderTexture.width; ++x) {
			for (int y = 0; y < mCameraBorderTexture.height; ++y) {
				mCameraBorderTexture.SetPixel (x, y, new Color(0, 0, 0, 0));
			}
		}
		// Fill top and bottom sides.
		for (int x = 0; x < mCameraBorderTexture.width; ++x) {
			mCameraBorderTexture.SetPixel (x, 0, cameraBorderColor);
			mCameraBorderTexture.SetPixel (x, 1, cameraBorderColor);
			mCameraBorderTexture.SetPixel (x, mCameraBorderTexture.height - 2, cameraBorderColor);
			mCameraBorderTexture.SetPixel (x, mCameraBorderTexture.height -1, cameraBorderColor);
		}
		// Fill left and right sides.
		for (int y = 2; y < mCameraBorderTexture.height - 2; ++y) {
			mCameraBorderTexture.SetPixel (0, y, cameraBorderColor); 
			mCameraBorderTexture.SetPixel (1, y, cameraBorderColor);
			mCameraBorderTexture.SetPixel (mCameraBorderTexture.width - 2, y, cameraBorderColor);
			mCameraBorderTexture.SetPixel (mCameraBorderTexture.width - 1, y, cameraBorderColor);
		}
		// Apply texture changes
		mFriendlyFireTeamMemberTexture.Apply (); 
		mEnemyFireTeamMemberTexture.Apply (); 
		mCameraBorderTexture.Apply();

		// Create camera Image
		mCameraImage = CreateImage(mCameraBorderTexture, mkCameraBorderSize);
	}
}

