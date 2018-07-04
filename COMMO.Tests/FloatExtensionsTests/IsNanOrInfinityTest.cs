using FluentAssertions;
using NUnit.Framework;

namespace COMMO.Tests.FloatExtensionsTests {
    public class IsNanOrInfinityTest {

		[TestCase(float.PositiveInfinity)]
		[TestCase(float.NegativeInfinity)]
		public void ShouldReturnTrueIfValueIsInfinity(float value) =>
			value.IsNanOrInfinity().Should().BeTrue();

		[TestCase(float.NaN)]
		public void ShouldReturnTrueIfValueIsNaN(float value) =>
			value.IsNanOrInfinity().Should().BeTrue();

		[TestCase(float.MaxValue)]
		[TestCase(float.MinValue)]
		[TestCase(float.Epsilon)]
		[TestCase(-1f)]
		[TestCase(0f)]
		[TestCase(1f)]
		public void ShouldReturnFalseIfValueIsNotInifnityOrNaN(float value) =>
			value.IsNanOrInfinity().Should().BeFalse();
	}
}
