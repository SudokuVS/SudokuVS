﻿namespace SudokuVS.Game.Exceptions;

public class EntityNotFoundException<TEntity> : Exception
{
    public EntityNotFoundException(object key) : base($"Could not find entity {typeof(TEntity).Name} with key {key}") { }
}