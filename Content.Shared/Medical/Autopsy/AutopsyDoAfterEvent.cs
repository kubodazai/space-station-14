using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Autpsy;

[Serializable, NetSerializable]
public sealed partial class AutopsyScannerDoAfterEvent : SimpleDoAfterEvent
{
}
