namespace SportsLeague.Domain.Interfaces.Services;

public interface IStandingsService
{
    // keyword o palabra reservada "object"
    // se utiliza para indicar que el método puede devolver
    // cualquier tipo de dato. En este caso, se espera que el
    // método devuelva un objeto que contenga la información de
    // la tabla de posiciones, los maximos goleadores o las estadisticas de tarjetas,
    // dependiendo del método especifico.
    Task<object> GetStandingsAsync(int tournamentId); // Obtener tabla de posiciones para un torneo específico
    Task<object> GetTopScorersAsync(int tournamentId); // Obtener lista de goleadores para un torneo específico
    Task<object> GetCardStatsAsync(int tournamentId); // Obtener estadísticas de tarjetas para un torneo específico
}
