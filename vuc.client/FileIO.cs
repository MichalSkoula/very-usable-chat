using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vuc.client;

public class FileIO
{
    public string File { get; set; }
    public string Folder { get; private set; }
    public string FullPath
    {
        get { return Path.Combine(this.Folder, File); }
    }

    public FileIO(string file = null)
    {
        this.File = file;
        this.Folder = ".";
        //this.Folder = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "vuc");

        if (this.Folder != "." && !Directory.Exists(this.Folder))
        {
            Directory.CreateDirectory(this.Folder);
        }
    }

    public void Load()
    {
        if (System.IO.File.Exists(this.FullPath))
        {
            string json = System.IO.File.ReadAllText(FullPath);
            System.Diagnostics.Debug.WriteLine(json.ToString());

            dynamic data = JObject.Parse(json);

            // Server property is required
            if (data.ContainsKey("Server") && ((string)data.Server).Length > 0 && Uri.IsWellFormedUriString((string)data.Server, UriKind.Absolute))
            {
                App.SaveData.Server = data.Server;
            }
            else
            {
                Console.WriteLine("vuc.json configuration file is missing the \"Server\" property or it is not valid URI");
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (data.ContainsKey("UserId"))
            {
                App.SaveData.UserId = data.UserId;
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
        else
        {
            Console.WriteLine("vuc.json configuration file not found");
            Console.ReadKey();
            Environment.Exit(0);
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