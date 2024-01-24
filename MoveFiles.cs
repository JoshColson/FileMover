using System;
using System.Diagnostics;
using System.IO;

namespace FileMover
{

    class MoveFiles
    {
        public static ReturnStatements Move(InputData input, MyForm instance, RadioSelection radioSelection)
        {
            if (input.sourcePath == null) { return ReturnStatements.NoSourcePath; }
            if (input.destinationPath == null) { return ReturnStatements.NoDestinationPath; }

            Debug.WriteLine("\nSource path chosen: "+input.sourcePath);
            Debug.WriteLine("Destination path chosen: " + input.destinationPath+"\n");

            DirectoryInfo sourceDir = new DirectoryInfo(input.sourcePath);
            DirectoryInfo destDir = new DirectoryInfo(input.destinationPath);

            var destFiles = destDir.GetFiles("*.*", SearchOption.AllDirectories)
                .Select(file => file.FullName.Replace(destDir.FullName, ""))
                .ToList();

            int currentFileIndex = 1;
            foreach (var sourceFile in sourceDir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                FileInfo destFile = new FileInfo(sourceFile.FullName.Replace(sourceDir.FullName, destDir.FullName));

                if (destFile.Exists)
                {
                    if (sourceFile.LastWriteTime > destFile.LastWriteTime)
                    {
                        Debug.WriteLine("Updating file: {0}", destFile.FullName);
                        sourceFile.CopyTo(destFile.FullName, true);
                    }
                }
                else
                {
                    Debug.WriteLine("Moving file: {0}", sourceFile.FullName);
                    Directory.CreateDirectory(destFile.DirectoryName);
                    sourceFile.CopyTo(destFile.FullName);
                }
                destFiles.Remove(destFile.FullName.Replace(destDir.FullName, ""));
                int totalFiles = sourceDir.GetFiles("*.*", SearchOption.AllDirectories).Length;
                int percentage = (int)(((double)currentFileIndex / totalFiles) * 100);
                instance.ProgressBarUpdate(percentage);
                currentFileIndex++;
            }

            if (radioSelection == RadioSelection.MoveIdenticalWithCleanup)
            {
                var destFiles = destDir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (var destFile in destFiles)
                {
                    string relativePath = destFile.FullName.Replace(destDir.FullName, "");
                    FileInfo sourceFile = new FileInfo(Path.Combine(sourceDir.FullName, relativePath));
                    if (!sourceFile.Exists)
                    {
                        Debug.WriteLine("Deleting file: {0}", destFile.FullName);
                        destFile.Delete();
                    }
                }

                var destDirs = destDir.GetDirectories("*.*", SearchOption.AllDirectories);
                foreach (var destDirectory in destDirs)
                {
                    string relativePath = destDirectory.FullName.Replace(destDir.FullName, "");
                    DirectoryInfo sourceDirectory = new DirectoryInfo(Path.Combine(sourceDir.FullName, relativePath));
                    if (!sourceDirectory.Exists)
                    {
                        Debug.WriteLine("Deleting directory: {0}", destDirectory.FullName);
                        destDirectory.Delete(true);
                    }
                }
            }

            return ReturnStatements.Success;
        }
    }


    public class InputData
    {
        public string destinationPath { get; set; }
        public string sourcePath { get; set; }
    }

    public enum ReturnStatements
    {
        NoSourcePath,
        NoDestinationPath,
        Success,
        UnknownFailure

    }
}