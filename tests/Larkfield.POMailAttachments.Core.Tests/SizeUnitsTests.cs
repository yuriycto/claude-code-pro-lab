namespace Larkfield.POMailAttachments.Tests;

public class SizeUnitsTests
{
	[Theory]
	[InlineData(null, 0L)]
	[InlineData(0, 0L)]
	[InlineData(-5, 0L)]
	[InlineData(1, 1024L)]
	[InlineData(25600, 26214400L)]
	public void FromKilobytes_converts_revision_size(int? kilobytes, long expected)
	{
		Assert.Equal(expected, SizeUnits.FromKilobytes(kilobytes));
	}

	[Theory]
	[InlineData(0L, "0")]
	[InlineData(1L, "0.1")]
	[InlineData(26214400L, "25")]
	[InlineData(26214401L, "25.1")]
	[InlineData(27577548L, "26.3")]
	[InlineData(27577549L, "26.4")]
	public void ToMegabytes_rounds_up_to_one_decimal(long bytes, string expected)
	{
		Assert.Equal(expected, SizeUnits.ToMegabytes(bytes));
	}

	[Fact]
	public void ToMegabytes_ignores_current_culture()
	{
		var original = Thread.CurrentThread.CurrentCulture;
		try
		{
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("uk-UA");
			Assert.Equal("25.1", SizeUnits.ToMegabytes(26214401L));
		}
		finally
		{
			Thread.CurrentThread.CurrentCulture = original;
		}
	}
}
