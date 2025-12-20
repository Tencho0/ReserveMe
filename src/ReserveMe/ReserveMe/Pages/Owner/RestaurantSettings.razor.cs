using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ReserveMe.Pages.Owner;

    

    public partial class RestaurantSettings
    {
        public class CoordinatesModel
        {
            public string Latitude { get; set; } = "42.6977";
            public string Longitude { get; set; } = "23.3219";
        }

        public class WorkingDayModel
        {
            public string DayName { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public bool IsEnabled { get; set; }
            public TimeOnly OpeningTime { get; set; }
            public TimeOnly ClosingTime { get; set; }
        }
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        protected CoordinatesModel Coordinates { get; set; } = new();
        protected List<WorkingDayModel> WorkingDays { get; set; } = new();

        protected override void OnInitialized()
        {
            InitializeWorkingDays();
        }

        private void InitializeWorkingDays()
        {
            WorkingDays = new List<WorkingDayModel>
        {
            new WorkingDayModel
            {
                DayName = "monday",
                DisplayName = "Понеделник",
                IsEnabled = true,
                OpeningTime = new TimeOnly(8, 0, 0),
                ClosingTime = new TimeOnly(22, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "tuesday",
                DisplayName = "Вторник",
                IsEnabled = true,
                OpeningTime = new TimeOnly(8, 0, 0),
                ClosingTime = new TimeOnly(22, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "wednesday",
                DisplayName = "Сряда",
                IsEnabled = true,
                OpeningTime = new TimeOnly(8, 0, 0),
                ClosingTime = new TimeOnly(22, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "thursday",
                DisplayName = "Четвъртък",
                IsEnabled = true,
                OpeningTime = new TimeOnly(8, 0, 0),
                ClosingTime = new TimeOnly(22, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "friday",
                DisplayName = "Петък",
                IsEnabled = true,
                OpeningTime = new TimeOnly(8, 0, 0),
                ClosingTime = new TimeOnly(23, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "saturday",
                DisplayName = "Събота",
                IsEnabled = false,
                OpeningTime = new TimeOnly(9, 0, 0),
                ClosingTime = new TimeOnly(23, 0, 0)
            },
            new WorkingDayModel
            {
                DayName = "sunday",
                DisplayName = "Неделя",
                IsEnabled = false,
                OpeningTime = new TimeOnly(10, 0, 0),
                ClosingTime = new TimeOnly(20, 0, 0)
            }
        };
        }

        protected void OnDayToggleChanged(WorkingDayModel day)
        {
            day.IsEnabled = !day.IsEnabled;
        }

        protected void OnSave(MouseEventArgs e)
        {
            Navigation.NavigateTo("/");
        }

        protected void OnCancel(MouseEventArgs e)
        {
            Navigation.NavigateTo("/");
        }
    }

