using Robust.Client.GameObjects;
using Robust.Shared.Timing;
using Content.Shared.Forensics;
using Robust.Client.UserInterface;

namespace Content.Client.Medical
{
    public sealed class AutopsyScannerBoundUserInterface : BoundUserInterface
    {
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        [ViewVariables]
        private AutopsyScannerMenu? _window;

        [ViewVariables]
        private TimeSpan _printCooldown;

        public AutopsyScannerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();
            _window = this.CreateWindow<AutopsyScannerMenu>();
            _window.Print.OnPressed += _ => Print();
            _window.Clear.OnPressed += _ => Clear();
        }

        private void Print()
        {
            SendMessage(new AutopsyScannerPrintMessage());

            if (_window != null)
                _window.UpdatePrinterState(true);

            // This UI does not require pinpoint accuracy as to when the Print
            // button is available again, so spawning client-side timers is
            // fine. The server will make sure the cooldown is honored.
            Timer.Spawn(_printCooldown, () =>
            {
                if (_window != null)
                    _window.UpdatePrinterState(false);
            });
        }

        private void Clear()
        {
            SendMessage(new AutopsyScannerClearMessage());
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (_window == null)
                return;

            if (state is not AutopsyScannerBoundUserInterfaceState cast)
                return;

            _printCooldown = cast.PrintCooldown;

            // TODO: Fix this
            if (cast.PrintReadyAt > _gameTiming.CurTime)
                Timer.Spawn(cast.PrintReadyAt - _gameTiming.CurTime, () =>
                {
                    if (_window != null)
                        _window.UpdatePrinterState(false);
                });

            _window.UpdateState(cast);
        }
    }
}
