namespace Larkfield.POMailAttachments.Tests;

public class OrderMailPlanTests
{
	[Fact]
	public void Order_that_does_not_ask_for_files_gets_none()
	{
		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: false,
			new[] { new OrderFile(Guid.NewGuid(), "quote.pdf", 1000) });

		Assert.False(plan.HasFiles);
		Assert.Equal(0, plan.TotalBytes);
	}

	[Fact]
	public void Order_without_files_gets_none()
	{
		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: true, Array.Empty<OrderFile>());

		Assert.False(plan.HasFiles);
	}

	[Fact]
	public void Null_file_list_is_treated_as_empty()
	{
		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: true, files: null!);

		Assert.False(plan.HasFiles);
	}

	[Fact]
	public void All_files_are_sent_and_sizes_add_up()
	{
		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: true, new[]
		{
			new OrderFile(Guid.NewGuid(), "quote.pdf", 1000),
			new OrderFile(Guid.NewGuid(), "drawing.dwg", 2500),
		});

		Assert.Equal(2, plan.Files.Count);
		Assert.Equal(3500, plan.TotalBytes);
	}

	[Fact]
	public void File_linked_twice_is_sent_once_and_counted_once()
	{
		var fileId = Guid.NewGuid();
		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: true, new[]
		{
			new OrderFile(fileId, "spec.pdf", 700),
			new OrderFile(fileId, "spec.pdf", 700),
		});

		Assert.Single(plan.Files);
		Assert.Equal(700, plan.TotalBytes);
	}

	[Fact]
	public void Files_keep_their_original_order()
	{
		var first = new OrderFile(Guid.NewGuid(), "a.pdf", 1);
		var second = new OrderFile(Guid.NewGuid(), "b.pdf", 1);

		var plan = OrderMailPlan.Create("RO", "000123", attachFiles: true, new[] { first, second });

		Assert.Equal(new[] { first.FileId, second.FileId }, plan.FileIds);
	}

	[Fact]
	public void Display_name_and_key_ignore_padding()
	{
		var plan = OrderMailPlan.Create("RO ", " 000123 ", attachFiles: false, files: null!);

		Assert.Equal("RO 000123", plan.DisplayName);
		Assert.Equal(OrderMailPlan.KeyOf("RO", "000123"), plan.Key);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("  ")]
	public void Order_number_is_required(string? orderNbr)
	{
		Assert.Throws<ArgumentException>(() => OrderMailPlan.Create("RO", orderNbr!, attachFiles: true, files: null!));
	}

	[Fact]
	public void File_rejects_empty_id_and_negative_size()
	{
		Assert.Throws<ArgumentException>(() => new OrderFile(Guid.Empty, "a.pdf", 1));
		Assert.Throws<ArgumentOutOfRangeException>(() => new OrderFile(Guid.NewGuid(), "a.pdf", -1));
	}
}
