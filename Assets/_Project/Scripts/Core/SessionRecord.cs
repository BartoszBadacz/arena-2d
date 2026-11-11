namespace Arena.Core
{
    // Rekord sesji: najwyższa osiągnięta fala. Ma przeżyć restart (przeładowanie sceny), ale nie zamknięcie gry —
    // dlatego instancję trzyma GameBootstrap w polu static. Sama klasa nie ma statyków, więc testy są niezależne.
    public sealed class SessionRecord
    {
        public int BestWave { get; private set; }

        // true = nowy rekord (remis rekordem nie jest).
        public bool Submit(int wave)
        {
            if (wave <= BestWave) return false;
            BestWave = wave;
            return true;
        }
    }
}
