using Spectrum.Resonator.Models;
using Spectrum.Resonator.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Providers
{
    public class StatusBarDataProvider : IStatusBarDataProvider
    {
        public StatusBarData Data { get; } = new StatusBarData();

        public void SetActionInfo(string newActionInfo)
        {
            Data.ActionInfo = newActionInfo;
        }

        public void SetDetailedStatus(string newDetailedStatus)
        {
            Data.DetailedStatus = newDetailedStatus;

        }
    }
}
