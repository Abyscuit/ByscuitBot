# ByscuitBot
<h3>Basic Discord Bot</h3>

<h4>Features</h4>
<ul>
  <li>Antispam</li>
  <li>Meme Generator</li>
  <li>Multiple Server Configuration</li>
  <li>Search YouTube</li>
  <li>Leveling System</li>
  <li>Reward System</li>
  <li>Steam API Calls</li>
  <li>CSGO Stat Calls</li>
  <li>Coin Market Cap API Calls</li>
  <li>EtherScan API Calls</li>
  <li>NanoPool API Calls</li>
  <li>Twitch API Calls</li>
  <li>Welcome/Bye Messages</li>
  <li>User/Bot/Member Count</li>
  <li>Move/Kick/Mute/Ban Members</li>
  <li>Add/Remove Roles</li>
  <li>Warn Users</li>
  <li>Change Nicknames</li>
  <li>Get Invite Link</li>
  <li>Customize Embed</li>
  <li>Verification System</li>
</ul>

<h4>More Features coming</h4>
<ul>
  <li>Captcha</li>
</ul>
<br>
<h4>Setup:</h4>
Put token and Command prefix in the Resources/config.json


<h4>Commands</h4>
<b><i>Server Stuff</i></b>
roles: Displays your roles or others roles with @username
kick: Kick @username 'reason'
ban: ban @username days 'reason'
unban: unban @username
addrole: adds a role to a user | addrole @username role_name
addroles: adds roles to a user | addrole @username role_name, role_name, role_name
removerole: remove a role from a user
removeallroles: removes all roles from a user
mute: Mutes a user
unmute: Unmute a user
move: Move a user to a Voice Channel
stats: Get stats of a user
level: Get account level of user
addxp: Add XP to a user
subxp: Remove XP to a user
warn: Warn a user
serverstats: Displays server the servers currents stats
nickname: Change nickname of a user
invite: Get an Invite Link for the server

<b><i>Media Stuff</i></b>
youtube: Search YouTube for a keyword
select: Select an option displayed
meme: Post a random or specific meme | Usage: meme <optional> keyword
creatememe: Create a meme using a members avatar | Usage: creatememe @user <top text,bottom text>

<b><i>Steam Commands</i></b>
resolve: Resolve steam URL or username to SteamID64
steaminfo: Get Account Summary of your linked account or a steam user
steambans: Get steam account bans (VAC, community, economy)
steamgames: Get All steam games or time played for a specific game
linksteam: Link steam account to Discord Account
steamaccts: Get all steam accounts linked to Discord users
csgostats: Get relevant CS:GO stats (totals, last match)
csgolastmatch: Get CS:GO last match info
csgolastmatches: Get CS:GO info for up to 10 games of your last matches saved
csgowepstats: Get CS:GO weapon stats of all weapons or a specific weapon
Developed with CARE ❤️•Today at 3:38 PM
Help/Commands
Server Configuration
prefix: Set the Prefix for the bot
color: Set the color of the embed message using 0-255. Usage: color <r> <g> <b>
footer: Set the footer text of the embed message
servername: Change the server's name
timestamp: Enable or disable timestamp on the embed messages
afkchannel: Set the afk channel for the server
afktimeout: Set the time in minutes (1, 5, 15, 30, 60) of inactivity for users to be moved to the AFK channel
cmdblacklist: Add a channel to the blacklist for the bot commands
memeblacklist: Add a channel to the meme blacklist
miningwhitelist: Add a channel to the mining whitelist
verifyrole: Set the default role for verified members | Usage: verifyrole <@role>
verification: Enable/Disable Verification when a user joins | Usage: verification <true|false>

<b><i>Cryptocurrency Commands</b></i>
nanopool: Get NanoPool general account info | Usage: nanopool <optional:address>
linketh: Link an ETH address to your discord account | Usage: linketh <address>
ethbal: Get the balance of an address or yours | Usage: ethbal <optional:address>
topcrypto: Get the top 10 cryptocurrencies by market cap
calceth: Calculate how much ETH you can mine using your Mh/s
ethtokens: Get ERC-20 Tokens linked to Ethereum address | Usage: ethtokens <optional:address>
crypto: Get basic price info of one or more coins use commas to separate | Usage: crypto btc,eth,bch

<b><i>Twitch Commands</b></i>
twitchuser: Get Twitch user details by user name | Usage: twitchuser <username>
twitchfollowers: Get followers from a Twitch user by ID | Usage: twitchfollowers <id>
