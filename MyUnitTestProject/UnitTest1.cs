using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static MyUnitTestProject.NativeMethods;

namespace MyUnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            string filePath = BuildFilePathWithLength(TestContext.TestResultsDirectory, 260);

            CreateDummyFile(filePath);

            try
            {
                this.TestContext.AddResultFile(filePath);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        private string BuildFilePathWithLength(string initialPath, int length, char character = 'a')
        {
            const string extension = ".txt";

            if (initialPath.Length + extension.Length - 1 >= length)
            {
                throw new Exception("The current path plus extension length is larger than total length.");
            }

            return $"{initialPath}\\{new string(character, length - initialPath.Length - extension.Length - 1)}{extension}";
        }

        private void CreateDummyFile(string fileName)
        {
            string formattedName = @"\\?\" + fileName;
            
            SafeFileHandle fileHandle = CreateFile(
                formattedName,
                EFileAccess.GenericWrite,
                EFileShare.None,
                IntPtr.Zero,
                ECreationDisposition.CreateAlways,
                0,
                IntPtr.Zero);

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (fileHandle.IsInvalid)
            {
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

            using (FileStream fs = new FileStream(fileHandle, FileAccess.Write))
            {
                fs.WriteByte(80);
                fs.WriteByte(81);
                fs.WriteByte(83);
                fs.WriteByte(84);
            }
        }
    }
}
