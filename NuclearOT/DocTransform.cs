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
        public int          ParentStateIndex;

        //----------------------------------------------------------------------
        public DocTransform( int _iParentStateIndex )
        {
            mlOps               = new List<DocOp>();
            ParentStateIndex    = _iParentStateIndex;
        }

        public DocTransform Clone()
        {
            DocTransform clone = new DocTransform( ParentStateIndex );
            clone.mlOps = new List<DocOp>( mlOps );
            return clone;
        }

        public void Serialize( dynamic _writer )
        {
            _writer.Write( ParentStateIndex );
            _writer.Write( mlOps.Count );

            foreach( DocOp op in mlOps )
            {
                _writer.Write( (byte)op.Type );
                _writer.Write( op.Site );
                _writer.Write( op.Position );

                switch( op.Type )
                {
                    case DocOpType.Insert:
                        _writer.Write( op.Char.ToString() );
                        break;
                    case DocOpType.Delete:
                        break;
                }
            }
        }

        public static DocTransform Deserialize( dynamic _reader )
        {
            DocTransform transform = new DocTransform( _reader.ReadInt32() );

            int iOpCount = _reader.ReadInt32();

            List<DocOp> lOps = new List<DocOp>();
            for( int iOp = 0; iOp < iOpCount; iOp++ )
            {
                DocOpType opType = (DocOpType)_reader.ReadByte();

                switch( opType )
                {
                    case DocOpType.Insert:
                        lOps.Add( DocOp.Insert( _reader.ReadUInt16(), _reader.ReadInt32(), _reader.ReadString()[0] ) );
                        break;
                    case DocOpType.Delete:
                        lOps.Add( DocOp.Delete( _reader.ReadUInt16(), _reader.ReadInt32() ) );
                        break;
                }
            }

            transform.Append( lOps );

            return transform;
        }

        public bool CheckSite( UInt16 _uiMemberId )
        {
            foreach( DocOp op in mlOps)
            {
                if( op.Site != _uiMemberId )
                {
                    return false;
                }
            }

            return true;
        }

        //----------------------------------------------------------------------
        public string Apply( string _strDocument )
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
        public static List<DocOp> Insert( UInt16 _uiSite, int _iPosition, string _strValue )
        {
            List<DocOp> lOps = new List<DocOp>();

            for( int i = 0; i < _strValue.Length; i++ )
            {
                DocOp insertOp = DocOp.Insert( _uiSite, _iPosition + i, _strValue[i] );
                lOps.Add( insertOp );
            }

            return lOps;
        }

        public static List<DocOp> Delete( UInt16 _uiSite, int _iPosition, int _iCount )
        {
            List<DocOp> lOps = new List<DocOp>();

            for( int i = 0; i < _iCount; i++ )
            {
                DocOp deleteOp = DocOp.Delete( _uiSite, _iPosition );
                lOps.Add( deleteOp );
            }

            return lOps;
        }

        //----------------------------------------------------------------------
        public void Append( List<DocOp> _lOps )
        {
            mlOps.AddRange( _lOps );
        }

        //----------------------------------------------------------------------
        public void Include( DocTransform _transform )
        {
            Transform( _transform );
            mlOps.InsertRange( 0, _transform.mlOps );
        }

        public void Transform( DocTransform _transform )
        {
            if( ParentStateIndex == _transform.ParentStateIndex - 1 )
            {
                throw new InvalidOperationException();
            }

            foreach( DocOp refOp in _transform.mlOps )
            {
                TransformOp( refOp );
            }

            ParentStateIndex++;
        }

        //----------------------------------------------------------------------
        void TransformOp( DocOp _refOp )
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
        }
    }
}
