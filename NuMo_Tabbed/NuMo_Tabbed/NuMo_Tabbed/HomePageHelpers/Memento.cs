using System;
using System.Collections.Generic;
using System.Text;

namespace NuMo_Tabbed
{
    class Memento
    {
        public string savepoint;


        public Memento(string save)
        {
            savepoint = save;
        }

        public Boolean getLastState()
        {
            if (savepoint != null)
            {
                DataAccessor db = DataAccessor.getDataAccessor();
                db.rollback(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addState(string save)
        {
            savepoint = save;
        }

        public string getSavepoint()
        {
            if (savepoint != null)
            {
                return savepoint;
            }
            else
            {
                return null;
            }

        }
    }
}
