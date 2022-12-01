namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// Instances collection.
		/// </summary>
		/// <typeparam name="TItemView">Item type.</typeparam>
		public class InstancesCollection<TItemView>
			where TItemView : ListViewItem
		{
			/// <summary>
			/// Instances.
			/// </summary>
			public List<TItemView> AllInstances
			{
				get;
				protected set;
			}

			/// <summary>
			/// Owners instances.
			/// </summary>
			protected Dictionary<InstanceID, List<TItemView>> OwnersInstances;

			/// <summary>
			/// Count.
			/// </summary>
			public int Count
			{
				get
				{
					return AllInstances.Count;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="InstancesCollection{TItemView}"/> class.
			/// </summary>
			/// <param name="allInstances">All instances.</param>
			public InstancesCollection(List<TItemView> allInstances)
			{
				AllInstances = allInstances;

				OwnersInstances = new Dictionary<InstanceID, List<TItemView>>();

				OwnerChanged();
			}

			/// <summary>
			/// Process owner change.
			/// </summary>
			public void OwnerChanged()
			{
				OwnersInstances.Clear();

				foreach (var instance in AllInstances)
				{
					List<TItemView> instances;
					var id = new InstanceID(instance.Owner);
					if (OwnersInstances.TryGetValue(id, out instances))
					{
						instances.Add(instance);
					}
					else
					{
						OwnersInstances.Add(id, new List<TItemView>() { instance });
					}
				}
			}

			/// <summary>
			/// Get instances belong to owner by ID.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <returns>Instances.</returns>
			public List<TItemView> Of(InstanceID ownerID)
			{
				List<TItemView> instances;
				if (!OwnersInstances.TryGetValue(ownerID, out instances))
				{
					instances = new List<TItemView>();
					OwnersInstances.Add(ownerID, instances);
				}

				return instances;
			}

			/// <summary>
			/// Add instance belong to owner.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="instance">Instance.</param>
			public void Add(InstanceID ownerID, TItemView instance)
			{
				AllInstances.Add(instance);
				Of(ownerID).Add(instance);
			}

			/// <summary>
			/// Remove instances belong to owner.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="completely">Remove owner completely.</param>
			public void Remove(InstanceID ownerID, bool completely = true)
			{
				var instances = Of(ownerID);
				foreach (var instance in instances)
				{
					AllInstances.Remove(instance);
				}

				instances.Clear();

				if (completely)
				{
					OwnersInstances.Remove(ownerID);
				}
			}

			/// <summary>
			/// Remove instance belong to owner by index.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="index">Index.</param>
			public void RemoveAt(InstanceID ownerID, int index)
			{
				var instances = Of(ownerID);
				AllInstances.Remove(instances[index]);
				instances.RemoveAt(index);
			}

			/// <summary>
			/// Clear.
			/// </summary>
			public void Clear()
			{
				AllInstances.Clear();
				OwnersInstances.Clear();
			}

			/// <summary>
			/// Copy data from source.
			/// </summary>
			/// <param name="source">Source.</param>
			public void CopyFrom(InstancesCollection<TItemView> source)
			{
				foreach (var instances in source.OwnersInstances)
				{
					Of(instances.Key).AddRange(instances.Value);
				}

				source.OwnersInstances.Clear();

				AllInstances.AddRange(source.AllInstances);
				source.AllInstances.Clear();
			}
		}
	}
}