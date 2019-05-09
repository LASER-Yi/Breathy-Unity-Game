using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LGameStructure;


public interface ICharacterDataDidChangedHandler
{
    void OnCharacterDataChanged(GameManager sender, CharacterData data);
}

public interface ITimeDidChangedHandler
{
    void OnGameTimeChanged(GameManager sender, TimeOfGame time);
}