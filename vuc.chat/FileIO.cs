using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vuc.chat;

public class FileIO
{
    public string File { get; set; }
    public string Folder { get; private set; }
    private string FullPath
    {
        get { return Path.Combine(this.Folder, File); }
    }

    public FileIO(string file = null)
    {
        this.File = file;

        this.Folder = "";/* Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "VUC");

        if (!Directory.Exists(this.Folder))
        {
            Directory.CreateDirectory(this.Folder);
        }
        */
    }

    public void Load()
    {
        if (System.IO.File.Exists(this.FullPath))
        {
            string json = System.IO.File.ReadAllText(FullPath);
            System.Diagnostics.Debug.WriteLine(json.ToString());

            try
            {
                dynamic data = JObject.Parse(json);

                if (data.ContainsKey("Server"))
                {
                    App.SaveData.Server = data.Server;
                }
                if (data.ContainsKey("RoomId"))
                {
                    App.SaveData.RoomId = data.RoomId;
                }
                if (data.ContainsKey("UserName"))
                {
                    App.SaveData.UserName = data.UserName;
                }
                if (data.ContainsKey("Password"))
                {
                    App.SaveData.Password = data.Password;
                }
            }
            catch (Exception e)
            {

            }
        }
    }

    public void Save(object data)
    {
        string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        System.IO.File.WriteAllText(this.FullPath, json);
    }

    public void Delete()
    {
        System.IO.File.Delete(this.FullPath);
    }
}