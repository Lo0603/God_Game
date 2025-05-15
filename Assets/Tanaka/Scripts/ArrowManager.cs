using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ArrowManager : MonoBehaviour
{
	public UnityAction action;


	private void Start()
	{
		Button button = GetComponent<Button>();
		if (button != null)
		{
			button.onClick.AddListener(action);
		}
	}
}
