using System;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Record.
/// </summary>
public class FileCabinetRecord : ICloneable
{
    /// <summary>
    /// Gets or sets record id.
    /// </summary>
    /// <value>Record id.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets record first name.
    /// </summary>
    /// <value>Record first name.</value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets record last name.
    /// </summary>
    /// <value>Record last name.</value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets record date of birthday.
    /// </summary>
    /// <value>Record date of birthday.</value>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets record points for four tests.
    /// </summary>
    /// <value>Record points for four tests.</value>
    public short PointsForFourTests { get; set; }

    /// <summary>
    /// Gets or sets identification number.
    /// </summary>
    /// <value>Record identification number.</value>
    public decimal IdentificationNumber { get; set; }

    /// <summary>
    /// Gets or sets identification letter.
    /// </summary>
    /// <value>Record identification letter.</value>
    public char IdentificationLetter { get; set; }

    /// <summary>
    /// Clone.
    /// </summary>
    /// <returns>Copy of record.</returns>
    public object Clone()
    {
        return new FileCabinetRecord()
        {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            DateOfBirth = this.DateOfBirth,
            IdentificationLetter = this.IdentificationLetter,
            IdentificationNumber = this.IdentificationNumber,
            PointsForFourTests = this.PointsForFourTests,
        };
    }
}