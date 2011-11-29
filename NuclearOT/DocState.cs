using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuclearOT
{
    public class DocTransform
    {
        //----------------------------------------------------------------------
        List<DocOp>         mlOps;

        //----------------------------------------------------------------------
        public DocTransform()
        {
            mlOps = new List<DocOp>();
        }

        //----------------------------------------------------------------------
        public void Insert( UInt16 _uiSite, int _iPosition, string _strValue )
        {
            for( int i = 0; i < _strValue.Length; i++ )
            {
                DocOp insertOp = DocOp.Insert( _uiSite, _iPosition + i, _strValue[i] );
                mlOps.Add( insertOp );
            }
        }

        public void Delete( UInt16 _uiSite, int _iPosition, int _iCount )
        {
            for( int i = 0; i < _iCount; i++ )
            {
                DocOp deleteOp = DocOp.Delete( _uiSite, _iPosition );
                mlOps.Add( deleteOp );
            }
        }

        //----------------------------------------------------------------------
        public string Transform( string _strDocument )
        {
            for( int iOp = 0; iOp < mlOps.Count; iOp++ )
            {
                switch( mlOps[iOp].Type )
                {
                    case DocOpType.Insert:
                        _strDocument = _strDocument.Insert( mlOps[iOp].Position, mlOps[iOp].Char.ToString() );
                        break;
                    case DocOpType.Delete:
                        _strDocument = _strDocument.Remove( mlOps[iOp].Position, 1 );
                        break;
                }
            }

            return _strDocument;
        }

        //----------------------------------------------------------------------
        enum TransformMode
        {
            Include,
            Exclude
        }

        void TransformOp( DocOp _refOp, TransformMode _mode )
        {
            for( int iOp = 0; iOp < mlOps.Count; iOp++ )
            {
                DocOp op = mlOps[iOp];

                // Based on http://www3.ntu.edu.sg/home/czsun/projects/otfaq/#_Toc308616441
                switch( _refOp.Type )
                {
                    case DocOpType.Insert:
                            
                        switch( op.Type )
                        {
                            case DocOpType.Insert:
                                if( op.Position < _refOp.Position || op.Position == _refOp.Position && op.Site > _refOp.Site )
                                {
                                    continue;
                                }
                                else
                                {
                                    mlOps[iOp] = DocOp.Insert( op.Site, op.Position + 1, op.Char );
                                }
                                break;

                            case DocOpType.Delete:
                                if( op.Position <= _refOp.Position )
                                {
                                    continue;
                                }
                                else
                                {
                                    mlOps[iOp] = DocOp.Insert( op.Site, op.Position - 1, op.Char );
                                }
                                break;
                        }

                        break;
                    case DocOpType.Delete:

                        switch( op.Type )
                        {
                            case DocOpType.Insert:
                                if( op.Position < _refOp.Position )
                                {
                                    continue;
                                }
                                else
                                {
                                    mlOps[iOp] = DocOp.Delete( op.Site, op.Position + 1 );
                                }
                                break;

                            case DocOpType.Delete:
                                if( op.Position < _refOp.Position )
                                {
                                    continue;
                                }
                                else
                                if( op.Position > _refOp.Position )
                                {
                                    mlOps[iOp] = DocOp.Delete( op.Site, op.Position - 1 );
                                }
                                else
                                {
                                    mlOps.RemoveAt( iOp );
                                    iOp--;
                                }
                                break;
                        }

                        break;
                }
            }

            mlOps.Add( _refOp );
        }
    }
}
