using System;
using System.Collections.Generic;
using System.IO;
using Caster.Modules;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using static Caster.Utils.GlobalVariables;
namespace Caster.Utils
{
    internal class ZeroSSL
    {
        public void GenerateCSR(string commonName)
        {

            string path = Path.Combine(PROJ_DIR, "Data", commonName, "csr.txt");

            var randomGenerator = new SecureRandom();
            var keyGenerationParameters = new KeyGenerationParameters(randomGenerator, 2048);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var keyPair = keyPairGenerator.GenerateKeyPair();

            var subject = new X509Name("CN=" + commonName);
            var signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private, randomGenerator);

            var csr = new Pkcs10CertificationRequest(signatureFactory, subject, keyPair.Public, null, keyPair.Private);

            using (var sw = new StringWriter())
            {
                var writer = new PemWriter(sw);
                writer.WriteObject(csr);

                // 写入到文件中

                File.WriteAllText(path, sw.ToString());
            }
        }
        public void GenerateCertificate()
        {

        }
    }
}
