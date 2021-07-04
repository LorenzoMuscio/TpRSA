using System;
using System.Collections.Generic;

namespace TP_RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            MyRSA rsa = new MyRSA();

            bool onMenu = true;
            string fileName = "monRSA";
            
            do
            {
                string userSelection = Console.ReadLine();
                var keyPath = "";
                var textToCrypt = "";
                var textToDecrypt = "";

                if (userSelection.ToLower().StartsWith("monrsa keygen -f "))
                {
                    fileName = userSelection.ToLower().Remove(0, 16);
                    fileName = fileName.Replace(" ", "_");
                    userSelection = "monrsa keygen";
                }
                
                if (userSelection.ToLower() == "monrsa crypt" || userSelection.ToLower() == "monrsa crypt " || userSelection.ToLower() == "monrsa decrypt" || userSelection.ToLower() == "monrsa decrypt ")
                {
                    Console.WriteLine(" Pour chiffrer (crypt) ou déchiffrer (decrypt), les paramètres [<clé>] et [<texte>] sont obligatoires.\n" +
                        "Mettez ces paramètres entre chevrons (par exemple -> <Un Texte ...>). Voir l'aide ci-dessous :\n");
                    userSelection = "monrsa";
                }
                
                if (userSelection.ToLower().StartsWith("monrsa crypt <"))
                {
                    var parameters = userSelection.ToLower().Remove(0, 12);

                    if (parameters.Contains("> <"))
                    {
                        string[] parametersArray = parameters.Split("> <");
                        keyPath = parametersArray[0].Replace("<", "").Trim();
                        textToCrypt = parametersArray[1].Replace(">", "").Trim();
                        userSelection = "monrsa crypt";
                    }
                    else
                    {
                        Console.WriteLine("Tout les paramètres ne sont pas présent. Voir l'aide ci-dessous :\n");
                        userSelection = "monrsa";
                    }
                }
                
                if (userSelection.ToLower().StartsWith("monrsa decrypt <"))
                {
                    var parameters = userSelection.ToLower().Remove(0, 14);

                    if (parameters.Contains("> <"))
                    {
                        string[] parametersArray = parameters.Split("> <");
                        keyPath = parametersArray[0].Replace("<", "").Trim();
                        textToDecrypt = parametersArray[1].Replace(">", "").Trim();
                        userSelection = "monrsa decrypt";
                    }
                    else
                    {
                        Console.WriteLine("Tout les paramètres ne sont pas présent. Voir l'aide ci-dessous :\n");
                        userSelection = "monrsa";
                    }
                }

                switch (userSelection.ToLower())
                {
                    case "monrsa":
                        new MyRSA();
                        break;

                    case "monrsa help":
                        new MyRSA();
                        break;

                    case "monrsa keygen":
                        if (fileName.Length != 0)
                            rsa.Keygen(fileName);
                        else
                            rsa.Keygen("monRSA");
                        Console.WriteLine("\n");
                        break;

                    case "monrsa crypt":
                        rsa.Crypt(keyPath, textToCrypt, fileName);
                        Console.WriteLine("\n");
                        break;

                    case "monrsa decrypt":
                        rsa.Decrypt(keyPath, textToDecrypt, fileName);
                        Console.WriteLine("\n");
                        break;

                    default:
                        Console.WriteLine("Utiliser des termes valides. Vous pouvez executer MyRSA ou MyRSA help pour consulter le manuel d'utilisation\n");
                        break;
                }
            } while (onMenu == true);
        }
    }
}
