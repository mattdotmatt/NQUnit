using System;
using System.IO;
using System.Reflection;

namespace NQunit.Tests
{
    /// <summary>
    /// Get a database from an embedded resource, save it to the working directory, then remove it when we are done
    /// </summary>

    public class EmbeddedResourceHelper : IDisposable
    {

        #region Properties

        public string FullPath { get; private set; }

        #endregion

        #region Public Constructors

        public EmbeddedResourceHelper(Assembly assembly, string resourceName, string filename)
        {
            FullPath = Path.Combine(Path.GetTempPath(), filename);
            try
            {

                // Save the file to disk, and build the ConnectionString
                WriteToFile(assembly, resourceName);
            }
            catch (Exception)
            {
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                }
                throw;
            }

            //  Notice - The file will be removed when this object is disposed
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose of this object, and make sure the file we created has been deleted
        /// </summary>
        public void Dispose()
        {
            if (String.IsNullOrEmpty(FullPath))
            {
                Console.WriteLine("An embedded file was never created");
            }
            else if (File.Exists(FullPath) == false)
            {
                Console.WriteLine(String.Format("The file {0} should have been deleted from the directory {1}, but it could not be found", FullPath, Directory.GetCurrentDirectory()));
            }
            else
            {
                File.Delete(FullPath);
            }
        }

        #endregion

        #region Private Methods

        private void WriteToFile(Assembly assembly, string resourceName)
        {
            // Use a temp file

            using (var inStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (inStream == null)
                    throw new ApplicationException(String.Format("Resource name {0} not found. Check it is defined as an embedded resource.", resourceName));

                using (var outStream = File.Create(FullPath))
                {
                    CopyStream(inStream, outStream);
                }
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            // Don't do anything if our streams are null
            if ((input == null) || (output == null)) return;

            // Insert null checking here for production
            var buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        #endregion
    }
}
