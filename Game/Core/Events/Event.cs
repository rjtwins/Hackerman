using Core;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Game.Core.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Event
    {
        #region backing fields
        [JsonProperty]
        private Guid id; //Event ID
        [JsonProperty]
        private string name; //Event Name
        [JsonProperty]
        private double interval;//Time until firing of event in time units
        [JsonProperty]
        private DateTime startTime; //Time when to fire event in time units
        [JsonProperty]
        private string methodName = string.Empty;
        [JsonProperty]
        private string methodClass = string.Empty;
        [JsonProperty]
        private bool canceled = false;
        #endregion

        #region properties
        private Delegate Action { get; set; }
        private MethodInfo MethodInfo { get; set; }
        private Type MethodClassType { get; set; }
        private object[] MethodArguments;
        private enum MethodType { VOID, ACTION, FUNC, NOTHING };
        private MethodType mType = MethodType.VOID;
        public Guid Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double Interval { get => interval; set => interval = value; }
        public DateTime StartTime { get => startTime; set => startTime = value; }
        public string MethodName { get => methodName; set => methodName = value; }
        public string MethodClass { get => methodClass; set => methodClass = value; }
        public bool Canceled { get => canceled; set => canceled = value; }
        private MethodType MType1 { get => mType; set => mType = value; }
        #endregion


        [JsonConstructor]
        public Event()
        {

        }

        [OnDeserialized]
        public void OnJsonDeSerialize(StreamingContext streamingContext)
        {
            this.MethodClassType = Type.GetType(MethodClass);
            this.MethodInfo = MethodClassType.GetMethod(MethodName);
        }

        public Event(string v)
        {
            MType1 = MethodType.NOTHING;
            this.Id = Guid.NewGuid();
        }

        public Event(string name, double startInSecondes, Delegate methodToRun, object[] methodArguments)
        {
            this.Id = Guid.NewGuid();
            this.MType1 = MethodType.ACTION;
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.Action = methodToRun;
            this.MethodArguments = methodArguments;
            Register();
        }        

        private void Register()
        {

            this.MethodInfo = Action.Method;
            this.MethodName = Action.Method.Name;
            this.MethodClass = Action.Method.DeclaringType.FullName;
            this.MethodClassType = Type.GetType(MethodClass);

            if (!MethodInfo.IsStatic)
            {
                throw new Exception($"Delegate {methodName} is not static.\nOnly static methods can be registered at events");
            }

            if (Global.EventTicker == null)
            {
                return;
            }
            Global.EventTicker.RegisterEvent(this);
        }

        public virtual void StartEvent()
        {
            if (this.Canceled)
            {
                return;
            }
            LogEvent();
            if(this.MethodClassType == null)
            {
                this.MethodClassType = Type.GetType(MethodClass);
            }
            this.MethodClassType.GetMethod(MethodName).Invoke(null, new object[] { this.MethodArguments });
        }

        public virtual void LogEvent()
        {
            Logger.EventLog(Id.ToString(), Name);
        }

        public virtual void AddSecondes(double secondes)
        {
            this.StartTime = this.StartTime.AddSeconds(secondes);
        }

        public virtual void CancelEvent()
        {
            this.Canceled = true;
        }

        public virtual void UnCancelEvent()
        {
            this.Canceled = false;
        }

        public virtual void SetStartTime(DateTime dateTime)
        {
            this.StartTime = dateTime;
            this.Interval = (Global.GameTime - dateTime).TotalSeconds;
        }

        public virtual void SetStartInterval(double IntervalInSecondes)
        {
            this.Interval = IntervalInSecondes;
            this.StartTime = Global.GameTime.AddSeconds(IntervalInSecondes);
        }
    }
}