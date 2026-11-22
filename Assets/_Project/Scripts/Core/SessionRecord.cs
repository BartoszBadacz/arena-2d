namespace Arena.Core
{
    // Sesja gry (od uruchomienia do zamknięcia): najwyższy wynik punktowy i liczba rozegranych biegów.
    // Ma przeżyć restart (przeładowanie sceny), ale nie zamknięcie gry —
    // dlatego instancję trzyma GameBootstrap w polu static. Sama klasa nie ma statyków, więc testy są niezależne.
    public sealed class SessionRecord
    {
        public int BestScore { get; private set; }
        public int RunsPlayed { get; private set; } // 0 = pierwszy bieg w sesji (ekran startowy)

        // Zgłoszenie wyniku zakończonego biegu. true = nowy rekord (remis rekordem nie jest).
        public bool Submit(int score)
        {
            RunsPlayed++;
            if (score <= BestScore) return false;
            BestScore = score;
            return true;
        }
    }
}
