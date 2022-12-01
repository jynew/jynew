namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Tab button.
	/// </summary>
	public abstract class TabButtonBase : Button
	{
		/// <summary>
		/// Select event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnSelectEvent = new UnityEvent();

		/// <summary>
		/// Select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);

			OnSelectEvent.Invoke();
		}
	}
}