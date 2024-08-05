﻿using SudokuVS.Game;
using SudokuVS.Game.Persistence;

namespace SudokuVS.WebApi.Services;

public class GamesService
{
    readonly ISudokuGamesRepository _repository;

    public GamesService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SudokuGame>> GetGamesAsync(CancellationToken cancellationToken = default) => await _repository.GetAllAsync(cancellationToken);
}
