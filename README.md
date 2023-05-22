# very-usable-chat

Demo project - cross-platform server + client chat application.

## Client - .NET console app with Terminal.Gui user interface

![image](https://github.com/MichalSkoula/very-usable-chat/assets/5922575/c33299c2-8f57-46d2-9a14-fa2a2fc1f5f3)

### Run it:

- copy vuc.example.json to vuc.json
- change ```Server``` url
- ```dotnet run```

## Server - ASP.NET Minimal API with EF Core and SQLite DB

### Development:

- create db: ```dotnet ef database update``` (creates db in ./data/vuc.db) OR just rename vuc.example.db
- run it: ```dotnet run```
- Swagger API docs: /swagger/index.html


### Production:

- Published app should be copied to your webserver folder, for example: `/var/www/your.server.domain/`
- SQLite database file is stored in `/var/www/your.server.domain/data/vuc.db`
- How to run it on linux server: https://www.printables.com/cs/model/163302-worlds-best-css-developer-trophy
- vhost / reverse proxy example:

```
<VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://0.0.0.0:5000/
    ProxyPassReverse / http://0.0.0.0:5000/

    ServerAdmin webmaster@localhost
    ServerName your.server.domain
    DocumentRoot /var/www/your.server.domain
    ErrorLog ${APACHE_LOG_DIR}/error.log
    CustomLog ${APACHE_LOG_DIR}/access.log combined
</VirtualHost>
```
