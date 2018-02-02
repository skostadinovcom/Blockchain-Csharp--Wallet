using System;
using System.Collections;
using System.Globalization;
using System.Text;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using HBitcoin.KeyManagement;

namespace BitCoinWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            Begin:
            Console.WriteLine("Wellcome in our wallet. Please enter, operation.");
            Console.WriteLine("[\"Create\", \"Recover\", \"Balance\", \"History\", \"Receive\", \"Send\", \"Exit\"]");
      
            var operation = Console.ReadLine().ToLower().Trim();

            if( operation == "create" )
            {
                CreateNewWallet();
                goto Begin;
            }

            if( operation == "recover" )
            {
                RecoverWallet();
                goto Begin;
            }

            if( operation == "balance" )
            {
                WalletBalance();
                goto Begin;
            }

            if( operation == "history" )
            {
                WalletHistory();
                goto Begin;
            }

            if( operation == "receive" )
            {
                //Receive
            }

            if( operation == "send" )
            {
                //Send
            }
        }

        public static void CreateNewWallet()
        {
            Network currentNetwork = Network.TestNet;

            string walletsPath = @"Wallets/";

            //Set password
            string password;
            string confirmedPassword;

            do
            {
                Console.WriteLine("Please, enter password:");
                password = Console.ReadLine();

                Console.WriteLine("Confirm, your password:");
                confirmedPassword = Console.ReadLine();

                if( password != confirmedPassword )
                {
                    Console.WriteLine("Passwords, did not match!");
                    Console.WriteLine("Please try again.");
                }

            } while ( password != confirmedPassword );

            //Failure
            bool failure = true;
            while( failure )
            {
                try
                {
                    Console.WriteLine("Enter wallet name:");
                    string walletName = Console.ReadLine();

                    Mnemonic mnemonic;
                    Safe safe = Safe
                                .Create(out mnemonic, password, walletsPath + walletName + ".json", currentNetwork);

                    Console.WriteLine("Your wallet, was created successfully.");
                    Console.WriteLine("With the MNEMONIC words and the PASSWORD you can recover you wallet.");
                    Console.WriteLine("-----------");
                    Console.WriteLine( mnemonic );
                    Console.WriteLine("-----------");

                    Console.WriteLine("Write down and keep in SECURE place your private keys. Only through them you can access your coins!");

                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"Address: {safe.GetAddress(i)} -> Private key: {safe.FindPrivateKey(safe.GetAddress(i))}");
                    }

                    failure = false;
                }
                catch
                {
                    Console.WriteLine("Wallet already exist!");
                }
            }
        }

        public static void RecoverWallet()
        {
            Console.WriteLine("NOTE: The wallet cannot check if you password is incorect.");
            Console.WriteLine("If you provide, wrong password a wallet will be recovered with your provided mnemonic AND password pair.");
            Console.WriteLine("-----");

            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            Console.WriteLine("Enter mnemonic phrase: ");
            string mnemonic = Console.ReadLine();

            Console.WriteLine("Enter date (yyyy-MM-dd): ");
            string date = Console.ReadLine();

            Mnemonic mnem = new Mnemonic( mnemonic );

            Network currentNetwork = Network.TestNet;
            string walletsPath = @"Wallets\";
            Random rand = new Random();

            Safe safe = Safe.Recover(mnem, password, walletsPath + "RecoveredWalletNum" + rand.Next() + ".json", currentNetwork,
                creationTime: DateTimeOffset.ParseExact(date,
                    "yyyy-MM-dd", CultureInfo.InvariantCulture));

            Console.WriteLine("Wallet successfully recovered");
        }

        public static void WalletBalance()
        {
            string walletsPath = @"Wallets/";

            Console.WriteLine("Enter wallet's name: ");
            string walletName = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            Console.WriteLine("Enter wallet address: ");
            string wallet = Console.ReadLine();

            try
            {
                Safe loadedSafe = Safe.Load(password, walletsPath + walletName + ".json");
            }
            catch
            {
                Console.WriteLine("Wrong wallet or password!");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
            decimal totalBalance = 0;
            var balance = client.GetBalance(BitcoinAddress.Create(wallet), true).Result;

            foreach (var entry in balance.Operations)
            {
                foreach (var coin in entry.ReceivedCoins)
                {
                    Money amount = (Money)coin.Amount;
                    decimal currentAmount = amount.ToDecimal(MoneyUnit.BTC);
                    totalBalance += currentAmount;
                }
            }

            Console.WriteLine($"Balance of wallet: {totalBalance}");
        }

        public static void WalletHistory()
        {
            string walletsPath = @"Wallets/";

            Console.WriteLine("Enter wallet's name: ");
            string walletName = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            Console.WriteLine("Enter wallet address: ");
            string wallet = Console.ReadLine();

            try
            {
                Safe loadedSafe = Safe.Load(password, walletsPath + walletName + ".json");
            }
            catch
            {
                Console.WriteLine("Wrong wallet or password!");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
            var coinsReceived = client.GetBalance(BitcoinAddress.Create(wallet), true).Result;
            string header = "-----COINS RECEIVED-----";
            Console.WriteLine(header);
            foreach (var entry in coinsReceived.Operations)
            {
                foreach (var coin in entry.ReceivedCoins)
                {
                    Money amount = (Money)coin.Amount;
                    Console.WriteLine($"Transaction ID: {coin.Outpoint}; Received coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                }
            }

            Console.WriteLine(new string('-', header.Length));
            var coinsSpent = client.GetBalance(BitcoinAddress.Create(wallet), false).Result;
            string footer = "-----COINS SPENT-----";

            Console.WriteLine(footer);

            foreach (var entry in coinsSpent.Operations)
            {
                foreach (var coin in entry.SpentCoins)
                {
                    Money amount = (Money)coin.Amount;
                    Console.WriteLine($"Transaction ID: {coin.Outpoint}; Spent coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                }
            }

            Console.WriteLine(new string('-', footer.Length));
        }
    }
}
