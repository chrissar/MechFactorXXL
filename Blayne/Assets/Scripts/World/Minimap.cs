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

	
	}
}

