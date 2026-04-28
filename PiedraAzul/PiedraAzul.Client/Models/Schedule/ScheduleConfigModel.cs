namespace PiedraAzul.Client.Models.Schedule;

public class ScheduleConfigModel
{
    public string DoctorId { get; set; } = string.Empty;
    public int BookingWindowWeeks { get; set; } = 1;
    public int IntervalMinutes { get; set; } = 10;
    public List<AvailabilityDayModel> Availability { get; set; } = [];
}
