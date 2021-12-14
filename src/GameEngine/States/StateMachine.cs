using System.Diagnostics;

namespace GameEngine.States
{
    // StateMachine Class

    // Code taken and edited from lessons in http://howtomakeanrpg.com

    // Usage:

    // States are only created as need, to save memory, reduce clean-up bugs and increase speed
    // due to garbage collection taking longer with more data in memory.

    // States are added with a string identifier and an intialisation function.
    // It is expect the init function, when called, will return a table with
    // Render, Update, Enter and Exit methods.

    //gStateMachine = StateMachine {
    //		['MainMenu'] = function()
    //			return MainMenu()
    //		end,
    //		['InnerGame'] = function()
    //			return InnerGame()
    //		end,
    //		['GameOver'] = function()
    //			return GameOver()
    //		end,
    //}
    //gStateMachine:change("MainGame")

    // Arguments passed into the Change function after the state name
    // will be forwarded to the Enter function of the state being changed too.

    // State identifiers should have the same name as the state table, unless there's a good
    // reason not to. i.e. MainMenu creates a state using the MainMenu table. This keeps things
    // straight forward.
    public class StateMachine
    {
        State empty;
        Dictionary<string, State> states;
        State current;

        bool hasStateEntered; // Used to avoid calling Update or Render before Enter

        public StateMachine(Dictionary<string, State> states)
        {
            empty = new EmptyState();
            this.states = states ?? new Dictionary<string, State>(); // [name] -> [function that returns states]
            current = empty;

            hasStateEntered = true;
        }

        public async Task Change(string stateName, Dictionary<string, object> enterParameters = null)
        {
            Debug.Assert(states.ContainsKey(stateName)); // state must exist!
            hasStateEntered = false;
            await current.Exit();
            current = states[stateName];
            await current.Enter(enterParameters);
            hasStateEntered = true;
        }

        public async Task Update(TimeSpan dt)
        {
            if (hasStateEntered)
            {
                await current.Update(dt);
            }
        }

        public async Task Render()
        {
            if (hasStateEntered)
            {
                await current.Render();
            }
        }

        // Used for states that can be controlled by the AI to influence update logic.
        public async Task ProcessAI(Dictionary<string, object> parameters, TimeSpan dt)
        {
            await current.ProcessAI(parameters, dt);
        }
    }
}
