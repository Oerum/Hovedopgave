namespace Database.Application.Interface;

public interface IMariaDbBackupImplementation
{
    Task Backup();
}