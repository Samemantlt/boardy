using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Models.Secrets;

namespace TheLiar.Api.Domain.Models;

public record GameStateGlobals(
    Room Room,
    TimeoutOptions TimeoutOptions,
    Action<IEvent> RaiseEvent,
    Func<ISecret> CreateSecret);