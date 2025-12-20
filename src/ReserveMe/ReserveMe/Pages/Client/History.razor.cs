using Microsoft.AspNetCore.Components;

namespace ReserveMe.Pages.Client;



    
    
        public partial class History : ComponentBase
        {
            protected List<Reservation> Reservations { get; set; } = new();

        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

            protected override void OnInitialized()
            {
                Reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = 1,
                    RestaurantName = "Pizza",
                    Number = "#11111",
                    Date = new DateTime(2025, 11, 20, 19, 30, 0),
                    Status = "Потвърдена",
                    Address = "ул. „Пицария“ 10, София",
                    LogoFallback = "P",
                    QrCodeText = "QR"
                },
                new Reservation
                {
                    Id = 2,
                    RestaurantName = "Garden",
                    Number = "#11111",
                    Date = new DateTime(2025, 11, 20, 19, 30, 0),
                    Status = "Планирана",
                    Address = "ул. „Пицария“ 10, София",
                    LogoFallback = "G",
                    QrCodeText = "QR"
                },
                new Reservation
                {
                    Id = 3,
                    RestaurantName = "Bistro",
                    Number = "#11111",
                    Date = new DateTime(2025, 11, 20, 19, 30, 0),
                    Status = "Отминала",
                    Address = "ул. „Пицария“ 10, София",
                    LogoFallback = "B",
                    QrCodeText = "QR"
                }
            };
            }

            protected void OpenReservation(int id)
            {
                Console.WriteLine($"Open reservation {id}");
                NavigationManager.NavigateTo($"/reservation/{id}");
            }
        }

        public class Reservation
        {
            public int Id { get; set; }
            public string RestaurantName { get; set; } = string.Empty;
            public string Number { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string LogoFallback { get; set; } = string.Empty;
            public string QrCodeText { get; set; } = string.Empty;
        }
    



