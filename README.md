# very-usable-chat

Demo project - server / klient chat application built on .NET 7

## Client

Console app with Terminal.Gui interface.

![image](https://github.com/MichalSkoula/very-usable-chat/assets/5922575/c33299c2-8f57-46d2-9a14-fa2a2fc1f5f3)

### Run it:
- ```dotnet run```
- You must change server URL in ```App.cs``` - ```new SaveData("your.sever.url")```

## Server - ASP.NET Minimal API 

### Run it:

- ```dotnet run```
- Published app should be copied to your webserver folder, for example: `/var/www/DOMAIN`
- Database is stored in `/var/www/DOMAIN/data/vuc.db` (production) or in `Environment.SpecialFolder.LocalApplicationData` (development)
- How to run it on linux server: https://www.printables.com/cs/model/163302-worlds-best-css-developer-trophy
- Swagger API docs: /swagger/index.html
- vhost / reverse proxy example:

```
<VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://0.0.0.0:5000/
    ProxyPassReverse / http://0.0.0.0:5000/

    ServerAdmin webmaster@localhost
    ServerName DOMAIN
    ServerAlias www.DOMAIN
    DocumentRoot /var/www/DOMAIN
    ErrorLog ${APACHE_LOG_DIR}/error.log
    CustomLog ${APACHE_LOG_DIR}/access.log combined

    #RewriteEngine on
    #RewriteRule ^(.*)$ https://%{HTTP_HOST}$1 [R=301,L]
</VirtualHost>
```
