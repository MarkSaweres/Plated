using Plated.Core.Models;

namespace Plated.Core.Services;

public record RecentPlate(string State, string Number)
{
    public string StateName => UsStates.NameOf(State);
}

public interface IRecentPlatesService
{
    IReadOnlyList<RecentPlate> Load();

    /// <summary>Moves (or adds) the plate to the front and returns the updated list.</summary>
    IReadOnlyList<RecentPlate> Add(string state, string number);
}
