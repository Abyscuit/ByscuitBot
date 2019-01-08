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
<b><i>Server Stuff</i></b><br>
roles: Displays your roles or others roles with @username<br>
kick: Kick @username 'reason'<br>
ban: ban @username days 'reason'<br>
unban: unban @username<br>
addrole: adds a role to a user | addrole @username role_name<br>
addroles: adds roles to a user | addrole @username role_name, role_name, role_name<br>
removerole: remove a role from a user<br>
removeallroles: removes all roles from a user<br>
mute: Mutes a user<br>
unmute: Unmute a user<br>
move: Move a user to a Voice Channel<br>
stats: Get stats of a user<br>
level: Get account level of user<br>
addxp: Add XP to a user<br>
subxp: Remove XP to a user<br>
warn: Warn a user<br>
serverstats: Displays server the servers currents stats<br>
nickname: Change nickname of a user<br>
invite: Get an Invite Link for the server<br>
<br>
<b><i>Media Stuff</i></b><br>
youtube: Search YouTube for a keyword<br>
select: Select an option displayed<br>
meme: Post a random or specific meme | Usage: meme <optional> keyword<br>
creatememe: Create a meme using a members avatar | Usage: creatememe @user <top text,bottom text><br>
<br>
<b><i>Steam Commands</i></b><br>
resolve: Resolve steam URL or username to SteamID64<br>
steaminfo: Get Account Summary of your linked account or a steam user<br>
steambans: Get steam account bans (VAC, community, economy)<br>
steamgames: Get All steam games or time played for a specific game<br>
linksteam: Link steam account to Discord Account<br>
steamaccts: Get all steam accounts linked to Discord users<br>
csgostats: Get relevant CS:GO stats (totals, last match)<br>
csgolastmatch: Get CS:GO last match info<br>
csgolastmatches: Get CS:GO info for up to 10 games of your last matches saved<br>
csgowepstats: Get CS:GO weapon stats of all weapons or a specific weapon<br>
  <br>
  <b><i>Server Configuration</i></b><br>
prefix: Set the Prefix for the bot<br>
color: Set the color of the embed message using 0-255. Usage: color r g b<br>
footer: Set the footer text of the embed message<br>
servername: Change the server's name<br>
timestamp: Enable or disable timestamp on the embed messages<br>
afkchannel: Set the afk channel for the server<br>
afktimeout: Set the time in minutes (1, 5, 15, 30, 60) of inactivity for users to be moved to the AFK channel<br>
cmdblacklist: Add a channel to the blacklist for the bot commands<br>
memeblacklist: Add a channel to the meme blacklist<br>
miningwhitelist: Add a channel to the mining whitelist<br>
verifyrole: Set the default role for verified members | Usage: verifyrole <@role><br>
verification: Enable/Disable Verification when a user joins | Usage: verification <true|false><br>
<br>
<b><i>Cryptocurrency Commands</b></i><br>
nanopool: Get NanoPool general account info | Usage: nanopool <optional:address><br>
linketh: Link an ETH address to your discord account | Usage: linketh <address><br>
ethbal: Get the balance of an address or yours | Usage: ethbal <optional:address><br>
topcrypto: Get the top 10 cryptocurrencies by market cap<br>
calceth: Calculate how much ETH you can mine using your Mh/s<br>
ethtokens: Get ERC-20 Tokens linked to Ethereum address | Usage: ethtokens <optional:address><br>
crypto: Get basic price info of one or more coins use commas to separate | Usage: crypto btc,eth,bch<br>
<br>
<b><i>Twitch Commands</b></i><br>
twitchuser: Get Twitch user details by user name | Usage: twitchuser <username><br>
twitchfollowers: Get followers from a Twitch user by ID | Usage: twitchfollowers <id><br>
