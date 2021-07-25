using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord(FileCabinetRecord newRecord, bool generateNewId = true);
        void Edit(FileCabinetRecord newRecord);
        ReadOnlyCollection<FileCabinetRecord> FindByDate(DateTime dataOfBirthday);
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);
        ReadOnlyCollection<FileCabinetRecord> GetRecords();
        int GetStat();
    }
}