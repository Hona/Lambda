# Archived, pending total rewrite using [TempusHub API](https://tempushub.xyz/swagger/index.html)

---

# Lambda
Lambda bot is a discord bot using the [Discord.Net](https://github.com/discord-net/Discord.Net) library. It has several features including general commands, and discord administration. The bot also implements several [Tempus](https://tempus.xyz) API commands, and provides Source server commands (CSGO, TF2, etc). The bot is used in [SimplyTF](https://simplytf.com/) to provide server status' and integration with its databases.
## Getting Started

If you want to add the bot, or modify and host your own version of the bot follow the instructions below

### Adding the Bot
Add the bot using the [invite link](https://discordapp.com/api/oauth2/authorize?client_id=556347120718708736&permissions=19456&scope=bot). Make sure to check what permissions you want the bot to have (read/send messages/manage messages etc). To make sure your server is as safe as possible ensure that the bot only has elevated privileges in the channels it uses.
### Modifying the code
Make sure to remove all use of the SimplyTF database - including the `database.txt` file.


### Deploying to a Host
Ensure the projects are targeting .NET Core, then select `Build > Publish`.  Copy the files to your server (Use SFTP). Create a file called `lambda.sh` then add the following lines to start your bot in a separate screen without attaching.
```bash
#!/bin/bash
cd ~/lambdaBot # Change this to the directory you put the bot files in
screen -d -m -S lambda dotnet LambdaUI.dll
```


## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the code of conduct, and the process for submitting pull requests. First time contributors are encouraged to submit pull requests.

## Authors

* **Luke Parker** - *Main contributor* - [TheRealHona](https://github.com/TheRealHona)

See also the list of [contributors](https://github.com/TheRealHona/Lambda/contributors) who participated in this project.

## License

This project is licensed under the Apache 2.0 License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [QueryMaster](https://archive.codeplex.com/?p=querymaster) - edited this repository
* [Minecraft Server Ping](https://gist.github.com/csh/2480d14fbbb33b4bbae3) - based code off this file
