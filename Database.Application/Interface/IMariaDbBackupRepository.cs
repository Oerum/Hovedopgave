namespace Database.Application.Interface;

public interface IMariaDbBackupRepository
{
    Task Backup();
}