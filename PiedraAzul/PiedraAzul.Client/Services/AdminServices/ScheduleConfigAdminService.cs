using PiedraAzul.Client.Models.Admin;

namespace PiedraAzul.Client.Services.AdminServices;

public class ScheduleConfigAdminService
{
    private readonly Dictionary<string, ScheduleConfigEditModel> _store = new();

    public Task<ScheduleConfigEditModel> GetBySpecialistAsync(string specialistId)
    {
        if (string.IsNullOrWhiteSpace(specialistId))
        {
            return Task.FromResult(new ScheduleConfigEditModel());
        }

        if (!_store.TryGetValue(specialistId, out var config))
        {
            config = BuildDefaultConfig(specialistId);
            _store[specialistId] = Clone(config);
        }

        return Task.FromResult(Clone(config));
    }

    public async Task SaveAsync(ScheduleConfigEditModel model, CancellationToken cancellationToken = default)
    {
        await Task.Delay(450, cancellationToken);
        _store[model.SpecialistId] = Clone(model);
    }

    private static ScheduleConfigEditModel BuildDefaultConfig(string specialistId)
    {
        var hash = Math.Abs(specialistId.GetHashCode());
        var startHour = 7 + (hash % 3);
        var endHour = 16 + (hash % 4);
        var interval = new[] { 10, 15, 20 }[hash % 3];

        return new ScheduleConfigEditModel
        {
            SpecialistId = specialistId,
            WeekWindowInWeeks = 2 + (hash % 3),
            StartTime = $"{startHour:00}:00",
            EndTime = $"{endHour:00}:00",
            IntervalMinutes = interval,
            MondayEnabled = true,
            TuesdayEnabled = true,
            WednesdayEnabled = hash % 2 == 0,
            ThursdayEnabled = true,
            FridayEnabled = hash % 3 != 0
        };
    }

    private static ScheduleConfigEditModel Clone(ScheduleConfigEditModel model) => new()
    {
        SpecialistId = model.SpecialistId,
        WeekWindowInWeeks = model.WeekWindowInWeeks,
        StartTime = model.StartTime,
        EndTime = model.EndTime,
        IntervalMinutes = model.IntervalMinutes,
        MondayEnabled = model.MondayEnabled,
        TuesdayEnabled = model.TuesdayEnabled,
        WednesdayEnabled = model.WednesdayEnabled,
        ThursdayEnabled = model.ThursdayEnabled,
        FridayEnabled = model.FridayEnabled
    };
}
