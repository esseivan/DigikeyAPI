# DigikeyAPI
Digikey API usage

Requirements for getting tokens :
## 1. SSL Certificate

In VS Developer PowerShell as Administrator :

 ```$ New-SelfSignedCertificate -KeyExportPolicy Exportable -CertStoreLocation "Cert:\LocalMachine\My" -KeySpec Signature -KeyLength 2048 -KeyAlgorithm RSA -Subject "<your_cert_name>"```

 This command also output the certhash (thumbprint) in the command output. Otherwise with those steps :
 To find Certhash, do Win+R => mmc => Ctrl + M => Certificates => Computer Account ; In the list : Personnal => Select your certificate => Details => Scroll down to Thumbprint
 
 ```$ netsh http add sslcert ipport=0.0.0.0:<your_port> certhash=<your_certhash> appid='{<your_appid>}'```
 
 The port I used is ```36220```
 
 Note that there is no requirements on the appid other than to be a valid GUID and is only used for you to identify the sslcert
 
## 2. netsh urlacl

 The application can do this on its own but require admin privileges to do so.
 see ApiClientWrapper.ApiClientWrapper.RegisterListener
 
 In the cmd prompt in admin  run : 
 
 ```$ netsh http add urlacl url="<your_listening_url>" user=everyone```
 
 An example for the listening url is ```https:\\+:36220\OAuth2\```
 
 This url MUST be the same as the callback one set in your digikey API application
 
## 3. Fill in apiclient.config
 ClientID and ClientSecret are both from your Digikey application
 
 Redirect URI must be the same domain and be https, as the one specified in your digikey application apart from what is.
 e.g. I used ```https://localhost:36220/OAuth2/``` for redirecting and ```https://localhost``` as the callback URL for the app.
 But it's best to have the exact same ones
 
 Listen URI is the one used to listen the callback and must be the same as the one specified in the step 2
 
 The other three parameters are to leave blank, they are the tokens used to make API calls and the expiration date
