using System;
using System.Runtime.Serialization;

namespace Game.Core
{
    public class GameState
    {
        private static GameState instance;
        public static GameState Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new();
                }
                return instance;
            }
            private set
            {

            }
        }

        private GameState()
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            GameState.instance = this;
        }

        public string UserName { private set; get; }
        public int Reputation { private set; get; }
        public int Money { private set; get; }

        #region setters

        public void SetUserName(string userName)
        {
            this.UserName = userName;
        }

        public void SetReputation(int reputation)
        {
            this.Reputation = Math.Max(reputation, 0);
        }

        public void AddReputation(int add)
        {
            this.Reputation += add;
        }

        public void RemoveReputation(int remove)
        {
            this.Reputation = Math.Max(this.Reputation - remove, 0);
        }

        public void SetMoney(int money)
        {
            this.Money = Math.Max(money, 0);
        }

        public void AddMoney(int add)
        {
            this.Money += add;
        }

        public void RemoveMoney(int remove)
        {
            this.Money = Math.Max(this.Money - remove, 0);
        }

        #endregion setters
    }
}