using UnityEngine;
using System.IO;

public class SaveManager
{
    private readonly string _path;

    public SaveManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        _path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "/LevelData.json";
#else
        _path = Application.dataPath + "/_Workspace/Saves/" + "LevelData.json";
#endif
    }

    public void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data);

        using (var writer = new StreamWriter(_path))
        {
            writer.WriteLine(json);
        }
    }

    public SaveData Load()
    {
        if (!File.Exists(_path))
            return new SaveData();

        string json = "";

        using (var reader = new StreamReader(_path))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                json += line;
            }

            if (string.IsNullOrEmpty(json))
            {
                return new SaveData();
            }

            return JsonUtility.FromJson<SaveData>(json);
        }
    }
}
