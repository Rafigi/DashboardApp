# DashboardApp
**For at denne solution kan køre, skal der tilføjes nogle settings i AppSettings.json i forskellige projekter.**
<br>
<br>
I Dashboard projektet skal der tilføjet setting for at kunne kommunikere med FTP-server.
OBS! FtpServerURL skal ikke slutte med tegnet "/".
<br>
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
  
  
  
