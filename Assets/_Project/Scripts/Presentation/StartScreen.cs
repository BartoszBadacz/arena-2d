using Arena.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Arena.Presentation
{
    // Ekran startowy („Kliknij, aby zacząć”) — tylko przy pierwszym biegu w sesji.
    // Klik to polecenie dla logiki (RunSession.Begin); panel chowa się na fakt Started.
    // W przeglądarce klik daje też fokus ramce gry (klawiatura) i odblokowuje dźwięk.
    [RequireComponent(typeof(UIDocument))]
    public class StartScreen : MonoBehaviour
    {
        private const string HiddenClass = "start--hidden";

        private RunSession session;
        private VisualElement panel;

        public void Init(RunSession session)
        {
            this.session = session;
            session.Started += Hide;
        }

        // Start, nie Awake: UIDocument buduje drzewo elementów w swoim OnEnable.
        private void Start()
        {
            if (session == null)
            {
                Debug.LogError($"{nameof(StartScreen)}: brak Init — czy w scenie jest GameBootstrap?", this);
                enabled = false;
                return;
            }

            panel = GetComponent<UIDocument>().rootVisualElement.Q("startPanel");
            panel.EnableInClassList(HiddenClass, session.HasStarted);
            panel.RegisterCallback<PointerDownEvent>(_ => session.Begin());
        }

        private void OnDestroy()
        {
            if (session != null) session.Started -= Hide;
        }

        private void Hide() => panel?.AddToClassList(HiddenClass);
    }
}
