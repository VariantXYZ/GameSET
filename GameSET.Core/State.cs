using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSET.Core
{
    /// <summary>
    /// A particular GameState
    /// </summary>
    public class State
    {
        private static State CurrentState; // Maintain a reference to the current state so we can have C exports modify it

        public static void SetCurrentState(State st) => CurrentState = st;

        State(bool setCurrentState = true)
        {
            if (setCurrentState)
            {
                SetCurrentState(this);
            }
        }

        ~State()
        {
        }
    }
}
