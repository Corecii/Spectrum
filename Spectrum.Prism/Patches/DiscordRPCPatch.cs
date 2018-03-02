using Mono.Cecil;
using Spectrum.Prism.Runtime;
using Spectrum.Prism.Runtime.EventArgs;
using System;
using System.Linq;

namespace Spectrum.Prism.Patches
{
    public class DiscordRPCPatch : BasePatch
    {
        public override string Name => "DiscordRPC";
        public override bool NeedsSource => false;

        public override void Run(ModuleDefinition moduleDefinition)
        {
            try
            {
                var targetType = moduleDefinition.GetType(Resources.DiscordControllerTypeName);
                var methodDefinition = targetType.Methods.Single(m => m.Name == Resources.DiscordControllerStartMethodName);

                for (var i = 0; i < 9; i++)
                {
                    methodDefinition.Body.Instructions.RemoveAt(4);
                }

                OnPatchSucceeded(this, new PatchSucceededEventArgs(Name));
            }
            catch (Exception ex)
            {
                OnPatchFailed(this, new PatchFailedEventArgs(Name, ex));
            }
        }

        public override void Run(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            OnPatchFailed(this, new PatchFailedEventArgs(Name, new Exception("This patch does not require any source modules.")));
        }
    }
}
