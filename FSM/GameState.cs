using System.Collections;

namespace GameKit.FSM
{
    public interface IGameState
    {
        StateContext Context { get; set; }
    }
    
    public interface IGameStateStart : IGameState
    {
        void OnStart();
    }

    public interface IGameStateStop : IGameState
    {
        void OnStop();
    }

    public interface IGameStateUpdate : IGameState
    {
        void OnUpdate();
    }
}