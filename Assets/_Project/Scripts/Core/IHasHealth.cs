namespace Arena.Core
{
    // „Mam życie” — wspólny punkt zaczepienia dla modułów, które nie znają się nawzajem.
    // Gameplay (wróg, gracz) to implementuje; Presentation (błysk) i pociski tylko tego szukają.
    public interface IHasHealth
    {
        Health Health { get; }
    }
}
