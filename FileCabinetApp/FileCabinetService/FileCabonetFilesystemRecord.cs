namespace FileCabinetApp
{
    /// <summary>
    /// Filesystem record.
    /// </summary>
    public class FileCabonetFilesystemRecord
    {
        /// <summary>
        /// Gets or sets record.
        /// </summary>
        /// <value>record.</value>
        public FileCabinetRecord Record { get; set; }

        /// <summary>
        /// Gets or sets record service information in file system.
        /// </summary>
        /// <value>Record service information.</value>
        public short ServiceInormation { get; set; }
    }
}
