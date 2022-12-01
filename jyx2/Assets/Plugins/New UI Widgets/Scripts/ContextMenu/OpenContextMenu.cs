namespace UIWidgets.Menu
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Opens context menu by click on non-UI gameobject.
	/// Requires PhysicsRaycaster on main camera for the 3D objects.
	/// Requires PhysicsRaycaster2D on main camera for the 2D objects.
	/// </summary>
	public class OpenContextMenu : MonoBehaviour, IPointerClickHandler
	{
		/// <summary>
		/// Context menu to open.
		/// </summary>
		[SerializeField]
		public ContextMenu Menu;

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Menu.Open(eventData);
			}
		}
	}
}