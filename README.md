# DashboardApp
**For at denne solution kan køre, skal der tilføjes nogle settings i AppSettings.json i forskellige projekter.**
<br>
<br>
I Dashboard projektet skal der tilføjet setting for at kunne kommunikere med FTP-server, samt connectionString for databasen.
OBS! FtpServerURL skal ikke slutte med tegnet "/".
<br>
 "ConnectionStrings": {
    "DefaultConnection": ""
  },
"FtpSettings": {
    "Username": "",
    "Password": "",
    "FtpServerUri": ""
  }
  
  
  I WeatherServiceAPI skal der tilføjes et password i  Settings for a kommunikere med SoapServicen.
<br>
   "SOAPSettings": {
    "Password": ""
  }
  
  
  Hvis API ikke virker, tjek om porten i launchsettings i Dashboard projektet til 44320.
  
  
![image](https://user-images.githubusercontent.com/36636158/160621135-243f8d8b-246e-4479-8fd0-5c4b2df482f5.png)


  
