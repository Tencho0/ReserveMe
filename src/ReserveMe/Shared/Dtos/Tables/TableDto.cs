namespace Shared.Dtos.Tables
{
	public class TableDto
	{
		public int Id { get; set; }

		public int VenueId { get; set; }

		public int TableNumber { get; set; }

		public int Capacity { get; set; }

		public bool IsActive { get; set; }
	}
}
