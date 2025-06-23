using System;
using UnityEngine.UI;

[Serializable]
public class MenuItemData
{
	public string label;
	public Action onClick;

	public MenuItemData(string label, Action onClick)
	{
		this.label = label;
		this.onClick = onClick;
	}
}