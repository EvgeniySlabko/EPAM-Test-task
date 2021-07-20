using System;
using System.Collections.Generic;
using System.Globalization;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(FileCabinetRecord newRecord)
    {
        ValidityTest(newRecord);
        newRecord.Id = this.list.Count + 1;

        // add record in main list
        this.list.Add(newRecord);

        return newRecord.Id;
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

    public FileCabinetRecord[] FindByFirstName(string firstName)
    {
        if (firstName is null)
        {
            throw new ArgumentNullException(nameof(firstName));
        }

        List<FileCabinetRecord> subList = new List<FileCabinetRecord>();
        foreach (var record in this.list)
        {
            if (record.FirstName.ToLower(CultureInfo.CurrentCulture) == firstName.ToLower(CultureInfo.CurrentCulture))
            {
                subList.Add(record);
            }
        }

        return subList.ToArray();
    }

    public FileCabinetRecord[] FindByLastName(string lastName)
    {
        if (lastName is null)
        {
            throw new ArgumentNullException(nameof(lastName));
        }

        List<FileCabinetRecord> subList = new List<FileCabinetRecord>();
        foreach (var record in this.list)
        {
            if (record.LastName.ToLower(CultureInfo.CurrentCulture) == lastName.ToLower(CultureInfo.CurrentCulture))
            {
                subList.Add(record);
            }
        }

        return subList.ToArray();
    }

    public FileCabinetRecord[] FindByDate(DateTime dataOfBirthday)
    {
        List<FileCabinetRecord> subList = new List<FileCabinetRecord>();
        foreach (var record in this.list)
        {
            if (record.DateOfBirth == dataOfBirthday)
            {
                subList.Add(record);
            }
        }

        return subList.ToArray();
    }

    private static void ValidityTest(FileCabinetRecord newRecord)
    {
        if (newRecord is null)
        {
            throw new ArgumentNullException(nameof(newRecord));
        }

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
