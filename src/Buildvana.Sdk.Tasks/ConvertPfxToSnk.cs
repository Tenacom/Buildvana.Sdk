// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Buildvana.Sdk.Tasks.Base;
using Buildvana.Sdk.Tasks.Internal;
using Buildvana.Sdk.Tasks.Resources;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks;

public sealed class ConvertPfxToSnk : BuildvanaSdkTask
{
    [Required]
    public string PfxPath { get; set; } = null!;

    [Required]
    public string? PfxPassword { get; set; }

    [Required]
    public string OutputPath { get; set; } = null!;

    protected override void Run()
    {
        if (string.IsNullOrEmpty(PfxPath))
        {
            throw new BuildErrorException(Strings.MissingParameterFmt, nameof(PfxPath));
        }

        if (string.IsNullOrEmpty(OutputPath))
        {
            throw new BuildErrorException(Strings.MissingParameterFmt, nameof(OutputPath));
        }

        using var cert = LoadCertificate(PfxPath, PfxPassword);
        var keyBytes = ExtractPrivateKey(cert, PfxPath);
        SaveBytes(OutputPath, keyBytes!);
    }

    private static X509Certificate2 LoadCertificate(string path, string? password)
    {
        try
        {
            return new X509Certificate2(path, password, X509KeyStorageFlags.Exportable);
        }
        catch (CryptographicException)
        {
            throw new BuildErrorException(Strings.AssemblySigning.CannotExtractCertificateFmt, path);
        }
    }

    private static byte[] ExtractPrivateKey(X509Certificate2 certificate, string certificatePath)
    {
        if (certificate.GetRSAPrivateKey() is not RSACryptoServiceProvider privateKey)
        {
            throw new BuildErrorException(Strings.AssemblySigning.MissingRsaPrivateKeyFmt, certificatePath);
        }

        return privateKey.ExportCspBlob(true);
    }

    private static void SaveBytes(string outputPath, byte[] bytes)
    {
        try
        {
            // Overwrites file if it already exists (and can be overwritten)
            File.WriteAllBytes(outputPath, bytes);
        }
        catch (Exception e) when (e.IsIORelatedException())
        {
            throw new BuildErrorException(Strings.CouldNotWriteFileFmt, outputPath, e.Message);
        }
    }
}
