namespace Larkfield.POMailAttachments.Tests;

public class MailSizeLimitTests
{
	private const long MB = SizeUnits.BytesPerMegabyte;

	[Fact]
	public void Default_is_25_megabytes()
	{
		Assert.Equal(25 * MB, MailSizeLimit.Default.Bytes);
	}

	[Theory]
	[InlineData(0, false)]
	[InlineData(25 * MB - 1, false)]
	[InlineData(25 * MB, false)]
	[InlineData(25 * MB + 1, true)]
	public void Only_a_size_above_the_limit_exceeds_it(long totalBytes, bool exceeded)
	{
		Assert.Equal(exceeded, MailSizeLimit.Default.IsExceededBy(totalBytes));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Limit_must_be_positive(long bytes)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => new MailSizeLimit(bytes));
	}

	[Fact]
	public void FindOversized_keeps_only_large_plans_in_original_order()
	{
		var limit = new MailSizeLimit(10 * MB);
		var plans = new[]
		{
			Plan("000001", 11 * MB),
			Plan("000002", 10 * MB),
			Plan("000003", 2 * MB),
			Plan("000004", 40 * MB),
		};

		var oversized = limit.FindOversized(plans);

		Assert.Equal(new[] { "000001", "000004" }, oversized.Select(plan => plan.OrderNbr));
	}

	[Fact]
	public void FindOversized_ignores_orders_that_do_not_send_files()
	{
		var plan = OrderMailPlan.Create("RO", "000001", attachFiles: false,
			new[] { new OrderFile(Guid.NewGuid(), "huge.zip", 100 * MB) });

		Assert.Empty(MailSizeLimit.Default.FindOversized(new[] { plan }));
	}

	[Fact]
	public void FindOversized_accepts_null()
	{
		Assert.Empty(MailSizeLimit.Default.FindOversized(null!));
	}

	private static OrderMailPlan Plan(string orderNbr, long bytes)
		=> OrderMailPlan.Create("RO", orderNbr, attachFiles: true,
			new[] { new OrderFile(Guid.NewGuid(), orderNbr + ".pdf", bytes) });
}
