using System.ComponentModel.DataAnnotations;

namespace PiedraAzul.Client.Models.Admin;

public class ScheduleConfigEditModel
{
    [Required(ErrorMessage = "Selecciona un especialista.")]
    public string SpecialistId { get; set; } = "";

    [Range(1, 12, ErrorMessage = "La ventana de semanas debe ser entre 1 y 12.")]
    public int WeekWindowInWeeks { get; set; } = 1;

    [Required]
    public string StartTime { get; set; } = "08:00";

    [Required]
    public string EndTime { get; set; } = "17:00";

    [Range(5, 60, ErrorMessage = "El intervalo debe ser entre 5 y 60 minutos.")]
    public int IntervalMinutes { get; set; } = 10;

    public bool MondayEnabled { get; set; } = true;
    public bool TuesdayEnabled { get; set; } = true;
    public bool WednesdayEnabled { get; set; } = true;
    public bool ThursdayEnabled { get; set; } = true;
    public bool FridayEnabled { get; set; } = true;
}
