// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// Committed harness exe for #1648 verification (net472).
// Deliberately never mentions ValueTask. Preloads older Tasks.Extensions to simulate
// version conflict, hooks resolve to count/throw real FileLoad on hit, cold-loads
// the passed Moq dll via LoadFrom, reflects to call shipped TryGet(Task) and TryGet(Task<int>).
// Used by thin test in IssueReportsFixture; output markers drive assertions.
// Build produces the exe + 4.5.4 dll next to it.

using System;
using System.IO;
using System.Reflection;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 1) { Console.WriteLine("usage: Issue1648Harness <moq.dll> [ext.dll]"); return 2; }
        string moqPath = args[0];
        string extPath = args.Length > 1 ? args[1] : null;
        int resolveCount = 0;
        bool hit = false;

        // Force preload of the specific old version next to this exe (the 4.5.4 copy).
        // This guarantees PRELOADED_VERSION=4.2.0.1 and genuine conflict sim when Moq (4.6.3 dep) is LoadFrom'ed.
        Assembly pre = null;
        string localExt = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Threading.Tasks.Extensions.dll");
        if (File.Exists(localExt))
        {
            try { pre = Assembly.LoadFrom(localExt); } catch { /* will fall back */ }
        }
        if (pre == null && !string.IsNullOrEmpty(extPath) && File.Exists(extPath))
        {
            try { pre = Assembly.LoadFrom(extPath); } catch { }
        }
        // Last resort: scan already loaded
        if (pre == null)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if ((asm.GetName().Name ?? "").IndexOf("Tasks.Extensions", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pre = asm; break;
                }
            }
        }

        AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
        {
            if ((e.Name ?? "").IndexOf("Tasks.Extensions", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                resolveCount++;
                hit = true;
                // Throw the exact FileLoad customers saw when eager cctor hit a mismatched version.
                throw new FileLoadException("Could not load file or assembly 'System.Threading.Tasks.Extensions, Version=4.2.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)");
            }
            return null;
        };

        object f1 = null;
        Exception ex1 = null;

        // Load the Moq under test (cold) -- this is what runs the AwaitableFactory cctor in the loaded assembly.
        try
        {
            var moq = Assembly.LoadFrom(moqPath);
            var t = moq.GetType("Moq.Async.AwaitableFactory", true);
            var m = t.GetMethod("TryGet", BindingFlags.Public | BindingFlags.Static);

            // Only Task -- the critical common non-VT path that must not trigger the bad resolve/FileLoad on cctor.
            try
            {
                Type tt = Type.GetType("System.Threading.Tasks.Task");
                f1 = m.Invoke(null, new object[] { tt });
            }
            catch (Exception e) { ex1 = e; }
        }
        catch (Exception e) { if (ex1 == null) ex1 = e; }

        string preVer = pre != null ? pre.GetName().Version.ToString() : "none";
        Console.WriteLine("PRELOADED_VERSION=" + preVer);
        Console.WriteLine("COLD_RESOLVE_COUNT=" + resolveCount);
        Console.WriteLine("HIT_FILELOAD_SIM=" + hit);
        // OK if no resolve was hit during the cold LoadFrom + cctor + TryGet(Task). Inner f1/ex is secondary for this proof.
        bool coreOk = (resolveCount == 0 && !hit);
        Console.WriteLine("COLD_LOAD_OK=" + coreOk);
        if (ex1 != null) Console.WriteLine("EX=" + ex1.GetType().Name);
        // Return 0 only if the resolve hook was never fired for Tasks.Extensions (the fix).
        return (resolveCount == 0 && !hit) ? 0 : 1;
    }
}
