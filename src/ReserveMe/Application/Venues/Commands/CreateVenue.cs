namespace Application.Venues.Commands
{
	using Application.Common.Services.Data;
	using Domain.Entities;
	using MediatR;
	using Shared.Requests.Venues;

	public record CreateVenueCommand(SaveVenueRequest Data) : IRequest;

	public class CreateVenueCommandHandler
		: IRequestHandler<CreateVenueCommand>
	{
		private readonly IApplicationDbContext _context;

		public CreateVenueCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		async Task IRequestHandler<CreateVenueCommand>.Handle(CreateVenueCommand request, CancellationToken cancellationToken)
		{
			var entity = new Venue()
			{
				VenueTypeId = request.Data.VenueTypeId,
				Name = request.Data.Name,
				Description = request.Data.Description,
				Longitude = request.Data.Longitude,
				Latitude = request.Data.Latitude,
				LogoUrl = request.Data.LogoUrl,
				ImageUrl = request.Data.ImageUrl,
				CreatedAt = DateTime.UtcNow
			};

			await _context.Venues.AddAsync(entity);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
