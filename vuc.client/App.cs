using System.Diagnostics;
using Terminal.Gui;
using vuc.client.Classes;
using static Terminal.Gui.View;

namespace vuc.client;

public class App
{
    private static FileIO saveFile = new FileIO("vuc.json");
    public static SaveData SaveData = new SaveData("http://localhost:5049/");
    private MenuBar loggedMenu = new MenuBar();
    private MenuBar anonymMenu = new MenuBar();
    private List<Room> rooms = new();
    private bool runTask = false;

    public App()
    {
        Application.Init();
        this.CreateMenus();

        saveFile.Load();

        this.WelcomeScreen();
        Application.Shutdown();
    }

    private void CreateMenus()
    {
        loggedMenu.Menus = new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Rooms", "", () => {
                    RoomsScreen();
                }),
                new MenuItem ("_Create a new room", "", () => {
                    RoomCreateScreen();
                }),
                new MenuItem ("_Logout", "", () => {
                    LogoutScreen();
                }),
                new MenuItem ("_Quit", "", () => {
                    Application.RequestStop ();
                })
            }),
        };

        anonymMenu.Menus = new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Login", "", () => {
                    LoginScreen();
                }),
                new MenuItem ("_Register", "", () => {
                    RegisterScreen();
                }),
                new MenuItem ("_Quit", "", () => {
                    Application.RequestStop ();
                })
            }),
        };
    }

    private void RegisterScreen()
    {
        runTask = false;

        var window = new Window("Register") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

        window.Add(new Label("Username: ") { X = 1, Y = 1, Width = 20, Height = 1 });
        TextField UserName = new TextField("") { X = 1, Y = 2, Width = 30, Height = 1 };
        window.Add(UserName);

        window.Add(new Label("Password: ") { X = 1, Y = 4, Width = 20, Height = 1 });
        TextField Password = new TextField("") { X = 1, Y = 5, Width = 30, Height = 1, Secret = true };
        window.Add(Password);

        Button btnRegister = new Button("Register") { X = 1, Y = 7, };
        btnRegister.Clicked += () =>
        {
            Response response = APIClient.Register((string)UserName.Text, (string)Password.Text);
            if (response.Success)
            {
                MessageBox.Query("Registration Successful", "Registration Successful!", "Ok");
                LoginScreen();
            }
            else
            {
                MessageBox.ErrorQuery("Registration Error", response.Message, "Ok");
            }
        };
        window.Add(btnRegister);

        window.KeyDown += (KeyEventEventArgs args) =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                WelcomeScreen();
            }
        };

        Application.Top.RemoveAll();
        Application.Top.Add(anonymMenu, window);
    }

    private void LoginScreen()
    {
        runTask = false;

        var window = new Window("Login") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

        window.Add(new Label("Username: ") { X = 1, Y = 1, Width = 20, Height = 1 });
        TextField UserName = new TextField("") { X = 1, Y = 2, Width = 30, Height = 1 };
        window.Add(UserName);

        window.Add(new Label("Password: ") { X = 1, Y = 4, Width = 20, Height = 1 });
        TextField Password = new TextField("") { X = 1, Y = 5, Width = 30, Height = 1, Secret = true };
        window.Add(Password);

        Button btn = new Button("Login") { X = 1, Y = 7, };
        btn.Clicked += () =>
        {
            bool success = APIClient.Login((string)UserName.Text, (string)Password.Text);
            if (success)
            {
                SaveData.UserName = (string)UserName.Text;
                SaveData.Password = (string)Password.Text;

                saveFile.Save(SaveData);
                MessageBox.Query("Login Successful", "Login sucessfull.", "Ok");
                RoomsScreen();
            }
            else
            {
                MessageBox.ErrorQuery("Registration Error", "Cannot login", "Ok");
            }
        };
        window.Add(btn);

        window.KeyDown += (KeyEventEventArgs args) =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                WelcomeScreen();
            }
        };

        Application.Top.RemoveAll();
        Application.Top.Add(anonymMenu, window);
    }

    private void LogoutScreen()
    {
        runTask = false;

        saveFile.Delete();
        SaveData.UserName = null;
        SaveData.Password = null;
        SaveData.RoomId = null;
        WelcomeScreen();
    }

    private void RoomsScreen()
    {
        runTask = false;

        var window = new Window("Choose a chat room | User " + SaveData.UserName) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

        rooms = APIClient.GetRooms();

        if (!rooms.Any())
        {
            RoomCreateScreen();
            return;
        }

        ListView list = new ListView(rooms) { X = 0, Y = 0, Width = 30, Height = Dim.Fill() - 2 };

        list.OpenSelectedItem += (ListViewItemEventArgs item) =>
        {
            RoomScreen(item);
        };

        window.Add(list);

        Application.Top.RemoveAll();
        Application.Top.Add(loggedMenu, window);
    }

    private void RoomCreateScreen()
    {
        runTask = false;

        var window = new Window("Create a new chat room | User " + SaveData.UserName) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

        window.Add(new Label("Title: ") { X = 1, Y = 1, Width = 20, Height = 1 });
        TextField RoomTitle = new TextField("") { X = 1, Y = 2, Width = 30, Height = 1 };
        window.Add(RoomTitle);

        Button btn = new Button("Create") { X = 1, Y = 4, };
        btn.Clicked += () =>
        {
            Response response = APIClient.CreateRoom((string)RoomTitle.Text);
            if (response.Success)
            {
                MessageBox.Query("Room created", "Room created", "Ok");
                RoomsScreen();
            }
            else
            {
                MessageBox.ErrorQuery("Room creation error", response.Message, "Ok");
            }
        };
        window.Add(btn);

        // esc key - go back to rooms
        window.KeyDown += (KeyEventEventArgs args) =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                RoomsScreen();
            }
        };

        Application.Top.RemoveAll();
        Application.Top.Add(loggedMenu, window);
    }

    private void RoomScreen(ListViewItemEventArgs item)
    {
        runTask = true;

        // item.Item is index in rooms list (not an actual id)
        int roomId = rooms.ElementAtOrDefault(item.Item).RoomId;

        var window = new Window("Room: " + item.Value + " | User " + SaveData.UserName) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

        TextView content = new TextView() { X = 1, Y = 3, Width = Dim.Fill() - 1, Height = Dim.Fill() - 1 };
        content.ReadOnly = true;
        content.WordWrap = true;
        window.Add(content);

        TextField newMessage = new TextField("") { X = 1, Y = 1, Width = Dim.Percent(70), Height = 1 };
        window.Add(newMessage);
        newMessage.SetFocus();

        Button btn = new Button("Post") { X = Pos.Percent(80), Y = 1, Width = Dim.Percent(20) };
        btn.Clicked += () =>
        {
            if (((string)newMessage.Text).Trim() == "")
            {
                return;
            }

            newMessage.ReadOnly = true;
            Response response = APIClient.PostMessage(roomId, (string)newMessage.Text);
            newMessage.ReadOnly = false;

            if (response.Success)
            {
                newMessage.Text = "";
            }
            else
            {
                MessageBox.ErrorQuery("An error occured", response.Message, "Hmm");
            }

            newMessage.SetFocus();
        };
        window.Add(btn);

        // enter key - post
        window.KeyDown += (KeyEventEventArgs args) =>
        {
            if (args.KeyEvent.Key == Key.Enter)
            {
                btn.OnClicked();
            }
        };

        // periodically update content 
        var task = Task.Run(async () =>
        {
            for (; ; )
            {
                if (!runTask)
                {
                    throw new OperationCanceledException();
                }

                var messages = APIClient.GetMessages(roomId);
                string text = "";
                foreach (var message in messages)
                {
                    text += message.InsertedAt.TimeOfDay + "\t" + message.User.UserName + "\n";
                    text += message.Content + "\n\n";
                }

                // this must be done in mainloop to be immediately reflected in the UI
                Application.MainLoop.Invoke(() =>
                {
                    content.Text = text;
                });

                Debug.WriteLine("Fetched 3 seconds");
                await Task.Delay(3000);
            }
        });

        // esc - go back
        window.KeyDown += (KeyEventEventArgs args) =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                RoomsScreen();
            }
        };

        Application.Top.RemoveAll();
        Application.Top.Add(loggedMenu, window);
    }

    private void WelcomeScreen()
    {
        runTask = false;

        if (SaveData.UserName != null)
        {
            this.RoomsScreen();
        }
        else
        {
            var win = new Window("Velcome to VUC (Very Usable Chat)")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.RemoveAll();
            Application.Top.Add(win, anonymMenu);
        }

        Application.Run();
    }
}
