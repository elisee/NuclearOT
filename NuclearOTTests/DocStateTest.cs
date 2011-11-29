using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NuclearOT;

namespace NuclearOTTests
{
    [TestFixture]
    public class DocStateTest
    {
        [Test]
        public void SimpleLocalInsertion()
        {
            string strInitial           = "This is a test.";
            string strExpectedResult    = "This is a successful test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Insert( 0, 10, "successful " );

            string strResult = docTransform.Transform( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void SimpleLocalDeletion()
        {
            string strInitial           = "This is a failed test.";
            string strExpectedResult    = "This is a test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Delete( 0, 10, 7 );

            string strResult = docTransform.Transform( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }

        [Test]
        public void MultipleLocalOperations()
        {
            string strInitial           = "This is a failed test.";
            string strExpectedResult    = "This is a successful test.";

            DocTransform docTransform = new DocTransform();
            docTransform.Delete( 0, 10, 4 );
            docTransform.Insert( 0, 10, "success");
            docTransform.Insert( 0, 17, "ful " );
            docTransform.Delete( 0, 21, 3 );

            string strResult = docTransform.Transform( strInitial );

            Assert.AreEqual( strExpectedResult, strResult );
        }
    }
}
