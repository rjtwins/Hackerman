using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.World
{
    class Consequences
    {
        private static readonly Consequences instance = new Consequences();

        public static Consequences Instance
        {
            get
            {
                return instance;
            }
            set
            {

            }
        }

        public void ActiveTraceCaught(params object[] arguments)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
