#line 2 "ConvertPfxToSnk.cs"

using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Buildvana.Tasks
{
    public sealed class ConvertPfxToSnk : Task
    {
        [Required]
        public string PfxPath { get; set; }

        [Required]
        public string PfxPassword { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            try
            {
                try
                {
                    var cert = new X509Certificate2(PfxPath, PfxPassword, X509KeyStorageFlags.Exportable);
                }
                catch (CryptographicException)
                {
                    Log.LogError(null, "BVE1200", null, "ConvertPfxToSnk.cs", 0, 0, 0, 0, $"Cannot extract key from '{PfxPath}'.");
                    return false;
                }

                var privateKey = (RSACryptoServiceProvider)cert.PrivateKey;
                var keyBytes = privateKey.ExportCspBlob(true);
                File.WriteAllBytes(OutputPath, keyBytes);
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, true, true, "ConvertPfxToSnk.cs");
            }

            return !Log.HasLoggedErrors;
        }
    }
}