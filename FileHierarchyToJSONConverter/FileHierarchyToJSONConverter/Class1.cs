using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace FileHierarchy
{
    /// <summary>
    /// Represents a file in the directory. Contains name, size and path of the file.
    /// </summary>
    [DataContract]
    public class FilePresentation
    {
        /// <summary>
        /// Gets the name of a current instance of the FileHierarchy.File class.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the size of a current instance of the FileHierarchy.File class.
        /// </summary>
        [DataMember]
        public long Size { get; private set; }

        /// <summary>
        /// Gets the path of a current instance of the FileHierarchy.File class.
        /// </summary>
        [DataMember]
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the FileHierarchy.File class, which  has initial values by default.
        /// </summary>
        public FilePresentation() { }

        /// <summary>
        /// Initializes a new instance of the FileHierarchy.File class with value in parameters.
        /// 
        /// Parameters:
        ///     name:
        ///         A name of a current instance of the FileHierarchy.File class.
        ///     size:
        ///         A Size of a current instance of the FileHierarchy.File class.
        ///     path:
        ///         A path of a current instance of the FileHierarchy.File class.
        /// </summary>
        public FilePresentation(string name, long size, string path)
        {
            Name = name;
            Size = size;
            Path = path;
        }
    }
    /// <summary>
    /// Represents the folder in the directory. Contains name, date of creation, inner files and folders of the current folder.
    /// </summary>
    [DataContract]
    public class Folder
    {
        /// <summary>
        /// Gets the name of the current instance of the FileHierarchy.Folder class.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the date of creation of the current instance of the FileHierarchy.Folder class.
        /// </summary>
        [DataMember]
        public string DateCreated { get; private set; }

        [DataMember]
        private List<FilePresentation> filesList = null;

        [DataMember]
        private List<Folder> foldersList = null;

        /// <summary>
        /// Initializes a new instance of the FileHierarchy.Folder class, which has initial values by default.
        /// </summary>
        public Folder() { }

        /// <summary>
        /// Initializes a new instance of the FileHierarchy.Folder class with value in parameters.
        /// 
        /// Parameters:
        ///     name:
        ///         A name of the current instance of the FileHierarchy.Folder class.
        ///     dateCreated:
        ///         A date of creation of the current instance of the FileHierarchy.Folder class.
        /// </summary>
        public Folder(string name, string dateCreated)
        {
            Name = name;
            DateCreated = dateCreated;
        }

        /// <summary>
        /// Adds another instance of the FileHierarchy.Folder class as a child to the current instance.
        /// 
        /// parameters:
        ///      folder:
        ///         An instance of the FileHierarchy.Folder class, which is need to add to the current 
        ///         instance as a child.
        /// </summary>
        public void AddFolder(Folder folder)
        {
            if (foldersList == null)
                foldersList = new List<Folder>();
            foldersList.Add(folder);
        }

        /// <summary>
        /// Adds the instance of the FileHierarchy.File class to the current instance.
        /// 
        /// parameters:
        ///      file:
        ///         An inctance of the FileHierarchy.File class, which is need to add to the current instance.
        /// </summary>
        public void AddFile(FilePresentation file)
        {
            if (filesList == null)
                filesList = new List<FilePresentation>();
            filesList.Add(file);
        }

        /// <summary>
        /// Converts the directory to JSON format.
        /// 
        /// parameters:
        ///      path:
        ///         A path to the directory, which has to be converted.
        ///      destinationPath:
        ///         A path to the directory, where the JSON-file has to be saved.
        ///  
        /// exceptions:
        ///      ConvertToJsonMethodException:
        ///         The directory at the specified path does not exist.
        /// 
        /// </summary>
        public static void ToConvertToJson(string path, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
                throw new ConvertToJsonMethodException("The directory for destination JSON-file at the specified path does not exist!");
            Folder folder = ToBuildFileHierarchy(path);
            if (folder != null)
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Folder));
                using (FileStream fs = new FileStream(destinationPath + "\\" + "folderHierarchy.json", FileMode.OpenOrCreate))
                {
                    jsonFormatter.WriteObject(fs, folder);
                }
            }
            else
                throw new ConvertToJsonMethodException("The directory at the specified path does not exist!");

        }

        private static Folder ToBuildFileHierarchy(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo d = new DirectoryInfo(path);
                string name = d.Name;
                string dateCreated = d.CreationTime.ToLongDateString();
                Folder folder = new Folder(name, dateCreated);

                string[] files = Directory.GetFiles(path);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; ++i)
                    {
                        FileInfo fileInf = new FileInfo(files[i]);
                        folder.AddFile(new FilePresentation(fileInf.Name, fileInf.Length, fileInf.DirectoryName));
                    }
                }

                string[] childrenFolders = Directory.GetDirectories(path);
                if (childrenFolders != null)
                {
                    for (int i = 0; i < childrenFolders.Length; ++i)
                    {
                        folder.AddFolder(ToBuildFileHierarchy(childrenFolders[i]));
                    }
                }
                else
                    return null;

                return folder;
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Defines ConvertToJson method exceptions.
    /// </summary>
    public class ConvertToJsonMethodException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of class System.ApplicationException.ConvertToJsonMethodException.
        /// </summary>
        public ConvertToJsonMethodException() { }
        /// <summary>
        /// Initializes a new instance of class System.ApplicationException.ConvertToJsonMethodException 
        /// with the specified error message.
        /// 
        /// Parameters:
        ///     message:
        ///         A message describing the error.
        /// </summary>
        public ConvertToJsonMethodException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of class System.ApplicationException.ConvertToJsonMethodException 
        /// with the specified error message and a link to the internal exception that triggered this exception.
        /// 
        /// Parameters:
        ///     message:
        ///         A message describing the error.
        ///     inner:
        ///         Exception that is the reason for the current exception. If the innerException parameter
        ///         is not a NULL pointer, the current exception occurred in the catch block that is being processed
                ///         internal exception.
        /// </summary>
        public ConvertToJsonMethodException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the System.ApplicationException.ConvertToJsonMethodException class 
        /// with serializations data
        ///
        /// Parameters:
        ///     info:
        ///         Object containing serialized object data.
        ///     context:
        ///         Context information about the source or destination.
        /// </summary>
        protected ConvertToJsonMethodException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

