using System;
using System.Collections.Generic;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(FileCabinetRecord newRecord)
    {
        if (newRecord is null)
        {
            throw new ArgumentNullException(nameof(newRecord));
        }

        ValidityTest(newRecord);
        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = newRecord.FirstName,
            LastName = newRecord.LastName,
            DateOfBirth = newRecord.DateOfBirth,
            PointsForFourTests = newRecord.PointsForFourTests,
            IdentificationNumber = newRecord.IdentificationNumber,
            IdentificationLetter = newRecord.IdentificationLetter,
        };

        this.list.Add(record);

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        return this.list.ToArray();
    }

    public int GetStat()
    {
        return this.list.Count;
    }

    public void Edit(FileCabinetRecord newRecord)
    {
        if (newRecord is null)
        {
            throw new ArgumentNullException(nameof(newRecord));
        }

        foreach (var record in this.list)
        {
            if (record.Id == newRecord.Id)
            {
                ValidityTest(newRecord);
                record.FirstName = newRecord.FirstName;
                record.LastName = newRecord.LastName;
                record.DateOfBirth = newRecord.DateOfBirth;
                record.PointsForFourTests = newRecord.PointsForFourTests;
                record.IdentificationNumber = newRecord.IdentificationNumber;
                record.IdentificationLetter = newRecord.IdentificationLetter;
                return;
            }
        }

        throw new ArgumentException("Id was not found");
    }

    private static void ValidityTest(FileCabinetRecord newRecord)
    {
        if (string.IsNullOrWhiteSpace(newRecord.FirstName) || newRecord.FirstName.Length < 2 || newRecord.FirstName.Length > 60)
        {
            throw new ArgumentException($"Invalid {nameof(newRecord.FirstName)}");
        }

        if (string.IsNullOrWhiteSpace(newRecord.LastName) || newRecord.LastName.Length < 2 || newRecord.LastName.Length > 60)
        {
            throw new ArgumentException($"Invalid {nameof(newRecord.LastName)}");
        }

        if (newRecord.DateOfBirth < new DateTime(1950, 1, 1) || newRecord.DateOfBirth > DateTime.Today)
        {
            throw new ArgumentException($"Invalid {nameof(newRecord.DateOfBirth)}");
        }

        if (!char.IsLetter(newRecord.IdentificationLetter))
        {
            throw new ArgumentException($"Invalid {nameof(newRecord.IdentificationLetter)}");
        }

        if (newRecord.PointsForFourTests > 400 || newRecord.PointsForFourTests < 0)
        {
            throw new ArgumentException($"Invalid {nameof(newRecord.PointsForFourTests)}");
        }
    }
}
