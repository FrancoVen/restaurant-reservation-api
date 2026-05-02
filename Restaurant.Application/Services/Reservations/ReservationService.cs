using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Application.Interfaces.Persistence.Reservations;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Application.Services.Reservations.Interfaces;
using Restaurant.Domain.Entities;


namespace Restaurant.Application.Services.Reservations
{
    public class ReservationService : IReservationService
    {
        private readonly ILogger<ReservationService> _logger;
        private readonly IMapper _mapper;
        private readonly IReservationRepository _repository;
        private readonly IReservationValidator _reservationValidator;
        private readonly ICustomerRepository _customerRep;
        private readonly ITableRepository _tableRep;

        public ReservationService(ILogger<ReservationService> logger, IMapper mapper, IReservationRepository repository, IReservationValidator reservationValidator, ICustomerRepository customerRep, ITableRepository tableRep)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._repository = repository;
            this._reservationValidator = reservationValidator;
            this._customerRep = customerRep;
            this._tableRep = tableRep;
        }

        public async Task<ErrorOr<IReadOnlyCollection<ReservationDTO>>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            _logger.LogInformation("GET: All reservations were obtained from the database.");

            var reservationsDTO = _mapper.Map<IReadOnlyCollection<ReservationDTO>>(reservations);

            return reservationsDTO.ToErrorOr();
        }

        public async Task<ErrorOr<ReservationDTO>> GetByIdAsync(int id)
        {
            Reservation? reservationSearched = await _repository.GetByIdAsync(id);

            if (reservationSearched is null)
            {
                _logger.LogWarning("GET: The reservation searched with ID {id} was not found.", id);

                return Error.NotFound("Reservation.NotFound", $"The reservation with ID {id} was not found");
            }

            _logger.LogInformation("GET: Reservation with ID {Id} was found.", id);

            return _mapper.Map<ReservationDTO>(reservationSearched);
        }

        public async Task<ErrorOr<ReservationDTO>> CreateAsync(ReservationCreationDTO dto)
        {
            var customer = await _customerRep.GetByIdAsync(dto.CustomerId);
            if (customer is null)
            {
                return Error.NotFound("Customer.NotFound", $"Customer with ID '{dto.CustomerId}' is not registered or does not exist.");
            }

            var table = await _tableRep.GetByIdAsync(dto.TableId);
            if (table is null)
            {
                return Error.NotFound("Table.NotFound", $"Table with ID '{dto.TableId}' is not registered or does not exist.");
            }

            var timeValidation = await _reservationValidator.ValidateReservationTimeAsync(dto);
            if (timeValidation.IsError)
            {
                return timeValidation.Errors;
            }

            var reservation = _mapper.Map<Reservation>(dto);

            var returnedReservation = await _repository.CreateReservation(reservation);

            _logger.LogInformation("POST: Reservation created successfully with ID {ReservationId} at {ReservationTime}.", reservation.Id, reservation.ReservationTime);

            return _mapper.Map<ReservationDTO>(returnedReservation);
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(int reservationId, ReservationCreationDTO dto)
        {
            var customer = await _customerRep.GetByIdAsync(dto.CustomerId);
            if (customer is null)
            {
                return Error.NotFound("Customer.NotFound", $"Customer with ID '{dto.CustomerId}' is not registered or does not exist.");
            }

            var table = await _tableRep.GetByIdAsync(dto.TableId);
            if (table is null)
            {
                return Error.NotFound("Table.NotFound", $"Table with ID '{dto.TableId}' is not registered or does not exist.");
            }

            var timeValidation = await _reservationValidator.ValidateReservationTimeAsync(dto);
            if (timeValidation.IsError)
            {
                return timeValidation.Errors;
            }


            var searchedReservation = await _repository.GetByIdAsync(reservationId);

            if (searchedReservation is null)
            {
                _logger.LogWarning("PUT: The reservation you are trying to update does not exist in the database, ID entered {Id}.", reservationId);

                return Error.NotFound("Reservation.NotFound", $"Reservation with ID {reservationId} does not exist.");
            }

            _mapper.Map(dto, searchedReservation);

            await _repository.UpdateReservationAsync(searchedReservation);

            _logger.LogInformation("PUT: The reservation with ID {Id} has been updated succesfully", reservationId);

            return Result.Updated;
        }


        public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
        {
            Reservation? reservationSearched = await _repository.GetByIdAsync(id);

            if (reservationSearched is null)
            {
                _logger.LogWarning("DELETE: The reservation you are trying to delete does not exist in the database, ID entered {Id}.", id);

                return Error.NotFound("Reservation.NotFound", $"Reservation with ID {id} was not found");
            }

            await _repository.DeleteReservationAsync(reservationSearched);

            _logger.LogInformation("DELETE: Resevation with ID {Id} has been deleted successfully", id);

            return Result.Deleted;
        }

    }
}
