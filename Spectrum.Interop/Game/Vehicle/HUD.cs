using Spectrum.Interop.Helpers;
using UnityEngine;

namespace Spectrum.Interop.Game.Vehicle
{
    public class HUD
    {
        private static HoverScreenEmitter HoverScreenEmitter { get; set; }
        private static bool CanOperateOnHoverScreen => HoverScreenEmitter != null;

        internal HUD() { }

        public void SetHUDText(string text, float displayTime, int priority = -1)
        {
            UpdateParentObject();

            if (CanOperateOnHoverScreen)
            {
                HoverScreenEmitter.SetTrickText(new TrickyTextLogic.TrickText(displayTime, priority, TrickyTextLogic.TrickText.TextType.standard, text));
            }
        }

        public void SetHUDText(string text)
        {
            SetHUDText(text, 3.0f);
        }

        public void Clear()
        {
            HoverScreenEmitter hse = GameObject.Find("LocalCar")?.GetComponent<HoverScreenEmitter>();
            HoverScreenParent hsp = Reflection.GetPrivate<HoverScreenParent>(hse, "hoverScreenParent_");
            TrickyTextLogic ttl = hsp.rightTrickyTextObj_.GetComponent<TrickyTextLogic>();
            Reflection.GetPrivate<PriorityQueue<TrickyTextLogic.TrickText>>(ttl, "textList_").Clear();
        }

        private void UpdateParentObject()
        {
            var localCar = Utilities.FindLocalCar();
            HoverScreenEmitter = localCar?.GetComponent<HoverScreenEmitter>();
        }
    }
}
