// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Buildvana.Sdk.Tasks.Base;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks;

public sealed class HashWithMetadata : BuildvanaSdkTask
{
    [PublicAPI]
#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild APIs
    public ITaskItem[]? ItemsToHash { get; set; }
#pragma warning restore CA1819

    [PublicAPI]
    [Output]
    public string? HashResult { get; set; }

    protected override void Run()
    {
        if (ItemsToHash is null)
        {
            return;
        }

        var (buffer, count) = SerializeItems(ItemsToHash);
        var hashBytes = Hash(buffer, count);
        HashResult = BytesToHexString(hashBytes);
    }

    private static (byte[] Buffer, int Count) SerializeItems(IEnumerable<ITaskItem> items)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
        foreach (var item in items)
        {
            writer.Write('(');
            writer.Write(item.ItemSpec);
            foreach (var name in item.MetadataNames.OfType<string>())
            {
                writer.Write(';');
                writer.Write(name);
                writer.Write('=');
                writer.Write(item.GetMetadata(name) ?? string.Empty);
            }

            writer.Write(')');
        }

        return (stream.GetBuffer(), (int)stream.Position);
    }

    private static byte[] Hash(byte[] buffer, int count)
    {
        using var algorithm = SHA512.Create();
        return algorithm.ComputeHash(buffer, 0, count);
    }

    private static string BytesToHexString(IReadOnlyCollection<byte> bytes)
    {
        const string digits = "0123456789abcdef";
        var sb = new StringBuilder(2 * bytes.Count);
        foreach (var b in bytes)
        {
            _ = sb.Append(digits[b >> 4]).Append(digits[b & 0x0F]);
        }

        return sb.ToString();
    }
}
