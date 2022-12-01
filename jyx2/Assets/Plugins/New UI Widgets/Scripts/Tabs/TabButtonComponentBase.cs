namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Tab component.
	/// </summary>
	public class TabButtonComponentBase : MonoBehaviour, IUpgradeable, ISelectHandler
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Select event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnSelectEvent = new UnityEvent();

		/// <summary>
		/// Select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			OnSelectEvent.Invoke();
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void SetButtonData(Tab tab)
		{
			NameAdapter.text = Localization.GetTranslation(tab.Name);
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}