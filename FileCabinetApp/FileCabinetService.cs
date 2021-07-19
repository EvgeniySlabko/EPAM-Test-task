using System;
using System.Collections.Generic;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short pointsForFourTests, decimal identificationNumber, char identificationLetter)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
        {
            throw new ArgumentException($"Invalid {nameof(firstName)}");
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
        {
            throw new ArgumentException($"Invalid {nameof(lastName)}");
        }

        if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
        {
            throw new ArgumentException($"Invalid {nameof(dateOfBirth)}");
        }

        if (!char.IsLetter(identificationLetter))
        {
            throw new ArgumentException($"Invalid {nameof(identificationLetter)}");
        }

        if (pointsForFourTests > 400 || pointsForFourTests < 0)
        {
            throw new ArgumentException($"Invalid {nameof(pointsForFourTests)}");
        }

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            PointsForFourTests = pointsForFourTests,
            IdentificationNumber = identificationNumber,
            IdentificationLetter = identificationLetter,
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
}
