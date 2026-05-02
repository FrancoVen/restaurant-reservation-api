using Restaurant.Application.Dtos.Reservations;
using Restaurant.Application.Dtos.Tables;
using Restaurant.Domain.Entities;

namespace Restaurant.UnitTests.TestFactories
{
    public static class TableFactory
    {
        private const int DefaultId = 1;

        private const int DefaultTableNumber = 1;

        private const int DefaultCapacity = 6;


        public static Table CreateTable(int? id = null, int? tableNumber = null, int? capacity = null) => new()
        {
            Id = id ?? DefaultId,
            TableNumber = tableNumber ?? DefaultTableNumber,
            Capacity = capacity ?? DefaultCapacity
        };

        public static TableDTO CreateTableDTO(int? id = null, int? tableNumber = null, int? capacity = null) => new()
        {
            Id = id ?? DefaultId,
            Capacity = capacity ?? DefaultCapacity,
            TableNumber = tableNumber ?? DefaultTableNumber
        };

        public static TableCreationDTO CreateTableCreationDTO(int? tableNumber = null, int? capacity = null) => new()
        {
            Capacity = capacity ?? DefaultCapacity,
            TableNumber = tableNumber ?? DefaultTableNumber
        };

        public static TableReservationDTO CreateTableReservationDTO(int? tableNumber = null, int? capacity = null) => new()
        {
            Table = CreateTableDTO(),
            Reservations = new List<ReservationDTO>()
        };


        public static List<Table> CreateTableList() => new()
        {
            CreateTable(1,2),
            CreateTable(2,3),
            CreateTable(3,4)
        };

        public static List<TableDTO> CreateTableDTOList() => new()
        {
            CreateTableDTO(1,2),
            CreateTableDTO(2,3),
            CreateTableDTO(3,4)
        };
    }
}
