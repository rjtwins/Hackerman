using Game.Model;
using System;
using System.Collections.Generic;

namespace Game.Core.Endpoints
{
    public class BankEndpoint : Endpoint
    {
        public HashSet<Person> Clients { get; set; } = new();

        private enum TransferType { RECEIVED, SEND};
        private List<(TransferType, LogItem)> CreditTransferLogs { get; set; } = new();

        public BankEndpoint(Person person, EndpointType endpointType) : base(person, endpointType)
        {
        }

        public bool TransferMoney(string from, string too, BankEndpoint tooBank, int amount, out string result)
        {
            result = string.Empty;
            Person personFrom = null;
            Person personToo = null;

            foreach (Person p in Clients)
            {
                if (p.Name == from)
                {
                    personFrom = p;
                    break;
                }
            }

            foreach (Person p in tooBank.Clients)
            {
                if (p.Name == too)
                {
                    personToo = p;
                    break;
                }
            }

            if (personToo == null || personFrom == null)
            {
                result = "Receiver not found.";
                return false;
            }

            if (personToo == personFrom)
            {
                result = "Cannot transfer to yourself.";
                return false;
            }

            if (personFrom.BankBalance < amount)
            {
                result = "Insufficient balance.";
                return false;
            }

            personFrom.BankBalance -= amount;
            personToo.BankBalance += amount;

            result = "Transfer succesfull";

            LogItem transferLog = LogItemBuilder.Builder()
                    .CREDITTRANSFER(personFrom, this, personToo, tooBank, amount)
                    .From(null)
                    .User(null)
                    .AccesLevel(AccessLevel.USER)
                    .TimeStamp(Global.GameTime);

            this.CreditTransferLogs.Add((TransferType.SEND, transferLog));
            tooBank.LogReceivedTransfer(transferLog);
            return true;
        }

        private void LogReceivedTransfer(LogItem transferLog)
        {
            this.CreditTransferLogs.Add((TransferType.RECEIVED, transferLog));
        }

        public Person LoginBankAcount(string username, string password)
        {
            foreach (Person p in Clients)
            {
                if (p.Name == username && p.BankPassword == password)
                {
                    return p;
                }
            }
            return null;
        }

        internal string PrintTransferLogs()
        {
            if(((int)this.AccessLevel) < ((int)AccessLevel.ADMIN))
            {
                return "Invalid access level";
            }

            string result = string.Empty;
            foreach ((TransferType, LogItem) logItem in this.CreditTransferLogs)
            {
                if(logItem.Item1 == TransferType.RECEIVED)
                {
                    result += $"{logItem.Item2.TimeStamp} client: {logItem.Item2.TransferFrom.Name} received {logItem.Item2.TransferAmount} from {logItem.Item2.TransferToo}";
                }
                else
                {
                    result += $"{logItem.Item2.TimeStamp} client: {logItem.Item2.TransferFrom.Name} send {logItem.Item2.TransferAmount} to {logItem.Item2.TransferToo}";
                }
                result += Environment.NewLine;
            }
            return result;
        }

        public bool LoginBankAcount(Person p)
        {
            if (Clients.Contains(p))
            {
                return true;
            }
            return false;
        }

        public int GetBankBalance(Person p)
        {
            return p.BankBalance;
        }
    }
}