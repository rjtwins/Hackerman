using Game.Core.Mission.MissionTypes;
using Spin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Game.Core.Dialog
{
    public class GenericMissionDialogResolver
    {
        //TODO: Make the dialog show the target ip and name
        //TODO: Look into registering commands with the sequence to check for things instead of relying on flags.
        private Sequence Sequence;

        private VariableRef _ChoiseVariable = new VariableRef("choise");
        private VariableRef _EndOfConvoVariable = new VariableRef("endOfConvo");
        private VariableRef _DialogResult = new VariableRef("result");
        private VariableRef _Contact = new VariableRef("contact");
        private VariableRef _Target = new VariableRef("target");
        private VariableRef _Reward = new VariableRef("reward");
        private VariableRef _MissionCompleted = new VariableRef("missionCompleted");
        private VariableRef _MissionAccepted = new VariableRef("missionAccepted");
        private VariableRef _MissionRejected = new VariableRef("missionRejected");
        private VariableRef _CheckMissionCompleted = new VariableRef("checkMissionCompleted");
        private VariableRef _TargetIp = new VariableRef("targetIp");
        private VariableRef _FilesToSteal = new VariableRef("filesToSteal");

        public int DialogResult = 99;
        private string Contact = string.Empty;

        private string PreviousLineName = string.Empty;
        private List<string> History = new List<string>();
        public int Choise;
        public bool EndOfConvo = false;
        private string AttachedChannelName;
        public MissionTemplate Mission;
        private bool Started = false;

        public GenericMissionDialogResolver(string attachedChannelName)
        {
            Sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            Sequence.RegisterStandardLibrary();
            Sequence.AutomaticLineBreaks = false;
            this.AttachedChannelName = attachedChannelName;
        }

        public void SelectSequence(string sequenceName)
        {
            try
            {
                Sequence.LoadAndStartDocument("Core/Dialog/DialogSequences/" + sequenceName + ".spd");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        internal void DisplayFileNotAccepted(string v)
        {
            throw new NotImplementedException();
        }

        public void SelectSequenceFromMissionDictionaries(string sequenceName)
        {
            Sequence.LoadAndStartDocument("Core/Mission/Dictionaries/" + sequenceName + ".spd");
        }

        //public void SetContact(string contact)
        //{
        //    this.Contact = contact;
        //    this.Sequence.SetVariable(_Contact, contact);
        //}

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
                        s = Global.GameState.UserName;
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
                Global.MissionManager.AcceptMission(this.Mission);
            }
            if ((bool)Sequence.GetVariable(_MissionRejected))
            {
                Global.MissionManager.RejectMission(this.Mission);
            }
        }
    }
}