using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susis
{
    [TestFixture]
    class Tests
    {
        [Test]
        public void DataDeklaracijasTests()
        {
            // komits
            string targetResult = ".586\n.MODEL flat, stdcall\ninclude \\masm32\\include\\windows.inc\ninclude \\masm32\\macros\\macros.asm\ninclude \\masm32\\include\\masm32.inc\ninclude \\masm32\\include\\gdi32.inc\ninclude \\masm32\\include\\user32.inc\ninclude \\masm32\\include\\kernel32.inc\nincludelib\\masm32\\lib\\masm32.lib\nincludelib\\masm32\\lib\\gdi32.lib\nincludelib\\masm32\\lib\\user32.lib\nincludelib\\masm32\\lib\\kernel32.lib\ninclude \\masm32\\include\\msvcrt.inc\nincludelib \\masm32\\lib\\msvcrt.lib\r\n"
                                  + ".data\r\nX db 4\nmov eax, input (\"beigas\")\nret\n main ENDP\nEND main";

            string input = "NOSAUKT X = 4";

            string result = MainWindow.SuperKrutāFunkcijaKuraKompilēKodu(input);

            Assert.AreEqual(targetResult, result);
        }

        [Test]
        public void IzvadīšanasTests() 
        {
            string targetResult = ".586\n.MODEL flat, stdcall\ninclude \\masm32\\include\\windows.inc\ninclude \\masm32\\macros\\macros.asm\ninclude \\masm32\\include\\masm32.inc\ninclude \\masm32\\include\\gdi32.inc\ninclude \\masm32\\include\\user32.inc\ninclude \\masm32\\include\\kernel32.inc\nincludelib\\masm32\\lib\\masm32.lib\nincludelib\\masm32\\lib\\gdi32.lib\nincludelib\\masm32\\lib\\user32.lib\nincludelib\\masm32\\lib\\kernel32.lib\ninclude \\masm32\\include\\msvcrt.inc\nincludelib \\masm32\\lib\\msvcrt.lib\r\n"
                                    + ".data\r\nX db 4\r\n .code\nmain PROC\r\n mov eax , X\r\nprint str$(eax)\nmov eax, input (\"beigas\")\nret\n main ENDP\nEND main"; 
        String input = "NOSAUKT X = 4\nIZVADĪT X";
            string result = MainWindow.SuperKrutāFunkcijaKuraKompilēKodu(input);
            Assert.AreEqual(targetResult, result);
        }

    }
}
