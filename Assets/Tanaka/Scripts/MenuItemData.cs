using System;
using UnityEngine.UI;

[Serializable]
public class MenuItemData
{
	public string label;
	public Action onClick;
	//public SpriteState state;

	public MenuItemData(string label, Action onClick/*, SpriteState state*/)
	{
		this.label = label;
		this.onClick = onClick;
		//this.state = state;
	}
}