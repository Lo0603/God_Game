using System;
using UnityEngine.UI;

[Serializable]
public class MenuItemData
{
	public string label;
	public Action onClick;
	//public SpriteState state;
	public Image icon;

	public MenuItemData(string label, Action onClick/*, SpriteState state*/, Image icon)
	{
		this.label = label;
		this.onClick = onClick;
		this.icon = icon;
		//this.state = state;
	}
}