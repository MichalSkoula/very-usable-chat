# very-usable-chat

## client

## server 

https://medium.com/@sebishenzenn/running-ours-asp-net-core-apps-using-apache-server-with-reverse-proxy-c0784ae7babd

Swagger API docs: /swagger/index.html

Published app should be copied to your vhost folder, for example: `/var/www/DOMAIN`

Database is stored in `/var/www/DOMAIN/data/vuc.db` (production) or in `Environment.SpecialFolder.LocalApplicationData` (development)

vhost / reverse proxy example:

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
