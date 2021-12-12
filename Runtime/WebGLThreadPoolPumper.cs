using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;

[assembly:AlwaysLinkAssembly]

namespace TransformsAI.WebGLThreadingFix
{
    [Preserve]
    public static class WebGlThreadPoolPumper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void StartPumping()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer || Application.isEditor) return;

            var dispatchMethod = Type
                .GetType("System.Threading.ThreadPoolWorkQueue")?
                .GetMethod("Dispatch", BindingFlags.NonPublic | BindingFlags.Static)?
                .CreateDelegate(typeof(Func<bool>)) as Func<bool>;

            Pump(dispatchMethod);
        }
    
        private static void Pump(object dispatchMethod)
        {
            var method = (Func<bool>)dispatchMethod;
            var didFinishWork = method();
            SynchronizationContext.Current.Post(Pump, dispatchMethod);
        }
    }
}

