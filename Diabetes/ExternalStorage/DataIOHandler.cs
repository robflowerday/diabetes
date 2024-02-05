using System;

using Newtonsoft.Json;

using Diabetes.ExternalStorage.DataModels;
using Diabetes.User.FileIO;


namespace Diabetes.ExternalStorage
{
    public class DataIOHandler<TDataModel> where TDataModel : IDataModel
    {
        private IFileIO _fileIO;
        private readonly string _jsonFilePath;

        public DataIOHandler(string jsonFilePath, IFileIO fileIO)
        {
            _fileIO = fileIO;
            _jsonFilePath = jsonFilePath;
        }

        public TDataModel LoadOrCreateDataModelInstance()
        {
            // Initialize user configuration object
            TDataModel dataModelInstance;
            
            // Check if file exists
            if (_fileIO.Exists(_jsonFilePath))
            {
                // Read in JSON file
                string jsonString = _fileIO.ReadAllText(_jsonFilePath);
                
                // Convert JSON string to C# object
                dataModelInstance = JsonConvert.DeserializeObject<TDataModel>(value: jsonString);
            }
            // If file doesn't exist
            else
            {
                // Create default data model instance
                dataModelInstance = Activator.CreateInstance<TDataModel>();
            }
                
            // Validate property relationships
            dataModelInstance.ValidateDataModelPropertyRelationships();
                
            // If file configuration is valid, return file configuration object
            return dataModelInstance;
        }

        public void SaveDataModelInstanceToFile(TDataModel dataModelInstance)
        {
            // Convert data model instance to json string
            string jsonString = JsonConvert.SerializeObject(value: dataModelInstance, formatting: Formatting.Indented);
            
            // Write json string to file
            _fileIO.WriteAllText(path: _jsonFilePath, contents: jsonString);
        }
    }
}