using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	private const int mkTextureSize = 2;

	public Color friendlyColor;
	public Color enemyColor;
	public RectTransform minimapPanel;

	private float mWorldOriginX;
	private float mWorldOriginZ;
	private float mWorldWidth;
	private float mWorldHeight;
	private float mPanelWidth;
	private float mPanelHeight;

    private Canvas mCanvas;
    private Sprite mSprite;
	private Texture2D mFriendlyFireTeamTexture;
	private Texture2D mFriendlyFireTeamMemberTexture;
	private Texture2D mEnemyFireTeamTexture;
	private Texture2D mEnemyFireTeamMemberTexture;
	private Dictionary<int, Image[]> mImageArrays;
	private TeamList mTeamList;

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

		// Initialize reusable textures.
		mFriendlyFireTeamTexture = new Texture2D (mkTextureSize, mkTextureSize);
		mEnemyFireTeamTexture = new Texture2D (mkTextureSize, mkTextureSize);
		for (int x = 0; x < mkTextureSize; ++x) {
			for (int y = 0; y < mkTextureSize; ++y) {
				mFriendlyFireTeamTexture.SetPixel (x, y, friendlyColor);
				mEnemyFireTeamTexture.SetPixel (x, y, enemyColor);
			}
		}
		mFriendlyFireTeamTexture.Apply (); // Apply texture changes
		mEnemyFireTeamTexture.Apply (); // Apply texture changes

		mImageArrays = new Dictionary<int, Image[]>(); // Used to store created image game objects.
		mTeamList = GameObject.Find("TeamList").GetComponent<TeamList>() as TeamList;
    }

	public void Update()
	{
		// Draw the positions of the friendly fire teams on the minimap.
		List<FireTeam> friendFireTeams = mTeamList.GetTeamsWithGivenAlignment(FireTeam.Side.Friend);
		DrawMapCoordinatesForFireTeamList (friendFireTeams, mFriendlyFireTeamTexture);

		// Draw the positions of the enemy fire teams on the minimap.
		List<FireTeam> EnemyFireTeams = mTeamList.GetTeamsWithGivenAlignment(FireTeam.Side.Enemy);
		DrawMapCoordinatesForFireTeamList (EnemyFireTeams,  mEnemyFireTeamTexture);
	}

	private void DrawMapCoordinatesForFireTeamList(List<FireTeam> fireTeamList, Texture2D texture)
	{
		foreach (FireTeam fireTeam in fireTeamList) {
			int key = fireTeam.teamNumber; // Use the fire team number as the key.

			// Get the image array in the dictionary of images. If there is no image array
			// for the given key, create one.
			Image[] images = GetImagesWithKey (key);
			print(images);
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
				print ("creating: " + i);
				image = CreateFireTeamMemberImage (texture);
				images [i] = image;
			}

			// Move the image object to the new map coordinate position.
		 	image.transform.localPosition = mapCoordinates;
		}
	}

	private Image CreateFireTeamMemberImage(Texture2D texture)
	{	
		// Initialize an image for the fire team member.
		// Create the new sprite image object
		GameObject imageObject = new GameObject();
		Image image = imageObject.AddComponent<Image>();
		image.rectTransform.sizeDelta = new Vector2 (mkTextureSize, mkTextureSize);
		Rect rect = new Rect(0, 0, mkTextureSize, mkTextureSize);
		Sprite sprite =	Sprite.Create(texture, rect, new Vector2(0, 0));
		image.sprite = sprite;

		// Add the image object to the transform of this script.
		image.transform.parent = transform;

		return image;
	}

	private Vector2 ConvertVector3To2DMapCoordinates(Vector3 vector3ToConvert)
	{
		// Convert from world coordinates to minimap coordinates.
		float newXCoord = (vector3ToConvert.x - mWorldOriginX) * mPanelWidth / mWorldWidth;
		float newYCoord = (vector3ToConvert.z - mWorldOriginZ) * mPanelHeight / mWorldHeight;
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
}

