using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NuclearOT;

namespace NuclearOTTests
{
    [TestFixture]
    public class DocTransformTest
    {
        [Test]
        public void SimpleLocalInsertion()
        {
            string strInitial           = "This is a test.";
            string strExpectedResult    = "This is a successful test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Append( DocTransform.Insert( 0, 10, "successful " ) );

            string strResult = docTransform.Apply( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void SimpleLocalDeletion()
        {
            string strInitial           = "This is a failed test.";
            string strExpectedResult    = "This is a test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Append( DocTransform.Delete( 0, 10, 7 ) );

            string strResult = docTransform.Apply( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void MultipleLocalOperations()
        {
            string strInitial           = "This is a failed test.";
            string strExpectedResult    = "This is a successful test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Append( DocTransform.Delete( 0, 10, 4 ) );
            docTransform.Append( DocTransform.Insert( 0, 10, "success") );
            docTransform.Append( DocTransform.Insert( 0, 17, "ful " ) );
            docTransform.Append( DocTransform.Delete( 0, 21, 3 ) );

            string strResult = docTransform.Apply( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void IncludeRemoteChangesInEmptyTransform()
        {
            string strInitial           = "String";
            string strExpectedResult    = "PrefixString";

            DocTransform remoteTransform = new DocTransform();
            remoteTransform.Append( DocTransform.Insert( 0, 0, "Prefix" ) );

            DocTransform localTransform = new DocTransform();
            localTransform.Include( remoteTransform );

            string strResult = localTransform.Apply( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void IncludeRemoteChangesInExistingTransform()
        {
            string strInitial           = "InitialString";
            string strExpectedResult    = "PrefixChangedInitialString";
            
            DocTransform remoteTransform = new DocTransform();
            remoteTransform.Append( DocTransform.Insert( 0, 0, "Prefix" ) );

            DocTransform localTransform = new DocTransform();
            localTransform.Append( DocTransform.Insert( 0, 0, "Changed" ) );
            localTransform.Include( remoteTransform );

            string strResult = localTransform.Apply( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }
    }
}
