using Game.Core.Mission;
using Game.Core.Mission.MissionTypes;
using Newtonsoft.Json;
using Spin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Game.Core.Dialog
{
    [JsonObject(MemberSerialization.OptOut)]
    public class GenericMissionDialogResolver
    {
        //TODO: Make the dialog show the target ip and name
        //TODO: Look into registering commands with the sequence to check for things instead of relying on flags.
        [JsonIgnore]
        private Sequence Sequence;

        [JsonProperty]
        private string SequenceName = string.Empty;
        [JsonProperty]
        private string NextLine;

        [JsonProperty]
        private VariableRef _ChoiseVariable = new VariableRef("choise");
        [JsonProperty]
        private object _ChoiseVariableValue;
        [JsonProperty]
        private VariableRef _EndOfConvoVariable = new VariableRef("endOfConvo");
        [JsonProperty]
        private object _EndOfConvoVariableValue;
        [JsonProperty]
        private VariableRef _DialogResult = new VariableRef("result");
        [JsonProperty]
        private object _DialogResultValue;
        [JsonProperty]
        private VariableRef _Contact = new VariableRef("contact");
        [JsonProperty]
        private object _ContactValue;
        [JsonProperty]
        private VariableRef _Target = new VariableRef("target");
        [JsonProperty]
        private object _TargetValue;
        [JsonProperty]
        private VariableRef _Reward = new VariableRef("reward");
        [JsonProperty]
        private object _RewardValue;
        [JsonProperty]
        private VariableRef _MissionCompleted = new VariableRef("missionCompleted");
        [JsonProperty]
        private object _MissionCompletedValue;
        [JsonProperty]
        private VariableRef _MissionAccepted = new VariableRef("missionAccepted");
        [JsonProperty]
        private object _MissionAcceptedValue;
        [JsonProperty]
        private VariableRef _MissionRejected = new VariableRef("missionRejected");
        [JsonProperty]
        private object _MissionRejectedValue;
        [JsonProperty]
        private VariableRef _CheckMissionCompleted = new VariableRef("checkMissionCompleted");
        [JsonProperty]
        private object _CheckMissionCompletedValue;
        [JsonProperty]
        private VariableRef _TargetIp = new VariableRef("targetIp");
        [JsonProperty]
        private object _TargetIpValue;
        [JsonProperty]
        private VariableRef _FilesToSteal = new VariableRef("filesToSteal");
        [JsonProperty]
        private object _FilesToStealValue;



        [JsonProperty]
        public int DialogResult = 99;
        [JsonProperty]
        private string Contact = string.Empty;
        [JsonProperty]
        private string PreviousLineName = string.Empty;
        [JsonProperty]
        private List<string> History = new List<string>();
        [JsonProperty]
        public int Choise;
        [JsonProperty]
        public bool EndOfConvo = false;
        [JsonProperty]
        private string AttachedChannelName;
        [JsonProperty]
        private Guid mission;
        [JsonProperty]
        private bool Started = false;
        
        [JsonIgnore]
        public MissionTemplate Mission 
        { 
            get 
            { 
                if(this.mission == Guid.Empty)
                {
                    return null;
                }
                return Global.AllMissionsDict[mission]; 
            }
            set => mission = value.Id; 
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            this.Sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            Sequence.RegisterStandardLibrary();
            Sequence.AutomaticLineBreaks = false;
            Sequence.LoadAndStartDocument("Core/Mission/Dictionaries/" + this.SequenceName + ".spd");

            Sequence.SetVariable(_ChoiseVariable, _ChoiseVariableValue);
            Sequence.SetVariable(_EndOfConvoVariable, _EndOfConvoVariableValue);
            Sequence.SetVariable(_DialogResult, _DialogResultValue);
            Sequence.SetVariable(_Contact, _ContactValue);
            Sequence.SetVariable(_Target, _TargetValue);
            Sequence.SetVariable(_Reward, _RewardValue);
            Sequence.SetVariable(_MissionCompleted, _MissionCompletedValue);
            Sequence.SetVariable(_MissionAccepted, _MissionAcceptedValue);
            Sequence.SetVariable(_MissionRejected, _MissionRejectedValue);
            Sequence.SetVariable(_CheckMissionCompleted, _CheckMissionCompletedValue);
            Sequence.SetVariable(_TargetIp, _TargetIpValue);
            Sequence.SetVariable(_FilesToSteal, _FilesToStealValue);

            Sequence.SetNextLine(this.NextLine);
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext streamingContext)
        {
            this.NextLine = this.Sequence.NextLine.Value.Name;
            Sequence.TryGetVariable(_ChoiseVariable, out _ChoiseVariableValue);
            Sequence.TryGetVariable(_EndOfConvoVariable, out _EndOfConvoVariableValue);
            Sequence.TryGetVariable(_DialogResult, out _DialogResultValue);
            Sequence.TryGetVariable(_Contact, out _ContactValue);
            Sequence.TryGetVariable(_Target, out _TargetValue);
            Sequence.TryGetVariable(_Reward, out _RewardValue);
            Sequence.TryGetVariable(_MissionCompleted, out _MissionCompletedValue);
            Sequence.TryGetVariable(_MissionAccepted, out _MissionAcceptedValue);
            Sequence.TryGetVariable(_MissionRejected, out _MissionRejectedValue);
            Sequence.TryGetVariable(_CheckMissionCompleted, out _CheckMissionCompletedValue);
            Sequence.TryGetVariable(_TargetIp, out _TargetIpValue);
            Sequence.TryGetVariable(_FilesToSteal, out _FilesToStealValue);
        }


        [JsonConstructor]
        public GenericMissionDialogResolver()
        {

        }

        public GenericMissionDialogResolver(MissionTemplate mission)
        {
            this.Mission = mission;
            Sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            Sequence.RegisterStandardLibrary();
            Sequence.AutomaticLineBreaks = false;
            this.AttachedChannelName = Mission.MissionChannel;
        }

        internal void DisplayFileNotAccepted(string v)
        {
            throw new NotImplementedException();
        }

        public void SelectSequenceFromMissionDictionaries(string sequenceName)
        {
            this.SequenceName = sequenceName;
            Sequence.LoadAndStartDocument("Core/Mission/Dictionaries/" + sequenceName + ".spd");
        }

        public void StartFromLine(string Line)
        {
            Sequence.SetNextLine(Line);
        }

        private void ContinueAfterDelay(int sec, int parserIndex, Sequence sequence)
        {
            System.Threading.Thread.Sleep(sec * 1000);
            Parse(sequence, parserIndex);
        }

        /// <summary>
        /// Recursivly parse a sequence starting at an index with possible delay.
        /// </summary>
        /// <param name="Sequence"></param>
        /// <param name="startIndex"></param>
        private void Parse(Sequence Sequence, int startIndex)
        {
            string sString = string.Empty;
            try
            {
                sString = Sequence.ExecuteCurrentLine().BuildString(true, false);
            }
            catch (NullReferenceException ex)
            {
                Sequence.SetNextLine(PreviousLineName);
                Sequence.StartNextLine();
                Sequence.ExecuteCurrentLine();
                return;
            }
            this.NextLine = Sequence.NextLine.Value.Name;

            string[] splitString = sString.Split('\n');
            string result = string.Empty;
            string user = string.Empty;
            string currentWho = string.Empty;
            for (int i = startIndex; i < splitString.Length; i++)
            {
                string s = splitString[i];
                if (s.StartsWith('%'))
                {
                    s = s.Remove(0, 1);
                    int delay = int.Parse(s);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, user, result);
                    }
                    result = string.Empty;
                    Task.Factory.StartNew(() => this.ContinueAfterDelay(delay, i + 1, Sequence));
                    return;
                }

                if (s.StartsWith('@'))
                {
                    if (result.StartsWith("\n"))
                    {
                        result.TrimStart();
                    }
                    if (!string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result))
                    {
                        Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, user, result);
                        result = string.Empty;
                    }
                    s = s.Remove(0, 1);
                    if (s.Contains("PLAYER"))
                    {
                        s = GameState.Instance.UserName;
                    }
                    user = s;
                    i++;
                    s = splitString[i];
                }
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = string.Empty;
                }
                if (i == splitString.Length - 1)
                {
                    result += s;
                    continue;
                }
                result += s + "\n";
            }
            Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, user, result);
        }

        public string SetInfoGetListing(string contact, string target, int reward)
        {
            this.Contact = contact;
            this.Sequence.SetVariable(_Contact, contact);
            this.Sequence.SetVariable(_Target, target);
            this.Sequence.SetVariable(_Reward, reward);
            this.Sequence.SetVariable(_TargetIp, Mission.TargetEndpoint.IPAddress);
            Sequence.SetNextLine("listing");
            Sequence.StartNextLine();
            return Sequence.ExecuteCurrentLine().BuildString(true, false);
        }

        public void SetFilesToSteal(string files)
        {
            Sequence.SetVariable(_FilesToSteal, files);
        }

        public void MissionCompleted()
        {
            this.Sequence.SetVariable(_MissionCompleted, true);
            this.Sequence.SetNextLine("missionCompleted");
        }

        public void SetupDialog()
        {
            this.EndOfConvo = false;
            Sequence.SetNextLine("start");
            Sequence.StartNextLine();
            Sequence.SetVariable(_ChoiseVariable, 0);
            Sequence.SetVariable(_EndOfConvoVariable, false);
            Sequence.SetVariable(_DialogResult, -1);
            Sequence.SetVariable(_MissionCompleted, false);
            Sequence.SetVariable(_MissionAccepted, false);
            Sequence.SetVariable(_MissionRejected, false);
            Sequence.SetVariable(_CheckMissionCompleted, false);
            Parse(Sequence, 0);
        }

        public void CheckStartDialog()
        {
            if (Started)
            {
                return;
            }
            Next(0);
            Started = true;
        }

        public void Next(int choise = 0)
        {
            this.PreviousLineName = Sequence.CurrentLine.Value.Name;
            if ((bool)Sequence.GetVariable(_EndOfConvoVariable))
            {
                this.EndOfConvo = true;

                return;
            }

            Sequence.StartNextLine();
            this.Choise = choise;
            Sequence.SetVariable(_ChoiseVariable, choise.ToString());
            Parse(Sequence, 0);

            //Chick mission completed
            if ((bool)Sequence.GetVariable(_CheckMissionCompleted))
            {
                if (Mission.CheckMissionCompleted())
                {
                    Sequence.SetVariable(_MissionCompleted, true);
                }
            }

            //Check for mission accepted
            if ((bool)Sequence.GetVariable(_MissionAccepted))
            {
                MissionManager.Instance.AcceptMission(this.Mission);
            }
            if ((bool)Sequence.GetVariable(_MissionRejected))
            {
                MissionManager.Instance.RejectMission(this.Mission);
            }
        }
    }
}