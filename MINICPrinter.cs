using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    class MINICPrinter
    {
        private StreamWriter m_ostream;
        private string m_dotName;
        private CodeContainer m_rep;

        public MINICPrinter(string dotFileName, CodeContainer rep)
        {
            m_ostream = new StreamWriter(dotFileName);
            m_dotName = dotFileName;
            m_rep = rep;
        }

        public void printer()
        {
            m_ostream.WriteLine(m_rep.ToString());
            m_ostream.Close();
        }  
    }
}
