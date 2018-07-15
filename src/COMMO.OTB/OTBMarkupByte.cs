namespace COMMO.OTB {

	/// <summary>
	/// Stores the values of markup bytes used in the .otb format.
	/// </summary>
	public enum OTBMarkupByte : byte {
		Escape = 0xFD,
		Start = 0xFE,
		End = 0xFF
	}
}
