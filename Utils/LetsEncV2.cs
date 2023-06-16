using Caster.Modules;
using static Caster.Utils.GlobalVariables;
using Certes.Acme;
using Certes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


namespace Caster.Utils
{
    internal class LetsEncV2
    {

        public static async Task RegisterCertificate(CredentialInfo credential)
        {
            var domain = credential.Domain;
            var email = credential.Email;
            // register account
            var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var account = await acme.NewAccount(email, true);

            // place order
            var order = await acme.NewOrder(new[] { domain });
            
            // Challange
            var authz = (await order.Authorizations()).First();
            var dnsChallenge = await authz.Dns();
            var dnsTxt = acme.AccountKey.DnsTxt(dnsChallenge.Token);
            var dnsUtil = new DNSUtil();

            // update dns record
            CredentialInfo decryptCredential = credential.DecryptThisRecord();
            string common_domain = decryptCredential.Domain.Replace("*.", "");
            string res = await dnsUtil.CreateCloudflareDNSTXTRecord(decryptCredential.ZoneId, decryptCredential.Email, decryptCredential.CF_token, "_acme-challenge." + common_domain, dnsTxt);
            await Task.Delay(10000);


            // wait for challenge to complete
            bool validationSucceeded = false;
            int attempts = 0;

            while (!validationSucceeded && attempts < 3)
            {
                try
                {
                    await dnsChallenge.Validate();

                    // If we reach this line, it means the Validate method did not throw an exception, so validation succeeded
                    validationSucceeded = true;
                }
                catch
                {
                    // Validation failed, so increment the attempts counter and wait 60 seconds before the next attempt
                    attempts++;
                    await Task.Delay(10000);
                }
            }

            if (!validationSucceeded)
            {
                throw new Exception("Validation failed after 3 attempts.");
            }

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);

            CertificateChain cert = null;
            for(int i = 0; i < 3; i++) {                 
                try
                {
                    cert = await order.Generate(new CsrInfo
                    {
                        CommonName = domain
                    }, privateKey);
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    await Task.Delay(10000);
                }
            }

            File.WriteAllText(Path.Combine(PROJ_DIR, "Data", "ssl" ,"fullchain.pem"), cert.ToPem());
            File.WriteAllText(Path.Combine(PROJ_DIR, "Data", "ssl", "privkey.pem"), privateKey.ToPem());
        }
    }
}
