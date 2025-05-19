using UnityEngine;

public abstract class BaseMenuManager : MonoBehaviour
{
	[SerializeField] protected Transform buttonContainer;
	[SerializeField] protected ButtonItem buttonPrefab;

	protected void CreateButton(string label, UnityEngine.Events.UnityAction action)
	{
		var button = Instantiate(buttonPrefab, buttonContainer);
		button.Setup(label, action);
	}

	protected abstract void SetupMenu();

	private void Start()
	{
		SetupMenu();
	}
}
