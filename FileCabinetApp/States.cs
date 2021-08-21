namespace FileCabinetApp
{
    /// <summary>
    /// Type of validation rule.
    /// </summary>
    public enum ValidationRule
    {
        /// <summary>
        /// Default validation rule.
        /// </summary>
        Default,

        /// <summary>
        /// Custom validation rule.
        /// </summary>
        Custom,
    }

    /// <summary>
    /// Service type.
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// Records are stored in a Memory.
        /// </summary>
        MemoryService,

        /// <summary>
        /// Records are stored in a file.
        /// </summary>
        FileService,
    }
}
