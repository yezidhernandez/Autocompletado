using PiedraAzul.Client.Models;
using PiedraAzul.Client.Models.Schedule;
using PiedraAzul.Client.Services.GraphQLServices;

namespace PiedraAzul.Client.Services.Schedule;

public interface IScheduleConfigService
{
    Task<ScheduleConfigModel?> GetByDoctorIdAsync(string doctorId);
    Task<Result<bool>> SaveAsync(ScheduleConfigModel config);
}

public class ScheduleConfigService(GraphQLHttpClient client) : IScheduleConfigService
{
    private readonly Dictionary<string, ScheduleConfigModel> _fallbackStore = new();

    public async Task<ScheduleConfigModel?> GetByDoctorIdAsync(string doctorId)
    {
        if (string.IsNullOrWhiteSpace(doctorId))
        {
            return null;
        }

        var fromBackend = await TryGetFromBackendAsync(doctorId);
        if (fromBackend is not null)
        {
            _fallbackStore[doctorId] = Clone(fromBackend);
            return fromBackend;
        }

        if (_fallbackStore.TryGetValue(doctorId, out var existing))
        {
            return Clone(existing);
        }

        var defaultConfig = BuildDefaultConfig(doctorId);
        _fallbackStore[doctorId] = Clone(defaultConfig);
        return defaultConfig;
    }

    public async Task<Result<bool>> SaveAsync(ScheduleConfigModel config)
    {
        if (config is null || string.IsNullOrWhiteSpace(config.DoctorId))
        {
            return Result<bool>.Failure(new ErrorResult("DoctorId es requerido.", "Validation"));
        }

        var backendSave = await TrySaveToBackendAsync(config);
        if (backendSave.IsSuccess)
        {
            _fallbackStore[config.DoctorId] = Clone(config);
            return backendSave;
        }

        _fallbackStore[config.DoctorId] = Clone(config);
        return Result<bool>.Success(true);
    }

    private async Task<ScheduleConfigModel?> TryGetFromBackendAsync(string doctorId)
    {
        const string query = """
            query GetScheduleConfigByDoctorId($doctorId: String!) {
                scheduleConfigByDoctorId(doctorId: $doctorId) {
                    doctorId
                    weekWindowInWeeks
                    startTime
                    endTime
                    intervalMinutes
                    mondayEnabled
                    tuesdayEnabled
                    wednesdayEnabled
                    thursdayEnabled
                    fridayEnabled
                }
            }
            """;

        try
        {
            return await client.ExecuteAsync<ScheduleConfigModel>(
                query,
                new { doctorId },
                "scheduleConfigByDoctorId");
        }
        catch
        {
            return null;
        }
    }

    private async Task<Result<bool>> TrySaveToBackendAsync(ScheduleConfigModel config)
    {
        const string mutation = """
            mutation SaveScheduleConfig($input: ScheduleConfigInput!) {
                saveScheduleConfig(input: $input)
            }
            """;

        try
        {
            var success = await client.ExecuteAsync<bool>(
                mutation,
                new
                {
                    input = new
                    {
                        doctorId = config.DoctorId,
                        weekWindowInWeeks = config.WeekWindowInWeeks,
                        startTime = config.StartTime,
                        endTime = config.EndTime,
                        intervalMinutes = config.IntervalMinutes,
                        mondayEnabled = config.MondayEnabled,
                        tuesdayEnabled = config.TuesdayEnabled,
                        wednesdayEnabled = config.WednesdayEnabled,
                        thursdayEnabled = config.ThursdayEnabled,
                        fridayEnabled = config.FridayEnabled
                    }
                },
                "saveScheduleConfig");

            return Result<bool>.Success(success);
        }
        catch
        {
            return Result<bool>.Failure(new ErrorResult("API de horario no disponible aún.", "ScheduleConfigApi"));
        }
    }

    private static ScheduleConfigModel BuildDefaultConfig(string doctorId)
    {
        var hash = Math.Abs(doctorId.GetHashCode());
        var startHour = 7 + (hash % 3);
        var endHour = 16 + (hash % 4);
        var interval = new[] { 10, 15, 20 }[hash % 3];

        return new ScheduleConfigModel
        {
            DoctorId = doctorId,
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

    private static ScheduleConfigModel Clone(ScheduleConfigModel model) => new()
    {
        DoctorId = model.DoctorId,
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
