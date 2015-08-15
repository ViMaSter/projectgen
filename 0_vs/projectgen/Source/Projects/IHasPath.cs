namespace projectgen.Projects
{

    /// <summary>
    /// Interface that guarantees a function that returns the absolute path to what the object deriving from this class encapsulates
    /// </summary>
    interface IHasPath
    {
        /// <summary>
        /// Returns the absolute path to what this object encapsulates
        /// </summary>
        /// <returns>The absolute path</returns>
        string GetAbsolutePath();
    }
}
