using System;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	public Color friendlyColor;
	public Color enemyColor;
	public RectTransform minimapPanel;
	public Texture2D texture;

	// public Texture2D texture;
	private int mPanelOriginX;
	private int mPanelOriginY;
	private int mPanelHalfWidth;
	private int mPanelHalfHeight;

    public Canvas mCanvas;

    private Sprite mSprite;

	public void Start()
	{
		// Initialize panel size and position variables.
		mPanelHalfWidth = (int)minimapPanel.rect.width / 2;
		mPanelHalfHeight = (int)minimapPanel.rect.height / 2;
		mPanelOriginX = (int)minimapPanel.rect.position.x;
		mPanelOriginY = (int)minimapPanel.rect.position.y;
		print (mPanelHalfWidth);
		print (mPanelHalfHeight);
		print (mPanelOriginX);
		print (mPanelOriginY);

        mCanvas = transform.parent.GetComponent<Canvas>();
        GameObject mImage = new GameObject();
        mImage.AddComponent<Image>();
        Rect rect = new Rect(0, 0, 512, 512);
        mSprite = Sprite.Create(texture, rect, new Vector2(0, 0));
        mImage.GetComponent<Image>().sprite = mSprite;

        mImage.transform.parent = transform;
        mImage.transform.localPosition = Vector3.zero;
        //rect = mImage.GetComponent<Rect>();

    }
}

