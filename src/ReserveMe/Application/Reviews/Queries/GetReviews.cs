namespace Application.Reviews.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;

	public record GetReviewsQuery(int VenueId) : IRequest<List<ReviewDto>>;

	public class GetReviewsQueryHandler
		: IRequestHandler<GetReviewsQuery, List<ReviewDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetReviewsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ReviewDto>> Handle(
			GetReviewsQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.VenueReviews
				.Include(x => x.User)
				.AsNoTracking()
				.Where(vr => vr.VenueId == request.VenueId)
				.Select(v => new ReviewDto
				{
					ReviewerName = v.UserId == null
						? "Anonymous"
						: ((v.User.FirstName ?? "") + " " + (v.User.LastName ?? "")).Trim(),
					Rating = v.Rating,
					CreatedAt = v.CreatedAt,
					Comment = v.Comment,
				})
				.OrderByDescending(x => x.CreatedAt)
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<VenueAdminDto>>(venue);
		}
	}
}
