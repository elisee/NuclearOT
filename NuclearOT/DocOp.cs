using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuclearOT
{
    //--------------------------------------------------------------------------
    public enum DocOpType
    {
        Insert,
        Delete
    }

    //--------------------------------------------------------------------------
    public struct DocOp
    {
        public DocOpType        Type;
        public UInt16           Site;
        public int              Position;
        public char             Char;

        //----------------------------------------------------------------------
        public static DocOp Insert( UInt16 _uiSite, int _iPosition, char _char )
        {
            DocOp docOp     = new DocOp();
            docOp.Type      = DocOpType.Insert;
            docOp.Site      = _uiSite;
            docOp.Position  = _iPosition;
            docOp.Char      = _char;

            return docOp;
        }

        //----------------------------------------------------------------------
        public static DocOp Delete( UInt16 _uiSite, int _iPosition )
        {
            DocOp docOp     = new DocOp();
            docOp.Type      = DocOpType.Delete;
            docOp.Site      = _uiSite;
            docOp.Position  = _iPosition;

            return docOp;
        }
    }
}
