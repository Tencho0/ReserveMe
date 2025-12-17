namespace Application.Venues.Commands
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;

	public record DeleteVenueCommand(int VenueId) : IRequest;

	public class DeleteVenueCommandHandler
		: IRequestHandler<DeleteVenueCommand>
	{
		private readonly IApplicationDbContext _context;

		public DeleteVenueCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		async Task IRequestHandler<DeleteVenueCommand>.Handle(DeleteVenueCommand request,
			CancellationToken cancellationToken)
		{
			var entity = await _context.Venues.FindAsync(request.VenueId);

			if (entity == null) return;

			entity.IsDeleted = true;

			var menuUsers = await _context.Users.Where(u => u.VenueId == request.VenueId).ToListAsync();

			foreach (var menuUser in menuUsers)
			{
				menuUser.VenueId = null;
			}

			await _context.SaveChangesAsync(cancellationToken);
		}
	}

}
