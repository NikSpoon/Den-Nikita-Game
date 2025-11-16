using System;
using UnityEngine;

namespace Fsm.UIApp
{
    public interface IAppSystem
    {
        AppState CurrentState { get; }
        void Trigger(AppTriger trigger);
        event Action<StateChangeData<AppState, AppTriger>> OnStateChange;
    }

    public class UIApp : IAppSystem
    {
        private Fsm<AppState, AppTriger> _stateMashine;
        public AppState CurrentState => _stateMashine.CurrentState;

        event Action<StateChangeData<AppState, AppTriger>> IAppSystem.OnStateChange
        {
            add => _stateMashine.OnStateChange += value;
            remove => _stateMashine.OnStateChange -= value;
        }


        public UIApp()
        {
            _stateMashine = new Fsm<AppState, AppTriger>(AppState.Loading);

            _stateMashine.AddTransition(AppState.Loading, AppTriger.ToMainMenu, AppState.MainMenu);

            _stateMashine.AddTransition(AppState.MainMenu, AppTriger.ToGame3D, AppState.Game3D);

            _stateMashine.AddTransition(AppState.Game3D, AppTriger.ToGame2D, AppState.Game2D);
            _stateMashine.AddTransition(AppState.Game3D, AppTriger.ToSave, AppState.Save);
            _stateMashine.AddTransition(AppState.Game3D, AppTriger.ToFinish, AppState.Finish);

            _stateMashine.AddTransition(AppState.Game2D, AppTriger.ToGame3D, AppState.Game3D);

            _stateMashine.AddTransition(AppState.Finish, AppTriger.ToMainMenu, AppState.MainMenu);

            _stateMashine.AddTransition(AppState.Save, AppTriger.ToMainMenu, AppState.MainMenu);
        }

        public void Trigger(AppTriger trigger)
        {
            _stateMashine.SetTrigger(trigger);
        }

    }

    public enum AppState
    {
        Loading,
        MainMenu,
        Game3D,
        Game2D,
        Finish,
        Save
    }

    public enum AppTriger
    {
        ToMainMenu,
        ToGame3D,
        ToGame2D,
        ToFinish,
        ToSave
    }
}