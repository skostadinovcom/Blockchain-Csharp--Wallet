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
                //Balance
            }

            if( operation == "history" )
            {
                //History
            }

            if( operation == "receive" )
            {
              //  Receive
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
    }
}
