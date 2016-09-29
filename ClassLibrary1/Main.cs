using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huffman;

namespace ClassLibrary
{
    public class Huffman : MarshalByRefObject, IPlugin
    {
        public Dictionary<char, List<bool>> codage;
        public List<KeyValuePair<byte, int>> frequency;
        public Noeud huffmanTable;
        public List<Noeud> charList;

        public string PluginName{ get { return "HuffmanTest"; } set { } }

        ///<summary>
        /// compresse le texte contenu dans la structure HuffmanData
        ///</summary>
        public bool Compress(ref HuffmanData data)
        {
            string text = data.uncompressedData.ToString();
            createList(text);
            tri(charList);
            createHuffmanTable(charList);
            createCode(huffmanTable, new List<bool>());

            data.compressedData = encode(text);
            data.frequency = frequency;
            data.sizeOfUncompressedData = data.uncompressedData.Length;

            printHuffman(huffmanTable);     //debug
            Console.Write(encode(text));    //debug

            return true;
        }

        ///<summary>
        /// compresse le texte contenu dans la structure HuffmanData
        ///</summary>
        public bool Decompress(ref HuffmanData data)
        {
            return true;
        }

        ///<summary>
        /// créé la liste de noeud à partir du texte
        ///</summary>
        public void createList(String text)
        {
            List<Noeud> charList = new List<Noeud>();
            foreach (char lettre in text)
            {
                Boolean exist = false;
                foreach (Noeud noeud in charList)
                {
                    if (noeud.lettre == lettre)
                    {
                        noeud.occurence++;
                        exist = true;
                    }
                }
                if (!exist)
                {
                    charList.Add(new Noeud(lettre, 1));
                }
            }
        }

        ///<summary>
        /// trie la liste de noeuds
        ///</summary>
        public List<Noeud> tri(List<Noeud> charList)
        {
            return charList.OrderBy(o => o.occurence).ToList();
        }

        ///<summary>
        /// huffmanise la liste de noeud, renvoie le premier noeud
        ///</summary>
        public void createHuffmanTable(List<Noeud> tab)
        {
            int level = 0;
            while (true)
            {

                //prépare le nouveau noeud, supprime les noeuds fils
                Noeud new_noeud = new Noeud(tab[0].occurence + tab[1].occurence, tab[0], tab[1]);

                tab.RemoveAt(0);
                tab.RemoveAt(0);

                //place le nouveau noeud à l'index aproprié
                Boolean inserted = false;
                foreach (Noeud noeud in tab)
                {

                    if (noeud.occurence > new_noeud.occurence)
                    {
                        tab.Insert(tab.IndexOf(noeud), new_noeud);
                        inserted = true;
                        break;
                    }

                }

                //si aucun noeud plus grand, placer le nouveau noeud à la fin
                if (!inserted)
                {
                    tab.Add(new_noeud);
                }

                //si la table ne contient qu'un seul noeud, l'opération est finie
                if (tab.Count == 1)
                {
                    huffmanTable = tab[0];
                }

                level++;
            }
        }

        ///<summary>
        /// créé la table de codage
        ///</summary>
        public void createCode(Noeud noeud, List<bool> chemin)
        {

            codage = new Dictionary<char, List<bool>>();

            if (noeud.noeudGauche != null)
            {
                List<bool> new_chemin = new List<bool>(chemin);
                new_chemin.Add(false);
                if (noeud.noeudGauche.lettre != '\0')
                {
                    codage.Add(noeud.noeudGauche.lettre, new_chemin);
                }
                createCode(noeud.noeudGauche, new_chemin);
            }
            if (noeud.noeudDroite != null)
            {
                List<bool> new_chemin = new List<bool>(chemin);
                new_chemin.Add(true);
                if (noeud.noeudDroite.lettre != '\0')
                {
                    codage.Add(noeud.noeudDroite.lettre, new_chemin);
                }
                createCode(noeud.noeudDroite, new_chemin);
            }
        }

        ///<summary>
        /// compresse le texte à partir de la table de codage, renvoie un tableau de byte (texte compressé)
        ///</summary>
        public byte[] encode(String text)
        {

            List<byte> encoded_text_l = new List<byte>();   // texte encodé sous forme de liste de byte 

            byte current_byte = 0;  //byte en cours de création
            int bit_worked = 0;     //index du bit en cours de modification dans le byte
            foreach (char c in text)
            {
                List<bool> code = codage[c];
                foreach (bool b in code)
                {
                    current_byte = (byte)((int)current_byte << 1);  //passage au bit suivant
                    if (b)
                    {
                        current_byte += 1;  //passage du bit à 1 (default 0)
                    }

                    bit_worked++;   //passage au bit suivant

                    if (bit_worked == 8) //passage au byte suivant
                    {
                        encoded_text_l.Add(current_byte);
                        bit_worked = 0;
                        current_byte = 0;
                    }
                }
                while (bit_worked !=0 )// completion du dernier byte
                {
                    current_byte = (byte)((int)current_byte << 1);
                    encoded_text_l.Add(current_byte);
                }
            }

            byte[] encoded_text = new byte[encoded_text_l.Count()]; // création du tableau de byte à retourner
            int index = 0;

            while (true) //conversion de la liste en tableau de byte
            {
                if (encoded_text_l.Count == 0)
                {
                    break;
                }
                encoded_text[index] = encoded_text_l.First();
                encoded_text_l.Remove(encoded_text_l.First());
                index++;
            }

            return encoded_text;
        }

        ///<summary>
        /// affiche dans la console le tableau de huffman
        ///</summary>
        public void printHuffman(Noeud noeud, int level = 0)
        {
            if (noeud.lettre == '\0')
            {
                Console.Write("**" + " : " + noeud.occurence + "\n");
            }
            else
            {
                String chemin = "";
                foreach (bool b in codage[noeud.lettre])
                {
                    if (b) { chemin += '1'; }
                    else { chemin += '0'; }
                }
                Console.Write(noeud.lettre + " : " + noeud.occurence + " : " + chemin + "\n");
            }

            if (noeud.noeudGauche != null)
            {
                for (int i = 0; i < level + 1; i++) { Console.Write(" "); }
                Console.Write("└ ");
                printHuffman(noeud.noeudGauche, level + 1);
            }
            if (noeud.noeudDroite != null)
            {
                for (int i = 0; i < level + 1; i++) { Console.Write(" "); }
                Console.Write("└ ");
                printHuffman(noeud.noeudDroite, level + 1);
            }
        }

    }

}
