# Telegram Tenor GIF Search 
Bot to search for Tenor GIFs in Telegram<br>
Supports both versions of Tenor API (api.tenor.com/v1 and tenor.googleapis.com/v2)<br><br>
You can find hosted version on [@tenrgifbot](https://t.me/@tenrgifbot) in Telegram<br>

# Known Tenor API keys
| Source | Key | client_key | API Version |
| --- | --- | --- | --- |
| Gboard for iOS | 3Z0688EVWYKH | - | v1 |
| Gboard for Android | AIzaSyAyimkuYQYF_FXVALexPuGQctUWRURdCYQ | gboard | v2 |
| Tenor Website | AIzaSyCZt6SSh5VgVPzD9fhyzG1DprdPRhtoaR4 | tenor_web | v2 |

# Running
Compile solution and place following configuration file with the name ``config.json`` next to the executable files
```
{
    "token": "",
    "key": "",
    "client_key": "",
    "dev_id": 0
}
```
### Scheme
- token (string): Telegram Bot token
- key (string): Tenor API key from the table above
- client_key (string): Tenor Client key from the table above (leave blank for v1 keys)
- dev_id (long): Your Telegram ID (for the debug info in /start message)

# Credits
[Heavily inspired by this Vencord plugin](https://github.com/Vendicated/Vencord/tree/main/src/plugins/tenorGifSearch)

###### Written on CSharp because i don't know any other languages <3