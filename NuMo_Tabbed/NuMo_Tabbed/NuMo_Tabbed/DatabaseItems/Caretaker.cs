using System;
using System.Collections.Generic;
using System.Text;

namespace NuMo_Tabbed
{
    class Caretaker
    {
        private static Caretaker caretaker;
        private Memento savepoint;


        private Caretaker()
        {
            savepoint = null;
        }

        public static Caretaker getCaretaker()
        {
            if (caretaker == null)
            {
                caretaker = new Caretaker();
            }
            return caretaker;
        }

        public void addMemento(Memento save)
        {
            savepoint = save;
        }

        public Memento getMemento()
        {
            return savepoint;
        }

    }
}
