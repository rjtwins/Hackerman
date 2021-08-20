using Spin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Dialog
{
    public class DialogResolver
    {
        Sequence Sequence;
        VariableRef ChoiseVariable = new VariableRef("choise");
        VariableRef DelayVariable = new VariableRef("delay");
        VariableRef EndOfConvoVariable = new VariableRef("endOfConvo");
        VariableRef ErrorVariable = new VariableRef("error");
        string PreviousLineName = string.Empty;
        List<string> History = new List<string>();
        public int Choise;
        public bool EndOfConvo = false;
        private string AttachedChannelName;

        public DialogResolver(string sequenceName, string attachedChannelName)
        {
            Sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            Sequence.RegisterStandardLibrary();
            Sequence.AutomaticLineBreaks = true;
            this.AttachedChannelName = attachedChannelName;
            SelectSequence(sequenceName);
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

        public void StartFromLine(string Line)
        {
            Sequence.SetNextLine(Line);
        }

        private void ContinueAfterDelay(int sec, int parserIndex, Sequence sequence)
        {
            System.Threading.Thread.Sleep(sec * 1000);
            Parse(sequence, parserIndex);
        }

        private void Parse(Sequence Sequence, int startIndex)
        {
            string sString = string.Empty;
            try
            {
                sString = Sequence.ExecuteCurrentLine().BuildString();
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
                    Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, Global.GameTime + " : " + user, result);
                    result = string.Empty;
                    Task.Factory.StartNew(() => this.ContinueAfterDelay(delay, i + 1, Sequence));
                    return;
                }

                if (s.StartsWith('@'))
                {
                    if(result != string.Empty)
                    {
                        Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, Global.GameTime + " : " + user, result);
                        result = string.Empty;
                    }
                    s = s.Remove(0, 1);
                    if(s.Contains("PLAYER"))
                    {
                        s = Global.CurrentUserName;
                    }
                    user = s;
                    i++;
                    s = splitString[i];
                }
                if(i == splitString.Length - 1)
                {
                    result += s;
                    continue;
                }
                result += s + "\n";
            }
            Global.IRCWindow.AddMessageFromThread(this.AttachedChannelName, Global.GameTime + " : " + user, result);
        }


        public void StartDialog()
        {
            this.EndOfConvo = false;
            Sequence.SetNextLine("start");
            Sequence.StartNextLine();
            Sequence.SetVariable(ChoiseVariable, 0);
            Sequence.SetVariable(DelayVariable, 0);
            Sequence.SetVariable(EndOfConvoVariable, false);
            Sequence.SetVariable(ErrorVariable, false);
            Parse(Sequence, 0);
        }

        public void Next(int choise)
        {
            this.PreviousLineName = Sequence.CurrentLine.Value.Name;
            if ((bool)Sequence.GetVariable(EndOfConvoVariable))
            {
                this.EndOfConvo = true;
                return;
                //return "--EndOfDialog--";
            }

            Sequence.StartNextLine();

            this.Choise = choise;
            Sequence.SetVariable(ChoiseVariable, choise.ToString());
            Parse(Sequence, 0);
        }
    }
}
