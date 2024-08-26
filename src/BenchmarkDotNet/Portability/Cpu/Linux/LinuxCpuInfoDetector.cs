﻿using System.Collections.Generic;
using BenchmarkDotNet.Helpers;

namespace BenchmarkDotNet.Portability.Cpu.Linux;

/// <summary>
/// CPU information from output of the `cat /proc/cpuinfo` and `lscpu` command.
/// Linux only.
/// </summary>
internal class LinuxCpuInfoDetector : ICpuInfoDetector
{
    public bool IsApplicable() => RuntimeInformation.IsLinux();

    public CpuInfo? Detect()
    {
        if (!IsApplicable()) return null;

        // lscpu output respects the system locale, so we should force language invariant environment for correct parsing
        var languageInvariantEnvironment = new Dictionary<string, string>
        {
            ["LC_ALL"] = "C",
            ["LANG"] = "C",
            ["LANGUAGE"] = "C"
        };

        string cpuInfo = ProcessHelper.RunAndReadOutput("cat", "/proc/cpuinfo") ?? "";
        string lscpu = ProcessHelper.RunAndReadOutput("/bin/bash", "-c \"lscpu\"", environmentVariables: languageInvariantEnvironment);
        return LinuxCpuInfoParser.Parse(cpuInfo, lscpu);
    }
}