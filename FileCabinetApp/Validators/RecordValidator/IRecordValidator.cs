namespace FileCabinetApp
{
    /// <summary>
    /// Interface for validators.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Method for parameter validation.
        /// </summary>
        /// <param name="record">Given record.</param>
        /// <returns>Result of validation.</returns>
        public bool ValidateParameters(FileCabinetRecord record);
    }
}
