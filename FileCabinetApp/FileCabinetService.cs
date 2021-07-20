using System;
using System.Collections.Generic;
using System.Globalization;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

    public int CreateRecord(FileCabinetRecord newRecord, bool generateNewId = true)
    {
        ValidityTest(newRecord);
        FileCabinetRecord currrentRecord = new FileCabinetRecord();

        currrentRecord.Id = generateNewId ? this.list.Count + 1 : newRecord.Id;
        currrentRecord.FirstName = newRecord.FirstName;
        currrentRecord.LastName = newRecord.LastName;
        currrentRecord.DateOfBirth = newRecord.DateOfBirth;

        currrentRecord.IdentificationLetter = newRecord.IdentificationLetter;
        currrentRecord.IdentificationNumber = newRecord.IdentificationNumber;
        currrentRecord.PointsForFourTests = newRecord.PointsForFourTests;

        List<FileCabinetRecord> subList;

        // add record in main list
        this.list.Add(currrentRecord);

        // add record in firstNameDictionary
        if (this.firstNameDictionary.TryGetValue(currrentRecord.FirstName.ToLower(CultureInfo.CurrentCulture), out subList))
        {
            subList.Add(currrentRecord);
        }
        else
        {
            subList = new List<FileCabinetRecord>();
            subList.Add(currrentRecord);
            this.firstNameDictionary.Add(currrentRecord.FirstName.ToLower(CultureInfo.CurrentCulture), subList);
        }

        // add record in lastNameDictionary
        if (this.lastNameDictionary.TryGetValue(currrentRecord.LastName.ToLower(CultureInfo.CurrentCulture), out subList))
        {
            subList.Add(currrentRecord);
        }
        else
        {
            subList = new List<FileCabinetRecord>();
            subList.Add(currrentRecord);
            this.lastNameDictionary.Add(currrentRecord.LastName.ToLower(CultureInfo.CurrentCulture), subList);
        }

        return currrentRecord.Id;
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

                this.firstNameDictionary[record.FirstName.ToLower(CultureInfo.CurrentCulture)].Remove(record);
                this.firstNameDictionary.Remove(record.FirstName.ToLower(CultureInfo.CurrentCulture));

                this.lastNameDictionary[record.LastName.ToLower(CultureInfo.CurrentCulture)].Remove(record);
                this.lastNameDictionary.Remove(record.LastName.ToLower(CultureInfo.CurrentCulture));

                this.list.Remove(record);

                this.CreateRecord(newRecord, false);

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
        if (this.firstNameDictionary.TryGetValue(firstName.ToLower(CultureInfo.CurrentCulture), out subList))
        {
            return subList.ToArray();
        }

        return null;
    }

    public FileCabinetRecord[] FindByLastName(string lastName)
    {
        if (lastName is null)
        {
            throw new ArgumentNullException(nameof(lastName));
        }

        List<FileCabinetRecord> subList = new List<FileCabinetRecord>();
        if (this.lastNameDictionary.TryGetValue(lastName.ToLower(CultureInfo.CurrentCulture), out subList))
        {
            return subList.ToArray();
        }

        return null;
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
