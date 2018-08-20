namespace COMMO.Communications.Criptography {
	using System;
	using System.Numerics;
	using System.Security.Cryptography;
	using SystemCryptography = System.Security.Cryptography;

	public static class RSA {

		public static byte[] Encrypt(byte[] message) {
			using (var csp = SystemCryptography.RSA.Create()) {
				var parameters = new RSAParameters() {
					Exponent = ConvertStringRepresentationToBase64BigEndian(OTClientExponent),
					Modulus = ConvertStringRepresentationToBase64BigEndian(OTClientModulus)
				};

				csp.ImportParameters(parameters);

				var encrypted = csp.Encrypt(
					data: message,
					padding: RSAEncryptionPadding.OaepSHA256);

				return encrypted;
			}
		}

		public static byte[] Decrypt(byte[] message) {
			using (var csp = SystemCryptography.RSA.Create()) {
				var parameters = new RSAParameters() {
					Exponent = ConvertStringRepresentationToBase64BigEndian(OTClientExponent),
					Modulus = ConvertStringRepresentationToBase64BigEndian(OTClientModulus),
					P = ConvertStringRepresentationToBase64BigEndian(OTClientP),
					Q = ConvertStringRepresentationToBase64BigEndian(OTClientQ),
					D = ConvertStringRepresentationToBase64BigEndian(OTClientD),
					DP = ConvertStringRepresentationToBase64BigEndian(OTClientDP),
					DQ = ConvertStringRepresentationToBase64BigEndian(OTClientDQ),
					InverseQ = ConvertStringRepresentationToBase64BigEndian(OTClientInverseq),
				};

				csp.ImportParameters(parameters);

				var encrypted = csp.Decrypt(
					data: message,
					padding: RSAEncryptionPadding.OaepSHA256);

				return encrypted;
			}
		}

		private static byte[] ConvertStringRepresentationToBase64BigEndian(string parameter) {
			var biggie = BigInteger.Parse(parameter);
			var valBytes = biggie.ToByteArray();
			int len = valBytes.Length;
			while (valBytes[len - 1] == 0) {
				--len;
				if (len == 0) {
					break;
				}
			}
			Array.Resize(ref valBytes, len);
			Array.Reverse(valBytes);
			return valBytes;
		}

		private const string OTClientP = "14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113";

		private const string OTClientQ = "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101";

		private const string OTClientD = "46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073";

		private const string OTClientModulus = "109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413";

		private const string OTClientExponent = "65537";

		private const string OTClientDP = "11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937";

		private const string OTClientDQ = "4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973";

		private const string OTClientInverseq = "5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418";
	}
}
