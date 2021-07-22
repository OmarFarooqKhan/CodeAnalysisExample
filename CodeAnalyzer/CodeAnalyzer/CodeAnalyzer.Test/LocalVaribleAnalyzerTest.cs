using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CodeAnalyzer.Test.CSharpAnalyzerVerifier<
    CodeAnalyzer.LocalVaribleAnalyzer>;

namespace CodeAnalyzer.Test
{
    [TestClass]
    public class LocalVaribleAnalyzerTest
    {

        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class test
        {   
            public void testMethod()
            {
                string {|#0:name|};
            }
        }
    }";

            DiagnosticResult expected = VerifyCS.Diagnostic("AddLocalPrefix").WithLocation(15, 24).WithArguments("name");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class test
        {   
            public void testMethod()
            {
                string _name, {|#0:blame|};
            }
        }
    }";

            DiagnosticResult expected = VerifyCS.Diagnostic("AddLocalPrefix").WithLocation(15, 31).WithArguments("blame");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}