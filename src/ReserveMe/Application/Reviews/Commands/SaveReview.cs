namespace Application.Reviews.Commands
{
	using Application.Common.Services.Data;
	using Domain.Entities;
	using MediatR;
	using Shared.Requests.Reviews;

	public record SaveReviewCommand(SaveReviewRequest Data) : IRequest;

	public class SaveReviewCommandHandler
		: IRequestHandler<SaveReviewCommand>
	{
		private readonly IApplicationDbContext _context;

		public SaveReviewCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		async Task IRequestHandler<SaveReviewCommand>.Handle(SaveReviewCommand request, CancellationToken cancellationToken)
		{
			var entity = new VenueReview()
			{
				UserId = request.Data.UserId,
				VenueId = request.Data.VenueId,
				Rating = request.Data.Rating,
				Comment = request.Data.Comment,
				CreatedAt = request.Data.CreatedAt
			};

			await _context.VenueReviews.AddAsync(entity);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
