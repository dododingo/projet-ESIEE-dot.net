using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Noeud
    {
        public char lettre;
        public int occurence;
        public Noeud noeudGauche;
        public Noeud noeudDroite;

        public Noeud(int _occurence, Noeud _noeudGauche, Noeud _noeudDroite)
        {
            occurence = _occurence;
            noeudGauche = _noeudGauche;
            noeudDroite = _noeudDroite;
        }

        public Noeud(char _lettre, int _occurence)
        {
            lettre = _lettre;
            occurence = _occurence;
        }
    }
}
