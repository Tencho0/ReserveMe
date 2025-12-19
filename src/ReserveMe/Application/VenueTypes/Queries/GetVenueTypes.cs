namespace Application.VenueTypes.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.VenueTypes;

	public record GetVenueTypesQuery() : IRequest<List<VenueTypeDto>>;

	public class GetVenueTypesQueryHandler
		: IRequestHandler<GetVenueTypesQuery, List<VenueTypeDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetVenueTypesQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<VenueTypeDto>> Handle(
			GetVenueTypesQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.VenueTypes
				.Select(v => new VenueTypeDto
				{
					Id = v.Id,
					Name = v.Name
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
		}
	}
}
