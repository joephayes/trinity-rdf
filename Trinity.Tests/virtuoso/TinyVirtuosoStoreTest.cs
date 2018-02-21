﻿// LICENSE:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// AUTHORS:
//
//  Moritz Eberl <moritz@semiodesk.com>
//  Sebastian Faubel <sebastian@semiodesk.com>
//
// Copyright (c) Semiodesk GmbH 2015

using System;
using System.IO;
using NUnit.Framework;
using Semiodesk.Trinity;
using System.Linq;
using System.Reflection;
using Semiodesk.Trinity.Test;

namespace Semiodesk.Trinity.Test
{


    [TestFixture]
    class TinyVirtuosoStoreTest
    {
        IStore Store;

        [SetUp]
        public void SetUp()
        {
            string connectionString = SetupClass.ConnectionString;

            Store = StoreFactory.CreateStore(string.Format("{0};rule=urn:semiodesk/test/ruleset", connectionString));
            Store.LoadOntologySettings();
        }

        [TearDown]
        public void TearDown()
        {
            Store.Dispose();
            Store = null;
        }



        [Test]
        public void LoadOntologiesFromFileTest()
        {
            var model = Store.GetModel(new Uri("http://purl.org/dc/elements/1.1/"));

            var res = model.GetResources(new SparqlQuery("SELECT ?s ?p ?o where { ?s ?p ?o. }"));
            var x = res.ToList();

        }

        [Test]
        public void AddModelTest()
        {
            Uri testModel = new Uri("ex:Test");

            IModel m = Store.CreateModel(testModel);

            Assert.IsNotNull(m);
        }

        [Test]
        public void ContainsModelTest()
        {
            Uri testModel = new Uri("ex:Test");

            Assert.IsFalse(Store.ContainsModel(testModel));

            IModel m = Store.CreateModel(testModel);

            var res = m.CreateResource(new Uri("ex:test:resource"));
            
            res.AddProperty(new Property(new Uri("ex:test:property")), "var");
            res.Commit();

            Assert.IsTrue(Store.ContainsModel(testModel));
            Assert.IsFalse(Store.ContainsModel(new Uri("ex:NoTest")));
        }

        [Test]
        public void GetModelTest()
        {
            Uri testModel = new Uri("ex:Test");

            Assert.IsFalse(Store.ContainsModel(testModel));

            IModel m = Store.CreateModel(testModel);

            var res = m.CreateResource(new Uri("ex:test:resource"));

            res.AddProperty(new Property(new Uri("ex:test:property")), "var");
            res.Commit();

            Assert.IsTrue(Store.ContainsModel(testModel));

            IModel model2 = Store.GetModel(testModel);
            Assert.AreEqual(testModel, model2.Uri);

            Assert.IsTrue(model2.ContainsResource(res));
        }

        [Test]
        public void RemoveModelTest()
        {
            Uri testModel = new Uri("ex:Test");

            Assert.IsFalse(Store.ContainsModel(testModel));

            IModel m = Store.CreateModel(testModel);

            var res = m.CreateResource(new Uri("ex:test:resource"));

            res.AddProperty(new Property(new Uri("ex:test:property")), "var");
            res.Commit();

            Assert.IsTrue(Store.ContainsModel(testModel));

            IModel model2 = Store.GetModel(testModel);
            Assert.AreEqual(testModel, model2.Uri);

            Store.RemoveModel(testModel);
            Assert.IsFalse(Store.ContainsModel(testModel));

            model2 = Store.GetModel(testModel);
            Assert.IsTrue(model2.IsEmpty);
        }

    }
}