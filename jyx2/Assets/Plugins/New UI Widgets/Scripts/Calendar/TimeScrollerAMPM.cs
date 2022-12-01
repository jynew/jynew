namespace UIWidgets
{
	using System;

	/// <summary>
	/// TimeScroller with only two AMPM values.
	/// </summary>
	[Obsolete("Replaced with TimeScroller.SingleAMPM property.")]
	public class TimeScrollerAMPM : TimeScroller
	{
		/// <inheritdoc/>
		public override void Init()
		{
			base.Init();

			SingleAMPM = true;
		}
	}
}