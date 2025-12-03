///////////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

namespace viperOSK
{
    /// <summary>
    /// interface for cursor module to allow implementation for both Unity UI and non-UI cursor implementations
    /// </summary>
    public interface I_OSK_Cursor
    {

        void Cursor();
        void Show(bool show);
    }
}
