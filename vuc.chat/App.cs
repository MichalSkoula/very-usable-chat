using Terminal.Gui;

namespace vuc.chat
{
    public class App
    {
        private static FileIO saveFile = new FileIO("vuc.json");
        public static SaveData SaveData = new SaveData("http://localhost:5049/");
        private MenuBar loggedMenu = new MenuBar();
        private MenuBar anonymMenu = new MenuBar();

        public App()
        {
            Application.Init();
            this.CreateMenus();

            saveFile.Load();
            System.Diagnostics.Debug.WriteLine(SaveData.UserName);
            this.WelcomeScreen();
        }

        private void CreateMenus()
        {
            loggedMenu.Menus = new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Rooms", "", () => {
                        RoomsScreen();
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
                Response response = ChatClient.Register((string)UserName.Text, (string)Password.Text);
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

            Application.Top.RemoveAll();
            Application.Top.Add(loggedMenu, window);
            Application.Run();
        }

        private void LoginScreen()
        {
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
                bool success = ChatClient.Login((string)UserName.Text, (string)Password.Text);
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

            Application.Top.RemoveAll();
            Application.Top.Add(anonymMenu, window);
            Application.Run();
        }

        private void LogoutScreen()
        {
            saveFile.Delete();
            SaveData.UserName = null;
            SaveData.Password = null;
            SaveData.RoomId = null;
            WelcomeScreen();
        }

        private void RoomsScreen()
        {
            var window = new Window("Choose chat room | User " + SaveData.UserName) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

            /*

            Button btnRegister = new Button("Register") { X = 1, Y = 7, };
            btnRegister.Clicked += () =>
            {
                Response response = ChatClient.Register((string)UserName.Text, (string)Password.Text);
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
            */
            Application.Top.RemoveAll();
            Application.Top.Add(loggedMenu, window);
            Application.Run();
            Application.Shutdown();
        }

        private void WelcomeScreen()
        {
            System.Diagnostics.Debug.WriteLine(SaveData.UserName);
            if (SaveData.UserName != null)
            {
                this.RoomsScreen();
                return;
            }
            

            var win = new Window("Hello")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.RemoveAll();
            Application.Top.Add(win, anonymMenu);
            Application.Run();
            Application.Shutdown();
        }

    }
}
