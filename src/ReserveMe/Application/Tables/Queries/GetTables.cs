namespace Application.Tables.Queries
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Tables;

	public record GetTablesQuery(int VenueId) : IRequest<List<TableDto>>;

	public class GetTablesQueryHandler
		: IRequestHandler<GetTablesQuery, List<TableDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetTablesQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<TableDto>> Handle(
			GetTablesQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.Tables
				.AsNoTracking()
				.Where(vr => vr.VenueId == request.VenueId)
				.Select(v => new TableDto
				{
					Id = v.Id,
					VenueId = v.VenueId,
					IsActive = v.IsActive,
					TableNumber = v.TableNumber,
					Capacity = v.Capacity
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<VenueAdminDto>>(venue);
		}
	}
}
