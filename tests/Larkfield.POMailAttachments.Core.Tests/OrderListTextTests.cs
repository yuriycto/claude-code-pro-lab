namespace Larkfield.POMailAttachments.Tests;

public class OrderListTextTests
{
	private const long MB = SizeUnits.BytesPerMegabyte;

	[Fact]
	public void Lists_orders_with_their_sizes()
	{
		var text = OrderListText.Describe(new[] { Plan("000101", 26 * MB), Plan("000102", 31 * MB) });

		Assert.Equal("RO 000101 (26 MB), RO 000102 (31 MB)", text);
	}

	[Fact]
	public void Long_lists_are_cut_with_a_count_of_the_rest()
	{
		var plans = Enumerable.Range(1, 5).Select(i => Plan("00010" + i, 30 * MB)).ToList();

		var text = OrderListText.Describe(plans, maxListed: 2);

		Assert.Equal("RO 000101 (30 MB), RO 000102 (30 MB) (+3)", text);
	}

	[Fact]
	public void Empty_list_gives_empty_text()
	{
		Assert.Equal(string.Empty, OrderListText.Describe(Array.Empty<OrderMailPlan>()));
	}

	[Fact]
	public void At_least_one_order_must_be_listed()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => OrderListText.Describe(new[] { Plan("000101", MB) }, maxListed: 0));
	}

	private static OrderMailPlan Plan(string orderNbr, long bytes)
		=> OrderMailPlan.Create("RO", orderNbr, attachFiles: true,
			new[] { new OrderFile(Guid.NewGuid(), orderNbr + ".pdf", bytes) });
}
