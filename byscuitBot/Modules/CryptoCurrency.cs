using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using byscuitBot.Core;
using Discord.WebSocket;
using byscuitBot.Core.Steam_Accounts;
using byscuitBot.Core.Server_Data;

namespace byscuitBot.Modules
{
    public class CryptoCurrency : ModuleBase<SocketCommandContext>
    {
        #region CryptoCurrency
        public string GetNanoPoolInfo(string address, SocketUser user)
        {
            NanoPool.Account account = NanoPool.GetAccount(address, user);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found!";
                return msg;
            }


            msg += "**Account:** " + account.account;
            msg += "\n**Unconfirmed Balance:** " + account.unconfirmed_balance;
            msg += "\n**Balance:** " + account.balance;
            double balance = double.Parse(account.balance);
            NanoPool.Prices prices = NanoPool.GetPrices();
            double usdBal = prices.price_usd * balance;
            double btcBal = prices.price_btc * balance;
            double gbpBal = prices.price_gbp * balance;
            double eurBal = prices.price_eur * balance;
            msg += "\n**USD Value:** " + string.Format("${0:N2}", usdBal);
            msg += "\n**BTC Value:** " + string.Format("{0:N8} BTC", btcBal);
            msg += "\n**Hashrate:** " + account.hashrate + " Mh/s";
            msg += "\n\n__Average Hashrate__";
            msg += "\n**1 Hour:** " + account.avgHashrate["h1"] + " Mh/s";
            msg += "\n**3 Hours:** " + account.avgHashrate["h3"] + " Mh/s";
            msg += "\n**6 Hours:** " + account.avgHashrate["h6"] + " Mh/s";
            msg += "\n**12 Hours:** " + account.avgHashrate["h12"] + " Mh/s";
            msg += "\n**24 Hours:** " + account.avgHashrate["h24"] + " Mh/s";
            msg += "\n\n__Workers__";
            for (int i = 0; i < account.workers.Count; i++)
            {
                if (i > 0)
                    msg += "\n";
                msg += "\n**ID:** " + account.workers[i].id;
                msg += "\n**UID:** " + account.workers[i].uid;
                msg += "\n**Hashrate:** " + account.workers[i].hashrate + " Mh/s";
                msg += "\n**Last Share:** " + SteamAccounts.UnixTimeStampToDateTime(account.workers[i].lastshare);
                msg += "\n**Rating:** " + account.workers[i].rating;
                /*
                msg += "\n**1hr Avg Hashrate:** " + account.workers[i].h1 + " Mh/s";
                msg += "\n**3hr Avg Hashrate:** " + account.workers[i].h3 + " Mh/s";
                msg += "\n**6hr Avg Hashrate:** " + account.workers[i].h6 + " Mh/s";
                msg += "\n**12hr Avg Hashrate:** " + account.workers[i].h12 + " Mh/s";
                msg += "\n**24hr Avg Hashrate:** " + account.workers[i].h24 + " Mh/s";
                */
            }
            return msg;
        }

        [Command("nanopool")]
        [Summary("Get NanoPool general account info - Usage: {0}nanopool <optional:@user>")]
        public async Task Nanopool(SocketUser user)
        {
            NanoPool.UserAccount userAccount = NanoPool.GetUser(user);
            string address = userAccount.address;

            await PrintEmbedMessage("NanoPool Account", GetNanoPoolInfo(address, user));
        }
        [Command("nanopool")]
        [Summary("Get NanoPool general account info - Usage: {0}nanopool <optional:address>")]
        public async Task Nanopool([Remainder]string address = null)
        {
            await PrintEmbedMessage("NanoPool Account", GetNanoPoolInfo(address, Context.User));
        }

        [Command("linketh")]
        [Summary("Link an ETH address to your discord account - Usage: {0}linketh <address>")]
        public async Task LinkEth([Remainder]string address)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found!";
                await PrintEmbedMessage("ETH Link Failed", msg);
                return;
            }
            if (account.discordID != Context.User.Id)
            {
                msg += "**Address:** " + account.address;
                msg += "\n**Already Linked:** " + account.discordUsername;
            }
            else
            {
                msg += "**Address:** " + account.address;
                msg += "\n**Now Linked:** " + account.discordUsername;
            }

            await PrintEmbedMessage("ETH Address Linked", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }


        [Command("ethbal")]
        [Summary("Get the balance of an address or yours - Usage: {0}ethbal <optional:address>")]
        public async Task Ethbal([Remainder]string address = null)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found! Please use linketh cmd or enter an address to link!";
                await PrintEmbedMessage("Account Retrieval Failed", msg);
                return;
            }
            if (address == null)
                address = account.address;


            double balance = double.Parse(EtherScan.GetBalance(address)) / 1000000000000000000d;
            NanoPool.Prices prices = NanoPool.GetPrices();
            double usdBal = prices.price_usd * balance;
            double btcBal = prices.price_btc * balance;
            double gbpBal = prices.price_gbp * balance;
            double eurBal = prices.price_eur * balance;
            msg += "**Account:** " + address;
            msg += "\n**Balance:** " + string.Format("{0:N15} ETH", balance);
            msg += "\n**USD Value:** " + string.Format("${0:N2}", usdBal);
            msg += "\n**BTC Value:** " + string.Format("{0:N8} BTC", btcBal);

            await PrintEmbedMessage("Ethereum Balance", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("ethtokens")]
        [Summary("Get ERC-20 Tokens linked to Ethereum address - Usage: {0}ethtokens <optional:address>")]
        public async Task EthTokens([Remainder]string address = null)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found! Please use linketh cmd or enter an address to link!";
                await PrintEmbedMessage("Account Retrieval Failed", msg);
                return;
            }
            if (address == null)
                address = account.address;
            List<EtherScan.Token> tokens = EtherScan.GetTokens(address);
            string symbols = "";
            int count = 0;
            foreach (EtherScan.Token token in tokens)
            {
                if (token.tokenSymbol != "" && !token.tokenName.ToLower().Contains("promo"))
                {
                    if (count == 1)
                    {
                        symbols += ",";
                        count = 0;
                    }
                    symbols += token.tokenSymbol;
                    count++;
                }
            }

            Dictionary<string, CoinMarketCap.Currency> currencies = CoinMarketCap.GetTokens(symbols);
            count = 0;
            int num = 0;
            msg += "**Account:** " + address;
            double totalValue = 0;
            foreach (EtherScan.Token token in tokens)
            {
                int decPlace = 0;
                if (token.tokenDecimal != "0" && token.tokenDecimal != "")
                    decPlace = int.Parse(token.tokenDecimal);
                double div = 1;
                for (int i = 0; i < decPlace; i++)
                {
                    div *= 10d;
                }
                double balance = double.Parse(token.value) / div;
                if (currencies.ContainsKey(token.tokenSymbol))
                    totalValue += currencies[token.tokenSymbol].quote.USD.price * balance;
            }
            msg += string.Format("\n**Total Value:** ${0:N5}", totalValue);
            foreach (EtherScan.Token token in tokens)
            {
                int decPlace = 0;
                if (token.tokenDecimal != "0" && token.tokenDecimal != "")
                    decPlace = int.Parse(token.tokenDecimal);
                double div = 1;
                for (int i = 0; i < decPlace; i++)
                {
                    div *= 10d;
                }
                msg += "\n\n**Name:** " + token.tokenName;
                msg += "\n**Symbol:** " + token.tokenSymbol;
                double balance = double.Parse(token.value) / div;
                if (decPlace == 0)
                    msg += "\n**Balance:** " + string.Format("{0:N0} " + token.tokenSymbol, balance);
                if (decPlace == 8)
                    msg += "\n**Balance:** " + string.Format("{0:N8} " + token.tokenSymbol, balance);
                if (decPlace == 18)
                    msg += "\n**Balance:** " + string.Format("{0:N15} " + token.tokenSymbol, balance);
                if (currencies.ContainsKey(token.tokenSymbol))
                    msg += string.Format("\n**USD Value:** ${0:N15}", currencies[token.tokenSymbol].quote.USD.price * balance);
                msg += "\n**Date:** " + SteamAccounts.UnixTimeStampToDateTime(double.Parse(token.timeStamp));
                msg += "\n**Confirmations:** " + string.Format("{0:N0}", ulong.Parse(token.confirmations));
                if (count > 8)
                {
                    msg += "|";
                    count = 0;
                }
                count++;
                num++;
            }
            string[] split = msg.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                await PrintEmbedMessage("ETH Tokens", split[i]);
            }


            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("crypto")]
        [Summary("Get basic price info of one or more coins use commas to separate - Usage: {0}crypto btc,eth,bch")]
        public async Task Crypto([Remainder]string symbols)
        {
            symbols = symbols.ToUpper();
            string[] Symbols = symbols.ToUpper().Split(',');
            string msg = "";
            Dictionary<string, CoinMarketCap.Currency> currencies = CoinMarketCap.GetTokens(symbols);
            for (int i = 0; i < Symbols.Length; i++)
            {
                if (i > 0)
                    msg += "\n";
                msg += "\n**Name:** " + currencies[Symbols[i]].name;
                msg += "\n**Symbol:** " + currencies[Symbols[i]].symbol;
                double price = currencies[Symbols[i]].quote.USD.price;
                string priceString = string.Format("${0:N2}", price);
                if (price <= 1.99)
                    priceString = string.Format("${0:N6}", price);
                else if (price <= 9.99)
                    priceString = string.Format("${0:N3}", price);
                msg += "\n**Rank:** " + string.Format("{0}", currencies[Symbols[i]].cmc_rank);
                msg += "\n**USD Price:** " + priceString;
                msg += "\n**Market Cap:** " + string.Format("${0:N2}", currencies[Symbols[i]].quote.USD.market_cap);
                msg += "\n**Volume 24h:** " + string.Format("${0:N2}", currencies[Symbols[i]].quote.USD.volume_24h);
                msg += "\n**1h Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_1h);
                msg += "\n**24h Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_24h);
                msg += "\n**7d Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_7d);
                msg += "\n**Circulating Supply:** " + string.Format("{0:N0}", currencies[Symbols[i]].circulating_supply);
                string max_supply = "None";
                if (currencies[Symbols[i]].max_supply != null)
                    max_supply = "" + currencies[Symbols[i]].max_supply;
                msg += "\n**Max Supply:** " + max_supply;
            }
            await PrintEmbedMessage("Cyptocurrency " + symbols, msg);
        }


        [Command("topcrypto")]
        [Summary("Get the top 10 cryptocurrencies by market cap - Usage: {0}topcrypto <optional:1-10>")]
        public async Task TopCrypto(int num = 0)
        {
            List<CoinMarketCap.Currency> currencies = CoinMarketCap.GetTop10();
            string msg = "";
            int count = 0;
            EmbedField[] fields = new EmbedField[(num == 0) ? 10 : num];
            foreach (CoinMarketCap.Currency currency in currencies)
            {
                msg += "\n\n__" + currency.name + "__";
                msg += "\n**Symbol:** " + currency.symbol;
                msg += "\n**Rank:** " + currency.cmc_rank;

                double price = currency.quote.USD.price;
                string priceString = string.Format("${0:N2}", price);
                if (price <= 1.99)
                    priceString = string.Format("${0:N6}", price);
                else if (price <= 9.99)
                    priceString = string.Format("${0:N3}", price);
                msg += "\n**Price:** " + priceString;
                msg += "\n**Market Cap:** " + string.Format("${0:N2}", currency.quote.USD.market_cap);
                msg += "\n**Volume 24h:** " + string.Format("${0:N2}", currency.quote.USD.volume_24h);
                msg += "\n**Change 1h:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_1h);
                msg += "\n**Change 24h:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_24h);
                msg += "\n**Change 7d:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_7d);
                msg += "\n**Circulating Supply:** " + currency.circulating_supply;
                string max_supply = "None";
                if (currency.max_supply != null)
                    max_supply = "" + currency.max_supply;
                msg += "\n**Max Supply:** " + max_supply;
                if (num != 0)
                {
                    num--;
                    if (num <= 0)
                        break;
                }
                count++;
                if (count >= 6)
                {
                    msg += "|";
                    count = 0;
                }
            }

            string[] split = msg.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                string title = "Top 10 Crypto";
                if (num != 0)
                    title = "Top " + num + " Crypto";
                await PrintEmbedMessage("Top Crypto", split[i]);
            }

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("calceth")]
        [Summary("Calculate how much ETH you can mine using your Mh/s - Usage: {0}calceth 12.5")]
        public async Task CalcETH(float hashrate)
        {
            List<NanoPool.Amount> amounts = NanoPool.CalculateEth(hashrate);
            string msg = "";
            EmbedField[] fields = new EmbedField[6];
            int count = 0;
            int num = 0;
            foreach (NanoPool.Amount amount in amounts)
            {
                switch (num)
                {
                    case 0:
                        {
                            fields[0].name = "__Minute__";
                            //msg += "\n\n__Minute__";
                            break;
                        }
                    case 1:
                        {
                            fields[1].name = "__Hour__";
                            //msg += "\n\n__Hour__";
                            break;
                        }
                    case 2:
                        {
                            fields[2].name = "__Day__";
                            //msg += "\n\n__Day__";
                            break;
                        }
                    case 3:
                        {
                            fields[3].name = "__Week__";
                            //msg += "\n\n__Week__";
                            break;
                        }
                    case 4:
                        {
                            fields[4].name = "__Month__";
                            //msg += "\n\n__Month__";
                            break;
                        }
                    case 5:
                        {
                            fields[5].name = "__Year__";
                            //msg += "\n\n__Month__";
                            break;
                        }
                }
                msg = "**Coins:** " + string.Format("{0:N10}", amount.coins);
                msg += "\n**Dollars:** " + string.Format("${0:N8}", amount.dollars);
                msg += "\n**Bitcoins:** " + string.Format("{0:N10}", amount.bitcoins);
                msg += "\n**Euros:** " + string.Format("€{0:N8}", amount.euros);
                msg += "\n**Pounds:** " + string.Format("£{0:N8}", amount.pounds);
                fields[num].value = msg;
                num++;
                count++;
                if (count >= 6)
                {
                    //msg += "|";
                    count = 0;
                }
            }

            //string[] split = msg.Split('|');
            //for (int i = 0; i < split.Length; i++)
            {
                await PrintEmbedMessage("ETH Calculator " + hashrate + " Mh/s", fields: fields);
            }

            //await Context.Channel.SendMessageAsync(msg);
        }

        #endregion


        public struct EmbedField
        {
            public string name;
            public string value;
        }
        public async Task PrintEmbedMessage(string title = "", string msg = "", string url = "", string iconUrl = "", EmbedField[] fields = null, IUser author = null)
        {
            ServerConfig config = new ServerConfig();
            if (!Context.IsPrivate)
                config = ServerConfigs.GetConfig(Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (title != "")
                embed.WithTitle(title);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (msg != "")
                embed.WithDescription(msg);
            if (author != null)
                embed.WithAuthor(author);
            if (url != "")
                embed.WithUrl(url);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            if (iconUrl != "")
                embed.WithThumbnailUrl(iconUrl);
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    embed.AddField(fields[i].name, fields[i].value, true);
                }
            }

            //Console.WriteLine(DateTime.Now + " | " + title + " |\n" + msg);
            string username = Context.User.Username + "#" + Context.User.Discriminator;
            string guildName = "Private Message";
            if (!Context.IsPrivate) guildName = Context.Guild.Name;
            string text = DateTime.Now + " | " + username + " used " + title + " in " + guildName;
            Console.WriteLine(text);
            Log.AddTextToLog(text);
            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }

        public async Task DMEmbedMessage(string title = "", string msg = "", string url = "", string iconUrl = "", EmbedField[] fields = null, IUser author = null)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (title != "")
                embed.WithTitle(title);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (msg != "")
                embed.WithDescription(msg);
            if (author != null)
                embed.WithAuthor(author);
            if (url != "")
                embed.WithUrl(url);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            if (iconUrl != "")
                embed.WithThumbnailUrl(iconUrl);
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    embed.AddField(fields[i].name, fields[i].value, true);
                }
            }
            //Console.WriteLine(DateTime.Now + " | " + title + " |\n" + msg);
            string username = Context.User.Username + "#" + Context.User.Discriminator;
            string guildName = "Private Message";
            if (!Context.IsPrivate) guildName = Context.Guild.Name;
            string text = DateTime.Now + " | " + username + " used " + title + " in " + guildName;
            Console.WriteLine(text);
            Log.AddTextToLog(text);

            var x = await Context.User.GetOrCreateDMChannelAsync();
            await x.SendMessageAsync("", false, embed.Build());
        }

    }
}
