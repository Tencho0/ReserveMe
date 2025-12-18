using Common.Enums;

namespace Application.Reservations.Commands
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Requests.Reservations;

	public record UpdateReservationStatusCommand(UpdateReservationStatusRequest Data) : IRequest;

	public class UpdateReservationStatusCommandHandler
		: IRequestHandler<UpdateReservationStatusCommand>
	{
		private readonly IApplicationDbContext _context;

		public UpdateReservationStatusCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		async Task IRequestHandler<UpdateReservationStatusCommand>.Handle(UpdateReservationStatusCommand request, CancellationToken cancellationToken)
		{
			var entity = await _context.Reservations
				.Where(r => r.Id == request.Data.ReservationId)
				.FirstOrDefaultAsync();

			if (entity == null) return;

			entity.Status = (ReservationStatus)request.Data.NewStatus;
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
