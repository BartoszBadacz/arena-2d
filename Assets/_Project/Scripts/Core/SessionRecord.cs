namespace Arena.Core
{
    // Rekord sesji: najwyższy wynik punktowy. Ma przeżyć restart (przeładowanie sceny), ale nie zamknięcie gry —
    // dlatego instancję trzyma GameBootstrap w polu static. Sama klasa nie ma statyków, więc testy są niezależne.
    public sealed class SessionRecord
    {
        public int BestScore { get; private set; }

        // true = nowy rekord (remis rekordem nie jest).
        public bool Submit(int score)
        {
            if (score <= BestScore) return false;
            BestScore = score;
            return true;
        }
    }
}
