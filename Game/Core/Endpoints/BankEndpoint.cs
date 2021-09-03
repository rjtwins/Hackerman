using Game.Model;
using System.Collections.Generic;

namespace Game.Core.Endpoints
{
    public class BankEndpoint : Endpoint
    {
        public HashSet<Person> Clients = new();

        public BankEndpoint(Person person, EndpointType endpointType) : base(person, endpointType)
        {
        }

        public string TransferMoney(string from, string too, BankEndpoint tooBank, int amount)
        {
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
                return "Receiver not found.";
            }

            if (personToo == personFrom)
            {
                return "Cannot transfer to yourself.";
            }

            if (personFrom.BankBalance < amount)
            {
                return "Insufficient balance.";
            }

            personFrom.BankBalance -= amount;
            personToo.BankBalance += amount;

            return "Transfer succesfull";
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