namespace UIWidgets
{
	/// <summary>
	/// Instance ID.
	/// </summary>
	public struct InstanceID
	{
		private readonly int id;

		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceID"/> struct.
		/// </summary>
		/// <param name="obj">Object.</param>
		public InstanceID(UnityEngine.Object obj)
		{
			id = obj.GetInstanceID();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceID"/> struct.
		/// </summary>
		/// <param name="id">ID.</param>
		private InstanceID(int id)
		{
			this.id = id;
		}

		/// <summary>
		/// InstanceID can be implicitly converted to int.
		/// </summary>
		/// <param name="id">ID.</param>
		[System.Obsolete("int replaced with InstanceID.")]
		public static implicit operator int(InstanceID id)
		{
			return id.id;
		}

		/// <summary>
		/// InstanceID can be implicitly converted to int.
		/// </summary>
		/// <param name="id">ID.</param>
		[System.Obsolete("int replaced with InstanceID.")]
		public static implicit operator InstanceID(int id)
		{
			return new InstanceID(id);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is InstanceID)
			{
				return Equals((InstanceID)obj);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(InstanceID other)
		{
			return id == other.id;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return id;
		}

		/// <summary>
		/// Compare specified ids.
		/// </summary>
		/// <param name="a">First id.</param>
		/// <param name="b">Second id.</param>
		/// <returns>true if the ids are equal; otherwise, false.</returns>
		public static bool operator ==(InstanceID a, InstanceID b)
		{
			return a.id == b.id;
		}

		/// <summary>
		/// Compare specified ids.
		/// </summary>
		/// <param name="a">First id.</param>
		/// <param name="b">Second id.</param>
		/// <returns>true if the ids not equal; otherwise, false.</returns>
		public static bool operator !=(InstanceID a, InstanceID b)
		{
			return a.id != b.id;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return id.ToString();
		}
	}
}