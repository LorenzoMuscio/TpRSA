using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TP_RSA
{
    //Je laisse certain Console.WriteLine dans le code si ça peut être utile pour la correction
    public class MyRSA
    {
        public string fileName = "";
        public MyRSA()
        {
            Console.WriteLine(" Script MonRSA by Lorenzo Muscio:\n" +
                " Syntaxe:\n" +
                " MonRSA <commande> [<clé>] [<texte>] [switchs]\n\n" +
                " Commande :\n" +
                "    keygen : Génére une paire de clés (les options <clé> et <texte> n'ont pas lieu d'être ici)\n" +
                "    crypt : Chiffre le <text> avec la clé publique <clé>\n" +
                "    decrypt : Déchiffre le <text> avec la clé privée <clé>\n" +
                "    help (ou aucun parametre spécifié) : affiche de nouveau ce manuel\n\n" +
                " Clé :\n" +
                "    Fichier contenant la clé public (.pub) si l'on fait crypt ou la clé privé (.priv) si l'on fait decrypt\n" +
                "    Veuillez indiquer la clé par son chemin complet (par exemple -> [C:\\User\\Documents]).\n\n" +
                " Texte :\n" +
                "    Texte à chiffrer si l'on utilise crypt ou texte chiffré à déchiffrer si l'on utilise decrypt\n\n" +
                " Switchs :\n" +
                "    '-f file' avec 'file' etant le nom que l'on souhaite donner au clé générés (monRSA.pub et monRSA.priv par défaut)\n\n");
        }

        public string Keygen(string fileName=null)
        {
            keygenerator keygenerator = new keygenerator();
            long p, q, n, nprime;
            List<long> keys = new List<long>();

            try
            {
                //On genère 2 nombres premiers de 10 charactères
                do
                {
                    p = keygenerator.Generator10char();
                    q = keygenerator.Generator10char();
                } while (p == 0 || q == 0);

                Console.WriteLine("p : " + p + "\n" + "q : " + q);

                n = p * q;
                nprime = (p-1) * (q-1);

                //Pour des raison obscure, n et nprime sont parfois negatif. Ici Je fait en sorte de les repasser en positifs lorsque c'est le cas
                if (n < 0)
                    n = n * (-1);
                if (nprime < 0)
                    nprime = nprime * (-1);

                Console.WriteLine("n : " + n + "\n" + "nprime : " + nprime);

                keys = Decompose(nprime);

                PublicKeyGen(fileName, n, keys);
                PrivateKeyGen(fileName, n, keys);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return fileName;
        }

        public void Crypt(string keysPath, string textToCrypt, string fileName)
        {
            try
            {
                keysPath = keysPath.Trim() + @"\paramétre2.pub";
                var fileText = File.ReadAllText(keysPath);
                var nEncode = "";
                var eEncode = "";
                int n;
                int e;

                if (true)
                {
                    string contentFile = File.ReadAllText(keysPath);
                    contentFile = contentFile.Replace("---begin " + fileName + " public key---\r\n", "");
                    contentFile = contentFile.Replace("\r\n---end " + fileName + " key---\r\n", "");
                    string[] contentFileArray = contentFile.Split(", ");

                    var nString = Convert.FromBase64String(contentFileArray[0]);
                    var eString = Convert.FromBase64String(contentFileArray[1]);
                    nEncode = Encoding.UTF8.GetString(nString);
                    eEncode = Encoding.UTF8.GetString(eString);
                    n = (int)Convert.ToInt64(nEncode);
                    e = (int)Convert.ToInt64(eEncode);

                    //Console.WriteLine("e : " + e);
                    //Console.WriteLine("n : " + n);

                    Byte[] encodingInASCII = Encoding.ASCII.GetBytes(textToCrypt);
                    List<int> encodedBlock = new List<int>();
                    string cryptogramme = "";

                    for (var i = 0; i < encodingInASCII.Length; i++)
                    {
                        //Console.WriteLine(encodingInASCII[i]);

                        encodedBlock.Add(encodingInASCII[i] ^ e % n);

                        cryptogramme = cryptogramme + encodedBlock[i].ToString("0000");
                    };

                    Console.WriteLine("Votre texte chiffré : " + cryptogramme);
                }
            }
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }
        }
        
        public void Decrypt(string keysPath, string textToDecrypt, string fileName)
        {
            keysPath = keysPath + @"\paramétre2.priv";
            File.SetAttributes(keysPath, FileAttributes.Normal);
            StreamReader sr = new StreamReader(keysPath);
            var nDecode = "";
            var dDecode = "";
            long n;
            long d;

            if (sr.ReadToEnd().Contains("---begin " + fileName + " private key---"))
            {
                string contentFile = File.ReadAllText(keysPath);
                contentFile = contentFile.Replace("---begin " + fileName + " private key---\r\n", "");
                contentFile = contentFile.Replace("\r\n---end " + fileName + " key---\r\n", "");
                string[] contentFileArray = contentFile.Split(", ");

                var nString = Convert.FromBase64String(contentFileArray[0]);
                var dString = Convert.FromBase64String(contentFileArray[1]);
                nDecode = Encoding.UTF8.GetString(nString);
                dDecode = Encoding.UTF8.GetString(dString);
                n = long.Parse(nDecode);
                d = long.Parse(dDecode);

                Console.WriteLine("d : " + d);
                Console.WriteLine("n : " + n);

                List<string> listChunckCrypto = new List<string>();

                for (var j = 0; j < textToDecrypt.Length; j+=4)
                {
                    listChunckCrypto.Add(textToDecrypt.Substring(j, 4));
                }

                //string decodeASCII = Encoding.ASCII.GetBytes(textToDecrypt);
                List<long> decodedBlock = new List<long>();
                string text = "";
                //string cryptogramme = "";

                for (var i = 0; i < listChunckCrypto.Count; i++)
                {
                    long listChunckCryptoInt = long.Parse(listChunckCrypto[i]);
                    Console.WriteLine(listChunckCrypto[i]);

                    decodedBlock.Add(listChunckCryptoInt ^ d % n);

                    long unicode = decodedBlock[i];
                    char character = (char)unicode;
                    text = text + character.ToString();
                };

                Console.WriteLine("Votre texte déchiffré : " + text);
            }
        }

        private List<long> Decompose(long nprime)
        {
            long d, ed;
            List<long> listPrimeE = new List<long>();
            List<long> cle = new List<long>();

            try
            {
                listPrimeE = ListOfPrime();

                for (ed = listPrimeE.Count; ed > 0; ed--)
                {
                    for (var i = 0; i < ed; i++)
                    {
                        if (ed % listPrimeE[i] == 0 && nprime % ed == 0)
                        {
                            d = ed / listPrimeE[i];
                            if (d != listPrimeE[i] && listPrimeE[i] != 1 && d != 1 && ed / d == listPrimeE[i])
                            {
                                cle.Add(listPrimeE[i]);
                                cle.Add(d);

                                Console.WriteLine("ed : " + ed);
                                Console.WriteLine("e : " + cle[0]);
                                Console.WriteLine("d : " + cle[1]);

                                return cle;
                            }
                        }
                    }
                }
                Console.WriteLine("Aucune clé n'a pu être généré, veuillez réessayer");
                Keygen(fileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return cle;
        }

        private List<long> ListOfPrime()
        {
            var limit = 10000;
            List<long> primes = new List<long>();
            primes.Add(3);
            int nextPrime = 5;
            while (primes.Count < limit)
            {
                int sqrt = (int)Math.Sqrt(nextPrime);
                bool isPrime = true;
                for (int i = 0; (int)primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes;
        }

        private void PublicKeyGen(string fileName, long n, List<long> keys)
        {
            try
            {
                Console.WriteLine("Indiquer le chemin de dossier où le fichier paramétre2.pub doit être enregisté");
                string executableLocation = Console.ReadLine();
                string path = Path.Combine(executableLocation, "paramétre2.pub");

                using (StreamWriter sw = File.CreateText(path))
                {
                    var nBytes = Encoding.UTF8.GetBytes(n.ToString());
                    var eBytes = Encoding.UTF8.GetBytes(keys[0].ToString());

                    sw.WriteLine("---begin " + fileName + " public key---");
                    sw.WriteLine(Convert.ToBase64String(nBytes) + ", " + Convert.ToBase64String(eBytes).ToString());
                    sw.WriteLine("---end " + fileName + " key---");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite");
                PublicKeyGen(fileName, n, keys);
            }

        }

        private void PrivateKeyGen(string fileName, long n, List<long> keys)
        {
            try
            {
                Console.WriteLine("Indiquer le chemin de dossier où le fichier paramétre2.priv doit être enregisté");
                string executableLocation = Console.ReadLine();
                
                string path = Path.Combine(executableLocation, "paramétre2.priv");

                using (StreamWriter sw = File.CreateText(path))
                {
                    var nBytes = Encoding.UTF8.GetBytes(n.ToString());
                    var dBytes = Encoding.UTF8.GetBytes(keys[1].ToString()); //(Le nom de cette variable n'est pas un jeu de mot)

                    sw.WriteLine("---begin " + fileName + " private key---");
                    sw.WriteLine(Convert.ToBase64String(nBytes) + ", " + Convert.ToBase64String(dBytes).ToString());
                    sw.WriteLine("---end " + fileName + " key---");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite");
                PrivateKeyGen(fileName, n, keys);
            }
        }

    }
}
