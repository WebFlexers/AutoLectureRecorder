using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Infrastructure.Persistence;

public class RecordingsRepository : IRecordingsRepository
{
    private readonly ISqliteDataAccess _sqliteDataAccess;
    private readonly ILogger<RecordingsRepository> _logger;

    public RecordingsRepository(ISqliteDataAccess sqliteDataAccess, ILogger<RecordingsRepository> logger)
    {
        _sqliteDataAccess = sqliteDataAccess;
        _logger = logger;
    }

    public async Task<bool> AddRecordingDirectory(string path)
    {
        try
        {
            var recordingsStorageDirectory = new RecordingsStorageDirectory(path);
            var sql = "insert into RecordingsStorageDirectories (Path) values (@Path);";
        
            var rowsAffectedNum = await _sqliteDataAccess.SaveData(sql, recordingsStorageDirectory)
                .ConfigureAwait(false);;

            if (rowsAffectedNum == 0)
            {
                _logger.LogError("Failed to add recording path: {Path}", path);
                return false;
            }

            _logger.LogInformation("Successfully added recording path: {Path}", path);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to add to recordings storage directories");
            return false;
        }
    }
    
    public async Task<IEnumerable<ReactiveRecordingsStorageDirectory>?> GetAllRecordingDirectories()
    {
        try
        {
            var sql = "select * from RecordingsStorageDirectories;";
        
            var result = await 
                _sqliteDataAccess.LoadData<RecordingsStorageDirectory, dynamic>(sql, new { })
                    .ConfigureAwait(false);;

            return result.Select(x => x.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to fetch recordings storage directories");
            return null;
        }
    }
}