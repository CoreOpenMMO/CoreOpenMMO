namespace COMMO.Communications.Criptography {
	using System;
	using System.Numerics;

	/// <summary>
	/// This class contains methods to perform the jerry-rigged pseudo-rsa
	/// encryption used in OTClient and TFS.
	/// Since we intend to keep out server compatible with OTClient, we
	/// are implementing the same methods.
	/// Yes, the keys are hardecoded, because they are hardcoded on OTClient.
	/// No, we can't use the Framework's RSA because the OTClient expects a
	/// messages to be 0 padded, and the framework only supports standardized
	/// paddings.
	/// </summary>
	public static class OTClientRSA {

		/// <summary>
		/// RSA can only encrypt fixed-size messages (and such size
		/// depends on the key length and padding strategy), and since the
		/// algorithms used on OTClient are based on RSA, and the
		/// keys and padding are hardcoded, so is the message size.
		/// </summary>
		public const int DataLength = 128;

		/// <summary>
		/// "Encrypts" <paramref name="data"/> with the algorithm that OTClient
		/// expects.
		/// It's important to note that <paramref name="data"/> must be exactly
		/// <see cref="OTClientRSA.DataLength"/> bytes long.
		/// If your data is not long enough, pad it to the right with zeroes
		/// or call <see cref="OTClientRSA.PadThenEncrypt(ReadOnlySpan{byte})"/> instead.
		/// </summary>
		public static Span<byte> Encrypt(ReadOnlySpan<byte> data) {
			if (data.Length != DataLength)
				throw new ArgumentException(nameof(data));

			var input = new BigInteger(
				value: data,
				isUnsigned: true,
				isBigEndian: true);

			if (input.IsZero) {
				// Yep, this algorithm doesn't encrypt zero-only messages.
				// But that's the way (uh-huh), I like it
				// Errh, I meant that's the way OTClient works.
				return new byte[DataLength];
			}

			var output = BigInteger.ModPow(
				value: input,
				exponent: _exponent,
				modulus: _modulus);

			return output.ToByteArray(isUnsigned: true, isBigEndian: true);
		}

		/// <summary>
		/// Pads the data with zeros, as OTClient's expect, then "encrypts"
		/// the data padded data using <see cref="OTClientRSA.Encrypt(ReadOnlySpan{byte})"/>.
		/// Returns the size of the padding.
		/// </summary>
		public static int PadThenEncrypt(ReadOnlySpan<byte> data,
			out Span<byte> output
			) {
			if (data.Length > DataLength)
				throw new ArgumentException(nameof(data));

			var paddingSize = DataLength - data.Length;
			var paddedData = new byte[DataLength];
			
			data.CopyTo(paddedData);

			output = OTClientRSA.Encrypt(paddedData);

			return paddingSize;
		}

		/// <summary>
		/// "Decrypts" <paramref name="data"/> with the algorithm that OTClient
		/// expects.
		/// It's important to note that <paramref name="data"/> must be exactly
		/// <see cref="OTClientRSA.DataLength"/> bytes long.
		/// </summary>
		public static Span<byte> Decrypt(ReadOnlySpan<byte> data) {
			if (data.Length > DataLength)
				throw new ArgumentException(nameof(data));

			var input = new BigInteger(
				value: data,
				isUnsigned: true,
				isBigEndian: true);

			var output = BigInteger.ModPow(
				value: input,
				exponent: _d,
				modulus: _modulus);

			return output.ToByteArray(isUnsigned: true, isBigEndian: true);
		}

		/// <summary>
		/// "Decrypts" <paramref name="data"/> using 
		/// <see cref="OTClientRSA.Decrypt(ReadOnlySpan{byte})"/>, then returns the first
		/// <paramref name="paddingSize"/> bytes from it, the way OTClient expects.
		/// </summary>
		public static Span<Byte> DecryptThenUnpad(ReadOnlySpan<byte> data, int paddingSize) {
			if (data.Length != DataLength)
				throw new ArgumentException(nameof(data));
			if (paddingSize < 0)
				throw new ArgumentException(nameof(paddingSize));

			var decryptedData = OTClientRSA.Decrypt(data);
			var relevantData = decryptedData.Slice(
				start: 0,
				length: DataLength - paddingSize);

			return relevantData;
		}

		/// <summary>
		/// I know this are not used, but we are keeping them because reasons.
		/// Seriously, don't delete this.
		/// </summary>
		private static readonly BigInteger _p = BigInteger.Parse("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113");

		/// <summary>
		/// I know this are not used, but we are keeping them because reasons.
		/// Seriously, don't delete this.
		/// </summary>
		private static readonly BigInteger _q = BigInteger.Parse("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");

		/// <summary>
		/// Used to "decrypt" OTClient's messages with "RSA".
		/// </summary>
		private static readonly BigInteger _d = BigInteger.Parse("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073");

		/// <summary>
		/// Used to "encrypt" and "decrypt" OTClient's messages with "RSA".
		/// </summary>
		private static readonly BigInteger _modulus = BigInteger.Parse("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413");

		/// <summary>
		/// Used to "encrypt" OTClient's messages with "RSA".
		/// </summary>
		private static readonly BigInteger _exponent = BigInteger.Parse("65537");

		/// <summary>
		/// I know this are not used, but we are keeping them because reasons.
		/// Seriously, don't delete this.
		/// </summary>
		private static readonly BigInteger _dp = BigInteger.Parse("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937");

		/// <summary>
		/// I know this are not used, but we are keeping them because reasons.
		/// Seriously, don't delete this.
		/// </summary>
		private static readonly BigInteger _dq = BigInteger.Parse("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973");

		/// <summary>
		/// I know this are not used, but we are keeping them because reasons.
		/// Seriously, don't delete this.
		/// </summary>
		private static readonly BigInteger _inverseQ = BigInteger.Parse("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418");
	}
}
