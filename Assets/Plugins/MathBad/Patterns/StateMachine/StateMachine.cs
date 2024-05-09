//  ___  _          _         __  __            _     _                                               
// / __|| |_  __ _ | |_  ___ |  \/  | __ _  __ | |_  (_) _ _   ___                                    
// \__ \|  _|/ _` ||  _|/ -_)| |\/| |/ _` |/ _|| ' \ | || ' \ / -_)                                   
// |___/ \__|\__,_| \__|\___||_|  |_|\__,_|\__||_||_||_||_||_|\___|                                   
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
public interface IState<T> where T : Component, IStateRunner<T>
{
  public T runner { get; set; }
  public int priority { get; set; }
  
  public void Init();
  public bool CanStep(float dt);
  public void EnterState();
  public void Step(float dt);
  public void ExitState();
}

public interface IStateRunner<T> where T : Component, IStateRunner<T>
{
  public StateMachine<T> stateMachine { get; }
}

/// <summary> State Machine </summary>
public class StateMachine<T> where T : Component, IStateRunner<T>
{
  T _runner;
  bool _hasAuthority = true;
  bool _isDirty = false;

  IState<T> _currentState;
  IState<T>[] _states;

  public StateMachine(T runner)
  {
    _runner = runner;
    _states = _runner.GetComponents<IState<T>>();

    for(int i = 0; i < _states.Length; i++)
    {
      _states[i].priority = i;
      _states[i].Init();
    }

    Array.Reverse(_states);
  }

  public bool hasAuthority
  {
    get => _hasAuthority;
    set => _hasAuthority = value;
  }
  public IState<T> currentState => _currentState;
  public IState<T>[] states => _states;

  // Step
  //----------------------------------------------------------------------------------------------------
  public void Step(float deltaTime)
  {
    // Step the current state
    if(_currentState != null && _currentState.CanStep(deltaTime))
      _currentState.Step(deltaTime);

    // Check for higher priority state
    foreach(IState<T> state in _states)
    {
      if(_currentState != null)
      {
        if(state == null)
          continue;

        if(state.CanStep(deltaTime))
          if(state.priority > _currentState.priority)
          {
            ChangeState(state);
          }
          else if(!_currentState.CanStep(deltaTime))
          {
            ChangeState(state);
          }
      }
      else if(state.CanStep(deltaTime))
      {
        ChangeState(state); // change State
      }

      if(_isDirty)
      {
        _isDirty = false;
        break;
      }
    }
  }

  // Change State
  //--------------------------------------------------------------------------------------------------
  void ChangeState(IState<T> state)
  {
    if(currentState != null)
      currentState.ExitState();

    _currentState = state;
    _currentState.EnterState();

    _isDirty = true;
  }

  // Authority
  //--------------------------------------------------------------------------------------------------

  // Run
  //--------------------------------------------------------------------------------------------------
  /// <summary>
  /// Exit states and start stepping.
  /// </summary>
  public void Run()
  {
    foreach(IState<T> state in _states)
      state.ExitState();

    _currentState = null;
    hasAuthority = true;
  }

  // Stop
  //--------------------------------------------------------------------------------------------------
  /// <summary>
  /// Reset and stop the state machine.
  /// </summary>
  public void Stop()
  {
    foreach(IState<T> state in _states)
      state.ExitState();

    _currentState = null;
    hasAuthority = false;
  }
}
}
